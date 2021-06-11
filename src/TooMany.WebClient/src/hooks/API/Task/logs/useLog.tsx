import { useQuery } from 'react-query';

import type * as Task from 'types/task';

import useApi from '../../useApi';

import { fetchLog } from './helpers';
import type { TaskName } from './types';

export default function (taskName: TaskName) {
	const api = useApi();
	return useQuery<Task.ILog[]>(fetchLog(api.task.logs, taskName));
}
