import { useQuery } from 'react-query';
import useFetcher from '@tm/hooks/useFetcher';
import type * as Task from '@tm/types/task';

export default function (name?: string) {
	const fetcher = useFetcher();
	return useQuery<Task.ILog[]>(
		['task', 'log', name],
		async () => {
			console.log('fetch log by name:', name);
			const result = await fetcher.getRequest<Task.ILog[]>(
				`${env.apiV1Url}/task/${name}/logs`,
			);
			return result;
		},
		{
			enabled: typeof name !== 'undefined',
			refetchOnWindowFocus: false,
			staleTime: Infinity, // realtime will handle state after mount
		},
	);
}
