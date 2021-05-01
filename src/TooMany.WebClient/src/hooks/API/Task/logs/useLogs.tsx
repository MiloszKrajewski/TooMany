import type { UseQueryResult } from 'react-query';
import { useQueries } from 'react-query';
import useApi from '../../useApi';
import type * as Task from '@tm/types/task';

import type { TaskName } from './types.d';
import { fetchLog } from './helpers';
import useRealtime from './useRealtime';

export default function (taskNames: TaskName[] = []) {
	useRealtime(taskNames);

	const api = useApi();
	return useQueries(
		taskNames.map((taskName) => fetchLog(api.task.logs, taskName)),
	) as UseQueryResult<Task.ILog[], unknown>[];
}
