import { useMutation } from 'react-query';
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

export default function () {
	const fetcher = useFetcher();
	return useMutation<Task.Meta, unknown, ITask>('task', async (payload) => {
		const result = await fetcher.postRequest<Task.Meta, ITask>(
			`${env.apiV1Url}/task/${payload.name}`,
			payload,
		);
		return result;
	});
}
