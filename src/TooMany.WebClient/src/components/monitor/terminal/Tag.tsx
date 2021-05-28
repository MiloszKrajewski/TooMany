import { useMemo } from 'react';
import { useParams } from 'react-router-dom';

import Terminal from '@components/terminal';

import { useLogs } from '@hooks/API/Task/log';
import { useMeta } from '@hooks/API/Task/meta';

function useTagLogs(tag: string) {
	const { data: metas = [], isLoading: isLoadingMetas } = useMeta();

	const taskNames = useMemo(() => {
		const result: string[] = [];
		for (const meta of metas) {
			if (meta.tags.includes(tag)) {
				result.push(meta.name);
			}
		}
		return result;
	}, [tag, metas]);

	const multipleLogs = useLogs(taskNames);
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
