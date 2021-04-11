import { useMutation, useQueryClient } from 'react-query';
import useFetcher from '@tm/hooks/useFetcher';
import type * as Task from '@tm/types/task';

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
	const fetcher = useFetcher();
	return useMutation<Task.IMeta, unknown, ITask>(
		'task',
		async (payload) => {
			console.log('create', name);
			const result = await fetcher.postRequest<Task.IMeta, ITask>(
				`${env.apiV1Url}/task/${payload.name}`,
				payload,
			);
			return result;
		},
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
