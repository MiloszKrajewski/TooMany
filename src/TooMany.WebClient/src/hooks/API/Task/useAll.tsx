import { useQuery, useQueryClient } from 'react-query';
import useApi from '../useApi';
import type * as Task from '@tm/types/task';
import { useByNameCache } from './useByName';

const getQueryKey = () => ['tasks'];

export default function () {
	const api = useApi();
	const setByNameCache = useByNameCache();
	return useQuery<Task.IMeta[]>(getQueryKey(), async () => {
		const results = await api.task.list();
		for (const result of results) {
			setByNameCache(result);
		}
		return results;
	});
}

export function useAllCache(name?: string) {
	const queryClient = useQueryClient();
	return (data: Task.IMeta) => {
		console.log('updating all cache', name, data);
		queryClient.setQueryData<Task.IMeta[]>(getQueryKey(), (tasks = []) => {
			if (data.name !== name) {
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
