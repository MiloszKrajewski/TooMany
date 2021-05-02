import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import * as meta from './meta';

export default function (name: string) {
	const api = useApi();
	const setMetasCache = meta.useCache();
	return useMutation<Task.IMeta, unknown, string>(
		['task', 'start', name],
		(taskName) => api.task.start(taskName),
		{
			onSuccess(result, taskName) {
				setMetasCache(result, taskName);
			},
		},
	);
}
