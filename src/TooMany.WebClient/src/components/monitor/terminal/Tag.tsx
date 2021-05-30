import 'xterm/css/xterm.css';
import { useEffect, useMemo, useRef } from 'react';

import type * as Realtime from 'types/realtime';

import SignalR from '@tm/SignalR';

import { useLogsByTag } from '@hooks/API/Task/log';
import { useMeta } from '@hooks/API/Task/meta';
import useTerminal from '@hooks/useTerminal';

export default function ({ name }: { name: string }) {
	const { data: metas = [] } = useMeta();
	const taskNames = useMemo(() => {
		const result: string[] = [];
		for (const meta of metas) {
			if (meta.tags.includes(name)) {
				result.push(meta.name);
			}
		}
		return result;
	}, [name, metas]);
	const { data: logs = [], isLoading } = useLogsByTag(taskNames, name);
	const container = useRef<HTMLDivElement>(null);

	const id = `tag/${name}`;
	const xterm = useTerminal(id, container.current, logs);

	useEffect(() => {
		if (typeof xterm === 'undefined') {
			return;
		}

		const fns: Record<string, Realtime.onLogFn> = {};
		for (const name of taskNames) {
			fns[name] = SignalR.onTaskLog(name, (_, log) => {
				if (!log.text) return;
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

	if (isLoading || typeof logs === 'undefined') return null;
	return <div style={{ flex: '1 100%' }} ref={container} />;
}
