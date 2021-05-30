import 'xterm/css/xterm.css';
import { useEffect, useRef } from 'react';
import { Terminal as XTerm } from 'xterm';
import { FitAddon } from 'xterm-addon-fit';
import { WebglAddon } from 'xterm-addon-webgl';

import type * as Task from 'types/task';

export default function ({
	logs,
	instance,
	id,
}: {
	logs: Task.ILog[];
	instance?: XTerm;
	id: string;
}) {
	const containerRef = useRef<HTMLDivElement>(null);

	useEffect(() => {
		const container = containerRef.current;
		if (container === null) return;

		if (!instance) return;

		instance.open(container);

		const fitAddon = new FitAddon();
		instance.loadAddon(fitAddon);
		const webglAddon = new WebglAddon();
		instance.loadAddon(webglAddon);

		const pastLogs = logs
			.map((log) => `${log.timestamp} - ${log.text}\r\n`)
			.join();
		instance.write(pastLogs);

		const resizeEvent = () => fitAddon.fit();
		resizeEvent();
		window.addEventListener('resize', resizeEvent);
		return function () {
			window.removeEventListener('resize', resizeEvent);
			instance.dispose();
		};
	}, [id]);

	return <div style={{ flex: '1 100%' }} ref={containerRef} />;
}
