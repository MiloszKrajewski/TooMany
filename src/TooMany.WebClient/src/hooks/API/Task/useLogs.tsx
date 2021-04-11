import type { UseQueryResult } from 'react-query';
import {
	useQuery,
	useQueries,
	useQueryClient,
	useIsFetching,
} from 'react-query';
import useFetcher from '../useFetcher';
import type * as Task from '@tm/types/task';
import SignalR from '@tm/SignalR';
import { useEffect, useRef } from 'react';
import type * as Realtime from '@tm/types/realtime';

let culture: string;
switch (typeof navigator.languages) {
	case 'string':
		culture = navigator.languages;
		break;
	case 'object':
		culture = navigator.languages[0];
		break;
	default:
		culture = 'en-GB';
		break;
}

const stripEmptyLogs = true; // TODO: move to user settings

function transformLog(name: string) {
	return function (log: Task.ILog) {
		return {
			...log,
			task: name || '',
			time: new Date(log.timestamp).getTime(),
			formattedTimestamp: new Date(log.timestamp).toLocaleString(culture, {
				hour12: false,
			}),
		};
	};
}

function fetchLog(fetcher: ReturnType<typeof useFetcher>, name?: string) {
	return {
		queryKey: ['task', 'log', name],
		queryFn: async function (): Promise<Task.ILog[]> {
			console.log('fetch log by name:', name);
			const logs = await fetcher.getRequest<Task.ILog[]>(
				`${env.apiV1Url}/task/${name}/logs`,
			);
			const iLogs = logs.map(transformLog(name || ''));
			if (stripEmptyLogs) {
				return iLogs.filter((log) => log.text);
			}
			return iLogs;
		},
		enabled: typeof name !== 'undefined',
		refetchOnWindowFocus: false,
		staleTime: Infinity, // realtime will handle state after mount
	};
}

type Name = string | undefined;

export const useRealtime = function (name?: Name) {
	const id = name || null;
	const queryKey = ['task', 'log', name];
	const queryClient = useQueryClient();
	const isFetchingLogs = useIsFetching(queryKey);

	useEffect(() => {
		if (isFetchingLogs || id === null) return;
		const fn = SignalR.onTaskLog(id, (log) => {
			queryClient.setQueryData<Task.ILog[]>(queryKey, (state) => {
				if (stripEmptyLogs && !log.text) {
					return state || [];
				}
				const newLog = transformLog(name || '')(log);
				if (!state) return [newLog];
				return [...state, newLog];
			});
		});
		return () => {
			SignalR.offTaskLog(fn);
		};
	}, [isFetchingLogs, name]);
};

export default function (name: Name) {
	const fetcher = useFetcher();
	useRealtime(name);
	return useQuery<Task.ILog[]>(fetchLog(fetcher, name));
}

export const useMultipleRealtime = function (taskNames?: Name[]) {
	const names = taskNames || [];
	const queryClient = useQueryClient();

	useEffect(() => {
		const fns = names.map((name) => {
			const id = name || null;
			const queryKey = ['task', 'log', name];
			if (queryClient.isFetching() || id === null) return;
			return SignalR.onTaskLog(id, (log) => {
				queryClient.setQueryData<Task.ILog[]>(queryKey, (state) => {
					if (stripEmptyLogs && !log.text) {
						return state || [];
					}
					const newLog = transformLog(name || '')(log);
					if (!state) return [newLog];
					return [...state, newLog];
				});
			});
		});
		return () => {
			for (const fn of fns) {
				if (!fn) continue;
				SignalR.offTaskLog(fn);
			}
		};
	}, [names]);
};

export const useMultiple = function (names: Name[]) {
	const fetcher = useFetcher();
	useMultipleRealtime(names);

	return useQueries(
		names.map((name) => fetchLog(fetcher, name)),
	) as UseQueryResult<Task.ILog[], unknown>[];
};
