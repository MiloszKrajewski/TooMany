import { useQuery } from 'react-query';
import useApi from '../../useApi';

import type * as Task from '@tm/types/task';
import type { TaskName } from './types';
import { fetchLog } from './helpers';

export default function (taskName: TaskName) {
	const api = useApi();
	return useQuery<Task.ILog[]>(fetchLog(api.task.logs, taskName));
}
