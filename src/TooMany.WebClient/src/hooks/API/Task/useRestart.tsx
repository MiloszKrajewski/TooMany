import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import * as meta from './meta';

export default function (name: string) {
	const setMetasCache = meta.useCache();
	const api = useApi();
	return useMutation<Task.IMeta, unknown, string>(
		['task', 'restart', name],
		(taskName) => api.task.restart(taskName),
		{
			onSuccess(result, taskName) {
				setMetasCache(result, taskName);
			},
		},
	);
}
