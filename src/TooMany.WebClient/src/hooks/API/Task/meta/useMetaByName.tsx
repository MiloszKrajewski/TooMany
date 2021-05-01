import { useQuery, useQueryClient } from 'react-query';
import useApi from '../../useApi';
import type * as Task from '@tm/types/task';
import { getQueryKey } from './helpers';
import { useCache as useMetaCache } from './useMeta';

export default function (name: string, suspense = true) {
	const api = useApi();
	const metaCache = useMetaCache(name);
	return useQuery<Task.IMeta>(
		getQueryKey(name),
		async () => {
			const result = await api.task.meta(name);
			metaCache(result);
			return result;
		},
		{
			suspense,
		},
	);
}

export function useCache() {
	const queryClient = useQueryClient();
	return (name: string, meta: Task.IMeta) => {
		queryClient.setQueryData<Task.IMeta>(getQueryKey(name), meta);
	};
}
