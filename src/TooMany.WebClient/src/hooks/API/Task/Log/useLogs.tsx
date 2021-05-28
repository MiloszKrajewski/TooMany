import type { UseQueryResult } from 'react-query';
import { useQueries } from 'react-query';

import type * as Task from 'types/task';

import useApi from '../../useApi';

import { fetchLog } from './helpers';
import type { TaskName } from './types';

export default function (taskNames: TaskName[] = []) {
	const api = useApi();
	return useQueries(
		taskNames.map((taskName) => fetchLog(api.task.logs, taskName)),
	) as UseQueryResult<Task.ILog[], unknown>[];
}
