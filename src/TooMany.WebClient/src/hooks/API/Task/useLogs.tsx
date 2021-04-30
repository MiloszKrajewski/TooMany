import type { UseQueryResult } from 'react-query';
import {
	useQuery,
	useQueries,
	useQueryClient,
	useIsFetching,
} from 'react-query';
import useApi from '../useApi';
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

type ReturnValue = Task.ILog[];

const stripEmptyLogs = true; // TODO: move to user settings
const getQueryKey = (name?: string) => ['task', name, 'log'];

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

type UseApi = ReturnType<typeof useApi>;
type TaskLogsFn = UseApi['task']['logs'];
function fetchLog(fetcher: TaskLogsFn, name?: string) {
	return {
		queryKey: getQueryKey(name),
		queryFn: async function (): Promise<ReturnValue> {
			const logs = await fetcher(name as string);
			const iLogs = logs.map(transformLog(name || ''));
			if (stripEmptyLogs) {
				return iLogs.filter((log) => log.text);
			}
			return iLogs;
		},
		enabled: typeof name === 'string',
		refetchOnWindowFocus: false,
		staleTime: Infinity, // realtime will handle state after mount
	};
}

function useFetchLog(name?: string) {
	const api = useApi();
	return fetchLog(api.task.logs, name);
}

type Name = string | undefined;

export const useRealtime = function (name?: Name) {
	const id = name || null;
	const queryKey = getQueryKey(name);
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
	useRealtime(name);
	return useQuery<ReturnValue>(useFetchLog(name));
}

export const useMultipleRealtime = function (taskNames?: Name[]) {
	const names = taskNames || [];
	const queryClient = useQueryClient();

	useEffect(() => {
		const fns = names.map((name) => {
			const id = name || null;
			const queryKey = getQueryKey(name);
			if (queryClient.isFetching() || id === null) return;
			return SignalR.onTaskLog(id, (log) => {
				queryClient.setQueryData<ReturnValue>(queryKey, (state) => {
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
	useMultipleRealtime(names);
	return useQueries(names.map(useFetchLog)) as UseQueryResult<
		ReturnValue,
		unknown
	>[];
};
