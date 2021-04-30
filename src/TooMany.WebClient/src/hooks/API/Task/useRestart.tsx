import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import { useAllCache } from './useAll';
import { useByNameCache } from './useByName';

export default function (name?: string) {
	const setByNameCache = useByNameCache();
	const setAllCache = useAllCache(name);
	const api = useApi();
	return useMutation<Task.IMeta>(
		['task', name, 'restart'],
		() => api.task.restart(name as string),
		{
			onSuccess(result) {
				setByNameCache(result);
				setAllCache(result);
			},
		},
	);
}
