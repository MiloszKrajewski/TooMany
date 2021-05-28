import type * as Task from 'types/task';

import { getCulture } from '@tm/helpers/culture';

import useApi from '../../useApi';

import settings from './settings';

const culture = getCulture();
export const getQueryKey = (name?: string) => ['task', name, 'log'];

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
