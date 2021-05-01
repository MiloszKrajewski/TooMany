import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import * as meta from './meta';

export default function (name?: string) {
	const api = useApi();
	const setMetasCache = meta.useCache(name);
	return useMutation<Task.IMeta>(
		['task', name, 'stop'],
		() => api.task.stop(name as string),
		{
			onSuccess(result) {
				setMetasCache(result);
			},
		},
	);
}
