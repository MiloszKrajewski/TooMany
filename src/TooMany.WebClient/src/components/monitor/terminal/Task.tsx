import 'xterm/css/xterm.css';
import { useEffect, useRef } from 'react';

import SignalR from '@tm/SignalR';

import { useLog } from '@hooks/API/Task/log';
import useTerminal from '@hooks/useTerminal';

export default function ({ name: task }: { name: string }) {
	const { data: logs = [], isLoading } = useLog(task);
	// const initialLogs = useRef(logs || []);
	const container = useRef<HTMLDivElement>(null);

	const id = `task/${task}`;
	const xterm = useTerminal(id, container.current);

	useEffect(() => {
		if (typeof xterm === 'undefined' || isLoading) {
			console.error('no term');
			return;
		}

		const fn = SignalR.onTaskLog(task, (_, log) => {
			xterm.writeln(`${log.timestamp} - ${log.text}`);
		});
		return () => {
			if (typeof fn === 'function') {
				SignalR.offTaskLog(fn);
			}
		};
	}, [xterm, id]);

	useEffect(() => {
		if (typeof xterm === 'undefined') return;

		const pastLogs = logs
			.map((log) => `${log.timestamp} - ${log.text}\r\n`)
			.join();
		xterm.write(pastLogs);
	}, [xterm, id, logs]);

	if (isLoading) return null;
	return (
		<div
			id={id}
			style={{
				flex: '1 100%',
			}}
			ref={container}
		/>
	);
}
