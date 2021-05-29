import { useEffect, useRef } from 'react';
import { useParams } from 'react-router-dom';
import { Terminal as XTerm } from 'xterm';

import SignalR from '@tm/SignalR';

import Terminal from '@components/terminal';

import { useLog } from '@hooks/API/Task/log';

export default function () {
	const { name } = useParams();
	const { data: logs = [], isLoading } = useLog(name);

	const xterm = useRef(new XTerm({ disableStdin: true }));

	useEffect(() => {
		const term = xterm.current;
		if (!term) return;

		const fn = SignalR.onTaskLog(name, (_, log) => {
			term.writeln(`${log.timestamp} - ${log.text}`);
		});
		return () => {
			if (typeof fn === 'function') {
				SignalR.offTaskLog(fn);
			}
		};
	}, []);

	if (isLoading) return null;
	return <Terminal instance={xterm.current} logs={logs} />;
}
