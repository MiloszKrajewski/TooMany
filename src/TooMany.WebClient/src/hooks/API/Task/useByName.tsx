import { useQuery } from 'react-query';
import useFetcher from '@tm/hooks/useFetcher';
import type * as Task from '@tm/types/task';

export default function (name?: string) {
	const fetcher = useFetcher();
	return useQuery<Task.IMeta>(
		['task', name],
		async () => {
			console.log('fetch by name:', name);
			const result = await fetcher.getRequest<Task.IMeta>(
				`${env.apiV1Url}/task/${name}`,
			);
			return result;
		},
		{
			enabled: typeof name !== 'undefined',
		},
	);
}
