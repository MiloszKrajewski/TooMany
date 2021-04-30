import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import { useAllCache } from './useAll';
import { useByNameCache } from './useByName';

interface ITask {
	name: string;
	executable?: string;
	arguments?: string;
	directory?: string;
	environment?: Record<string, string>;
	tags?: string[];
}

export default function (name: string) {
	const api = useApi();
	const setByNameCache = useByNameCache();
	const setAllCache = useAllCache(name);
	return useMutation<Task.IMeta, unknown, ITask>(
		['task', name],
		(payload) => api.task.create<ITask>(name, payload),
		{
			onSuccess(result) {
				setByNameCache(result);
				setAllCache(result);
			},
		},
	);
}
