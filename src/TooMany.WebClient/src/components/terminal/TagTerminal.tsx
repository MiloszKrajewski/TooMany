import { Task } from '@hooks/API';
import { useMemo } from 'react';
import Logs from './Logs';

function useTagLogs(name: string) {
	const { data: tasks = [], isLoading: isLoadingTasks } = Task.useAll();

	const taskNames = useMemo(() => {
		const result: string[] = [];
		for (const task of tasks) {
			if (task.tags.includes(name)) {
				result.push(task.name);
			}
		}
		return result;
	}, [name, tasks]);
	const multipleLogs = Task.Log.useLogs(taskNames);
	const isLogsLoading = multipleLogs.some((log) => log.isLoading);

	const logs = useMemo(() => {
		if (isLogsLoading) return [];
		return multipleLogs
			.flatMap((log) => log.data || [])
			.sort((a, b) => a.time - b.time);
	}, [isLogsLoading, multipleLogs]);

	return { data: logs, isLoading: isLoadingTasks || isLogsLoading };
}

export default function ({ name }: { name: string }) {
	const { data: logs, isLoading } = useTagLogs(name);

	if (isLoading) return null;
	return <Logs isTaskNameVisible logs={logs} />;
}
