import { useMutation, useQueryClient } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';

interface ITask {
	name: string;
	executable?: string;
	arguments?: string;
	directory?: string;
	environment?: Record<string, string>;
	tags?: string[];
}

export default function (name: string) {
	const queryClient = useQueryClient();
	const api = useApi();
	return useMutation<Task.IMeta, unknown, ITask>(
		'task',
		(payload) => api.task.create<ITask>(name, payload),
		{
			onSuccess(result) {
				queryClient.setQueryData<Task.IMeta>(
					['task', result.name],
					() => result,
				);
				queryClient.setQueryData<Task.IMeta[]>('tasks', (tasks = []) => {
					if (result.name !== name) {
						return [...tasks, result];
					}
					const newTasks: Task.IMeta[] = [];
					for (const task of tasks) {
						newTasks.push(task.name === name ? result : task);
					}
					return newTasks;
				});
			},
		},
	);
}
