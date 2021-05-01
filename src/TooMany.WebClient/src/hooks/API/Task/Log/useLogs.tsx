import type { UseQueryResult } from 'react-query';
import { useQueries } from 'react-query';
import useApi from '../../useApi';
import type * as Task from '@tm/types/task';

import type { TaskName } from './types';
import { fetchLog } from './helpers';

export default function (taskNames: TaskName[] = []) {
	const api = useApi();
	return useQueries(
		taskNames.map((taskName) => fetchLog(api.task.logs, taskName)),
	) as UseQueryResult<Task.ILog[], unknown>[];
}