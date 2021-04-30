import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import { useAllCache } from './useAll';
import { useByNameCache } from './useByName';

export default function (name?: string) {
	const api = useApi();
	const setByNameCache = useByNameCache();
	const setAllCache = useAllCache(name);
	return useMutation<Task.IMeta>(
		['task', name, 'stop'],
		() => api.task.stop(name as string),
		{
			onSuccess(result) {
				setByNameCache(result);
				setAllCache(result);
			},
		},
	);
}
