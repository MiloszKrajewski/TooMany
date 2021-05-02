import { useQueryClient } from 'react-query';
import type * as Task from '@tm/types/task';
import settings from './settings';

import { getQueryKey, transformLog } from './helpers';

export default function () {
	const queryClient = useQueryClient();
	return (name: string, log: Task.ILog) => {
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
