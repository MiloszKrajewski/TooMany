import { useEffect, useRef, useMemo } from 'react';
import { useParams } from 'react-router-dom';
import { Terminal as XTerm } from 'xterm';

import type * as Realtime from 'types/realtime';

import SignalR from '@tm/SignalR';

import Terminal from '@components/terminal';

import { useLogsByTag } from '@hooks/API/Task/log';
import { useMeta } from '@hooks/API/Task/meta';

export default function () {
	const { name: tag } = useParams();
	const { data: metas = [] } = useMeta();
	const taskNames = useMemo(() => {
		const result: string[] = [];
		for (const meta of metas) {
			if (meta.tags.includes(tag)) {
				result.push(meta.name);
			}
		}
		return result;
	}, [tag, metas]);
	const { data: logs, isLoading } = useLogsByTag(taskNames, tag);

	const xterm = useRef(new XTerm({ disableStdin: true }));

	useEffect(() => {
		const term = xterm.current;
		if (!term) return;

		const fns: Record<string, Realtime.onLogFn> = {};
		for (const name of taskNames) {
			fns[name] = SignalR.onTaskLog(name, (_, log) => {
				term.writeln(`${log.timestamp} - ${log.text}`);
			});
		}
		return () => {
			for (const name of taskNames) {
				if (typeof fns[name] === 'function') {
					SignalR.offTaskLog(fns[name]);
				}
			}
		};
	}, []);

	if (isLoading || typeof logs === 'undefined') return null;
	return <Terminal instance={xterm.current} logs={logs} />;
}
