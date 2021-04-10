import { useQuery } from 'react-query';
import useFetcher from '@tm/hooks/useFetcher';
import type * as Task from '@tm/types/task';

export default function () {
	const fetcher = useFetcher();
	return useQuery<Task.Meta>('list', async () => {
		const result = await fetcher.getRequest<Task.Meta>(`${env.apiV1Url}/task`);
		return result;
	});
}
