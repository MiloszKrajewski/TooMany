import { useQuery } from 'react-query';
import useApi from '../useApi';
import type * as Task from '@tm/types/task';

export default function (name?: string) {
	const api = useApi();
	return useQuery<Task.IMeta>(
		['task', name],
		() => api.task.meta(name as string),
		{
			enabled: typeof name === 'string',
		},
	);
}
