import { useMutation } from 'react-query';
import type * as Task from '@tm/types/task';
import useApi from '../useApi';
import * as meta from './meta';

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
	const setMetasCache = meta.useCache(name);
	return useMutation<Task.IMeta, unknown, ITask>(
		['task', name],
		(payload) => api.task.create<ITask>(name, payload),
		{
			onSuccess(result) {
				setMetasCache(result);
			},
		},
	);
}
