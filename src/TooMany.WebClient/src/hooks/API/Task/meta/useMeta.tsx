import { useQuery, useQueryClient } from 'react-query';
import useApi from '../../useApi';
import type * as Task from '@tm/types/task';
import { getQueryKey } from './helpers';
import type { TaskName } from './types';

export default function (suspense = true) {
	const api = useApi();
	return useQuery<Task.IMeta[]>(getQueryKey(), api.task.list, {
		suspense,
	});
}

export function useCache(taskName: TaskName) {
	const queryClient = useQueryClient();
	return (data: Task.IMeta) => {
		console.log('updating all cache', taskName, data);
		queryClient.setQueryData<Task.IMeta[]>(getQueryKey(), (tasks = []) => {
			if (data.name !== taskName) {
				return [...tasks, data];
			}
			const newTasks: Task.IMeta[] = [];
			for (const task of tasks) {
				newTasks.push(task.name === data.name ? data : task);
			}
			return newTasks;
		});
	};
}
