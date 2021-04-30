import { useQuery } from 'react-query';
import useApi from '../useApi';
import type * as Task from '@tm/types/task';

export default function () {
	const api = useApi();
	return useQuery<Task.IMeta[]>('tasks', () => api.task.list());
}