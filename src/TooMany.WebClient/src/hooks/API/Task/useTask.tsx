import { useQuery } from 'react-query';
import useFetcher from '@tm/hooks/useFetcher';
import type * as Task from '@tm/types/task';

export default function (taskName?: string) {
	const fetcher = useFetcher();
	return useQuery<Task.IMeta>(
		['task', taskName],
		async () => {
			console.log('fetching task:', taskName);
			const result = await fetcher.getRequest<Task.IMeta>(
				`${env.apiV1Url}/task/${taskName}`,
			);
			return result;
		},
		{
			enabled: typeof taskName !== 'undefined',
		},
	);
}
