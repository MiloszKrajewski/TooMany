import { useQueryClient } from 'react-query';
import type * as Task from '@tm/types/task';

import { getQueryKey } from './helpers';

export default function () {
	const queryClient = useQueryClient();
	return (name: string, meta: Task.IMeta) => {
		console.log(name, meta);
		queryClient.setQueryData<Task.IMeta[]>(getQueryKey(), (state) => {
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
