import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import * as meta from './meta';

export default function (name?: string) {
	const setMetasCache = meta.useCache(name);
	const api = useApi();
	return useMutation<Task.IMeta>(
		['task', name, 'restart'],
		() => api.task.restart(name as string),
		{
			onSuccess(result) {
				setMetasCache(result);
			},
		},
	);
}
