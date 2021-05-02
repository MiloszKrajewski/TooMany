import { Task } from '@hooks/API';
import { useMemo } from 'react';
import Logs from './Logs';

function useTagLogs(name: string) {
	const { data: metas = [], isLoading: isLoadingMetas } = Task.meta.useMeta();

	const taskNames = useMemo(() => {
		const result: string[] = [];
		for (const meta of metas) {
			if (meta.tags.includes(name)) {
				result.push(meta.name);
			}
		}
		return result;
	}, [name, metas]);

	const multipleLogs = Task.log.useLogs(taskNames);
	const isLogsLoading = multipleLogs.some((log) => log.isLoading);

	const logs = useMemo(() => {
		if (isLogsLoading) return [];
		return multipleLogs
			.flatMap((log) => log.data || [])
			.sort((a, b) => a.time - b.time);
	}, [isLogsLoading, multipleLogs]);

	return { data: logs, isLoading: isLoadingMetas || isLogsLoading };
}

export default function ({ name }: { name: string }) {
	const { data: logs, isLoading } = useTagLogs(name);

	if (isLoading) return null;
	return <Logs isTaskNameVisible logs={logs} />;
}
