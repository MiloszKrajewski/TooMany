import { useMemo } from 'react';
import { Task } from '@hooks/API';
import Terminal from '@components/terminal';
import { useParams } from 'react-router-dom';

function useTagLogs(tag: string) {
	const { data: metas = [], isLoading: isLoadingMetas } = Task.meta.useMeta();

	const taskNames = useMemo(() => {
		const result: string[] = [];
		for (const meta of metas) {
			if (meta.tags.includes(tag)) {
				result.push(meta.name);
			}
		}
		return result;
	}, [tag, metas]);

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

export default function () {
	const { name: tag } = useParams();
	const { data: logs, isLoading } = useTagLogs(tag);

	if (isLoading) return null;
	return <Terminal isTaskNameVisible logs={logs} />;
}
