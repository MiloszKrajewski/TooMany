import { useQuery } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';

type ReturnValue = Task.ILog[];

export default function (name?: string) {
	const api = useApi();
	return useQuery<ReturnValue>(
		['task', name],
		() => api.task.start(name as string, {}),
		{
			enabled: typeof name === 'string',
		},
	);
}
