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
import { useEffect } from 'react';

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

type Name = string | undefined;

function useRealtimeCacheSetter() {
	const queryClient = useQueryClient();
	return (log: Task.ILog, name: string) => {
		queryClient.setQueryData<ReturnValue>(getQueryKey(name), (state) => {
			if (stripEmptyLogs && !log.text) {
				return state || [];
			}
			const newLog = transformLog(name || '')(log);
			if (!state) return [newLog];
			return [...state, newLog];
		});
	};
}

export function useRealtime(names: Name[] = []) {
	const realtimeCacheSetter = useRealtimeCacheSetter();
	const queryClient = useQueryClient();

	useEffect(() => {
		const fns = names.map((name) => {
			const id = name || null;

			if (id === null || queryClient.isFetching(getQueryKey(id))) {
				return;
			}

			return SignalR.onTaskLog(id, (log) => {
				realtimeCacheSetter(log, id);
			});
		});
		return () => {
			for (const fn of fns) {
				if (typeof fn !== 'function') continue;
				SignalR.offTaskLog(fn);
			}
		};
	}, [names]);
}

export default function (name: Name) {
	useRealtime([name]);

	const api = useApi();
	return useQuery<ReturnValue>(fetchLog(api.task.logs, name));
}

export function useMultiple(names: Name[]) {
	useRealtime(names);

	const api = useApi();
	return useQueries(
		names.map((name) => fetchLog(api.task.logs, name)),
	) as UseQueryResult<ReturnValue, unknown>[];
}
