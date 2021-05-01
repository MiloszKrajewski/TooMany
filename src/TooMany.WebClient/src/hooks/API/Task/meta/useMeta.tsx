import { useQuery, useQueryClient } from 'react-query';
import useApi from '../../useApi';
import type * as Task from '@tm/types/task';
import { getQueryKey } from './helpers';
import type { TaskName } from './types';

export default function (suspense = true) {
	const api = useApi();
	return useQuery<Task.IMeta[]>(getQueryKey(), api.task.list, {
		suspense,
	});
}

export function useCache() {
	const queryClient = useQueryClient();
	return (meta: Task.IMeta, name: TaskName) => {
		queryClient.setQueryData<Task.IMeta[]>(getQueryKey(), (state = []) => {
			if (state === undefined) return [meta];
			const isCached = state.find((s) => s.name === name);
			if (!isCached) {
				return [...state, meta];
			}
			return state.map((s) => {
				if (s.name !== name) return s;
				return meta;
			});
		});
	};
}
