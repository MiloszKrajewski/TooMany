import { useQueryClient } from 'react-query';

import type * as Task from 'types/task';

import { getQueryKey, getTagQueryKey, transformLog } from './helpers';
import settings from './settings';

function handleLog(name: string, log: Task.ILog) {
	return function (state?: Task.ILog[]) {
		if (settings.stripEmptyLogs && !log.text) {
			return state || [];
		}
		const newLog = transformLog(name || '')(log);
		if (!state) return [newLog];
		state.push(newLog);
		return state;
	};
}

export default function () {
	const queryClient = useQueryClient();
	return (name: string, log: Task.ILog) => {
		const meta = queryClient.getQueryData<Task.IMeta>([
			'tasks',
			'meta',
			name,
		]);
		if (typeof meta?.tags !== 'undefined' && meta.tags.length >= 1) {
			for (const tag of meta.tags) {
				queryClient.setQueryData<Task.ILog[]>(
					getTagQueryKey(tag),
					handleLog(name, log),
				);
			}
		}
		queryClient.setQueryData<Task.ILog[]>(
			getQueryKey(name),
			handleLog(name, log),
		);
	};
}
