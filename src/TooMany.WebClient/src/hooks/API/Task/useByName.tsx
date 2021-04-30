import { useQuery, useQueryClient } from 'react-query';
import useApi from '../useApi';
import type * as Task from '@tm/types/task';
import { useAllCache } from './useAll';

const getQueryKey = (name?: string) => ['tasks', name];

export default function (name?: string) {
	const api = useApi();
	const setAllCache = useAllCache(name);
	return useQuery<Task.IMeta>(
		getQueryKey(name),
		async () => {
			const result = await api.task.meta(name as string);
			setAllCache(result);
			return result;
		},
		{
			enabled: typeof name === 'string',
		},
	);
}

export function useByNameCache() {
	const queryClient = useQueryClient();
	return (data: Task.IMeta) => {
		console.log('updating name cache', data);
		queryClient.setQueryData<Task.IMeta>(getQueryKey(data.name), () => data);
	};
}
