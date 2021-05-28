import { useMutation } from 'react-query';

import type * as Task from 'types/task';

import useApi from '../useApi';

import * as meta from './meta';

export default function (name: string) {
	const api = useApi();
	const setMetasCache = meta.useCache();
	return useMutation<Task.IMeta, unknown, string>(
		['task', 'stop', name],
		(taskName) => api.task.stop(taskName),
		{
			onSuccess(result, taskName) {
				setMetasCache(result, taskName);
			},
		},
	);
}
