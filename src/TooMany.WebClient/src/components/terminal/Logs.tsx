import { ReactNode } from 'react';
import type * as Task from '@tm/types/task';
import Row from './Row';

function Header({
	children,
	className,
}: {
	children: ReactNode;
	className?: string;
}) {
	return (
		<div
			className={`bg-gray-200 text-gray-900 font-bold sticky top-0 ${className}`}
		>
			{children}
		</div>
	);
}

const maxBuffer = 50; // TODO: move to user settings
const spliceIndex = 0 - maxBuffer;

export default function ({
	logs,
	isTaskNameVisible = false,
}: {
	logs: Task.ILog[];
	isTaskNameVisible?: boolean;
}) {
	let channelClassName = 'col-start-1 col-end-2';
	let taskClassName = '';
	let timestampClassName = 'col-start-2 col-end-4';
	let textClassName = 'col-start-4 col-end-13 break-words';
	if (isTaskNameVisible) {
		channelClassName = 'col-start-1 col-end-2';
		taskClassName = 'col-start-2 col-end-4';
		timestampClassName = 'col-start-4 col-end-6';
		textClassName = 'col-start-6 col-end-13 break-words';
	}

	return (
		<div className="grid grid-cols-12 divide-y divide-gray-600">
			<span tabIndex={0} />
			<Header className={channelClassName}>Channel</Header>
			{isTaskNameVisible && <Header className={taskClassName}>Task</Header>}
			<Header className={timestampClassName}>Timestamp</Header>
			<Header className={textClassName}>Text</Header>
			{logs.slice(spliceIndex).map((log, index) => (
				<Row
					key={`${log.task}/${index}`}
					isEven={index % 2 === 0}
					channel={log.channel}
					task={log.task}
					timestamp={log.formattedTimestamp}
					text={log.text}
					isTaskNameVisible={isTaskNameVisible}
					channelClassName={channelClassName}
					taskClassName={taskClassName}
					timestampClassName={timestampClassName}
					textClassName={textClassName}
				/>
			))}
		</div>
	);
}
