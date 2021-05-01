import type * as Task from '@tm/types/task';
import useApi from '../../useApi';
import settings from './settings';

export const getQueryKey = (name?: string) => ['task', name, 'log'];

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

export function transformLog(name: string) {
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
export function fetchLog(fetcher: TaskLogsFn, name?: string) {
	return {
		queryKey: getQueryKey(name),
		queryFn: async function (): Promise<Task.ILog[]> {
			const logs = await fetcher(name as string);
			const iLogs = logs.map(transformLog(name || ''));
			if (settings.stripEmptyLogs) {
				return iLogs.filter((log) => log.text);
			}
			return iLogs;
		},
		enabled: typeof name === 'string',
		refetchOnWindowFocus: false,
		staleTime: Infinity, // realtime will handle state after mount
	};
}
