import 'xterm/css/xterm.css';
import { useEffect, useRef } from 'react';
import { Terminal } from 'xterm';
import { FitAddon } from 'xterm-addon-fit';
import { WebglAddon } from 'xterm-addon-webgl';

import type * as Task from 'types/task';

export default function ({
	logs,
	instance,
}: {
	logs: Task.ILog[];
	instance: Terminal;
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

		instance.write(
			logs.map((log) => `\r\n${log.timestamp} - ${log.text}`).join(),
		);
		const resizeEvent = () => fitAddon.fit();
		resizeEvent();
		window.addEventListener('resize', resizeEvent);
		return function () {
			window.removeEventListener('resize', resizeEvent);
		};
	}, []);

	return <div style={{ flex: '1 100%' }} ref={containerRef} />;
}

// export default (function ({
// 	logs,
// 	isTaskNameVisible = false,
// }: {
// 	logs: Task.ILog[];
// 	isTaskNameVisible?: boolean;
// }) {
// 	return (
// 		<div className="divide-y divide-gray-600">
// 			{logs.map((log, index) => (
// 				<Log
// 					key={`${log.task}/${index}`}
// 					isEven={index % 2 === 0}
// 					task={log.task}
// 					timestamp={log.formattedTimestamp}
// 					text={log.text}
// 					isTaskNameVisible={isTaskNameVisible}
// 				/>
// 			))}
// 		</div>
// 	);
// });
// const Cell = ({ columnIndex, rowIndex, style }) => (
// 	<div style={style}>
// 		r{rowIndex}, c{columnIndex}
// 	</div>
// );

// export default function ({
// 	logs,
// 	isTaskNameVisible = false,
// }: {
// 	logs: Task.ILog[];
// 	isTaskNameVisible?: boolean;
// }) {
// 	return (
// 		<Grid
// 			className="Grid"
// 			columnCount={isTaskNameVisible ? 4 : 3}
// 			columnWidth={() => 200}
// 			height={1000}
// 			rowCount={logs.length}
// 			rowHeight={() => 50}
// 			width={isTaskNameVisible ? 800 : 600}
// 		>
// 			{Cell}
// 		</Grid>
// 	);
// }
