import { useQueryClient } from 'react-query';
import type * as Task from '@tm/types/task';
import SignalR from '@tm/SignalR';
import { useEffect } from 'react';
import settings from './settings';

import type { TaskName } from './types';
import { getQueryKey, transformLog } from './helpers';

function useCache() {
	const queryClient = useQueryClient();
	return (log: Task.ILog, name: string) => {
		queryClient.setQueryData<Task.ILog[]>(getQueryKey(name), (state) => {
			if (settings.stripEmptyLogs && !log.text) {
				return state || [];
			}
			const newLog = transformLog(name || '')(log);
			if (!state) return [newLog];
			return [...state, newLog];
		});
	};
}

export default function (taskNames: TaskName[] = []) {
	const cache = useCache();
	const queryClient = useQueryClient();

	useEffect(() => {
		const fns = taskNames.map((taskName) => {
			const id = taskName || null;

			if (id === null || queryClient.isFetching(getQueryKey(id))) {
				return;
			}

			return SignalR.onTaskLog(id, (log) => {
				cache(log, id);
			});
		});
		return () => {
			for (const fn of fns) {
				if (typeof fn !== 'function') continue;
				SignalR.offTaskLog(fn);
			}
		};
	}, [taskNames]);
}
