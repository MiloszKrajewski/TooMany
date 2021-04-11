import { useQuery } from 'react-query';
import useFetcher from '../useFetcher';
import type * as Task from '@tm/types/task';

export default function () {
	const fetcher = useFetcher();
	return useQuery<Task.IMeta[]>('tasks', async () => {
		console.log('fetch all');
		const result = await fetcher.getRequest<Task.IMeta[]>(
			`${env.apiV1Url}/task`,
		);
		return result;
	});
}
