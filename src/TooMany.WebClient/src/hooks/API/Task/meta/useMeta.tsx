import { useQuery, useQueryClient } from 'react-query';

import type * as Task from 'types/task';

import useApi from '../../useApi';

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
			const index = state.findIndex((s) => s.name === name);
			const isCached = index >= 0;
			if (!isCached) {
				state.splice(state.length - 1, 1, meta);
			} else {
				state.splice(index, 1, meta);
			}
			return state;
		});
	};
}
