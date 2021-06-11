import 'xterm/css/xterm.css';
import { useEffect, useRef, useMemo } from 'react';

import SignalR from '@tm/SignalR';

import { useLog } from '@hooks/API/Task/logs';
import useTerminal from '@hooks/useTerminal';

import { formatLine } from './helpers';

export default function ({ name }: { name: string }) {
	const { data: logs = [], isLoading } = useLog(name);
	const container = useRef<HTMLDivElement>(null);

	const id = `task/${name}`;

	const initialLogs = useMemo(() => {
		return logs
			.map((log) => formatLine(log.text, log.timestamp))
			.join('\r\n');
	}, [id, logs]);

	const xterm = useTerminal(id, container.current, initialLogs);

	useEffect(() => {
		if (typeof xterm === 'undefined') {
			return;
		}

		const fn = SignalR.onTaskLog(name, (_, log) => {
			if (!log.text) return;
			xterm.writeln(formatLine(log.text, log.timestamp));
		});
		return () => {
			if (typeof fn === 'function') {
				SignalR.offTaskLog(fn);
			}
		};
	}, [xterm, id]);

	if (isLoading) return null;
	return <div style={{ flex: '1 100%' }} ref={container} />;
}
