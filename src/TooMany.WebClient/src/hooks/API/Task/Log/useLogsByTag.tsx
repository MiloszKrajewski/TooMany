import { useQuery } from 'react-query';

import type * as Task from 'types/task';

import useApi from '../../useApi';

import { getTagQueryKey, transformLog } from './helpers';
import settings from './settings';
import type { TaskName } from './types';

export function query(
	taskNames: TaskName[],
	fetcher: (task: string) => Promise<Task.ILog[]>,
) {
	return async function () {
		const logFetchers = taskNames.map(async (name): Promise<
			Task.ILog[]
		> => {
			const logs = await fetcher(name as string);
			const iLogs = logs.map(transformLog(name || ''));
			if (settings.stripEmptyLogs) {
				return iLogs.filter((log) => log.text);
			} else {
				return iLogs;
			}
		});

		const results = await Promise.all(logFetchers);

		const out = results.flat().sort((a, b) => a.time - b.time);
		return out;
	};
}

export default function (taskNames: TaskName[] = [], tag: string) {
	const api = useApi();

	return useQuery(getTagQueryKey(tag), query(taskNames, api.task.logs));
}
