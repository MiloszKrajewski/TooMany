import { useQueryClient } from 'react-query';

import type * as Task from 'types/task';

import { getQueryKey } from './helpers';
import { useCache } from './useMetaByName';

export default function () {
	const metaByNameCache = useCache();
	const queryClient = useQueryClient();
	return (name: string, meta: Task.IMeta) => {
		metaByNameCache(name, meta);

		queryClient.setQueryData<Task.IMeta[]>(getQueryKey(), (state) => {
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
