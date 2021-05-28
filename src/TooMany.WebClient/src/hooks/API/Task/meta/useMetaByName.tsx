import { useQuery, useQueryClient } from 'react-query';

import type * as Task from 'types/task';

import useApi from '../../useApi';

import { getQueryKey } from './helpers';
import { useCache as useMetaCache } from './useMeta';

export default function (name: string, suspense = true) {
	const api = useApi();
	const metaCache = useMetaCache();
	return useQuery<Task.IMeta>(
		getQueryKey(name),
		async () => {
			const result = await api.task.meta(name);
			metaCache(result, name);
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
