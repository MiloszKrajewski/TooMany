import 'xterm/css/xterm.css';
import { useEffect, useRef } from 'react';

import SignalR from '@tm/SignalR';

import { useLog } from '@hooks/API/Task/log';
import useTerminal from '@hooks/useTerminal';

export default function ({ name }: { name: string }) {
	const { data: logs = [], isLoading } = useLog(name);
	const container = useRef<HTMLDivElement>(null);

	const id = `task/${name}`;
	const xterm = useTerminal(id, container.current, logs);

	useEffect(() => {
		if (typeof xterm === 'undefined') {
			return;
		}

		const fn = SignalR.onTaskLog(name, (_, log) => {
			if (!log.text) return;
			xterm.writeln(`${log.timestamp} - ${log.text}`);
		});
		return () => {
			if (typeof fn === 'function') {
				SignalR.offTaskLog(fn);
			}
		};
	}, [xterm, id]);

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
