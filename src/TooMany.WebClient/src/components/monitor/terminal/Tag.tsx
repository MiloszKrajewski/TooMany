import 'xterm/css/xterm.css';
import { useEffect, useMemo, useRef } from 'react';

import type * as Realtime from 'types/realtime';

import SignalR from '@tm/SignalR';

import { useLogsByTag } from '@hooks/API/Task/log';
import { useMeta } from '@hooks/API/Task/meta';
import useTerminal from '@hooks/useTerminal';

export default function ({ name: tag }: { name: string }) {
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
	const { data: logs = [], isLoading } = useLogsByTag(taskNames, tag);
	const container = useRef<HTMLDivElement>(null);

	const id = `tag/${tag}`;
	const xterm = useTerminal(id, container.current);

	useEffect(() => {
		if (typeof xterm === 'undefined') {
			return;
		}

		const fns: Record<string, Realtime.onLogFn> = {};
		for (const name of taskNames) {
			fns[name] = SignalR.onTaskLog(name, (_, log) => {
				xterm.writeln(`${log.timestamp} - ${log.text}`);
			});
		}
		return () => {
			for (const name of taskNames) {
				if (typeof fns[name] === 'function') {
					SignalR.offTaskLog(fns[name]);
				}
			}
		};
	}, [xterm, id]);

	useEffect(() => {
		if (typeof xterm === 'undefined') {
			return;
		}
		console.log(logs);

		const pastLogs = logs
			.map((log) => `${log.timestamp} - ${log.text}\r\n`)
			.join();
		xterm.write(pastLogs);
	}, [xterm, id, logs]);

	if (isLoading || typeof logs === 'undefined') return null;
	return <div style={{ flex: '1 100%' }} ref={container} />;
}
