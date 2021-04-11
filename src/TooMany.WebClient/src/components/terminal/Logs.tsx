import { Fragment, ReactNode } from 'react';
import type * as Task from '@tm/types/task';
import Link from '@components/link';

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
function Item({
	children,
	className,
	index,
}: {
	children: ReactNode;
	className?: string;
	index: number;
}) {
	let bg = '';
	if (index % 2 === 0) {
		bg = 'bg-gray-800';
	}
	return <div className={`${bg} ${className}`}>{children}</div>;
}

export default function ({
	logs,
	isTaskNameVisible = false,
}: {
	logs: Task.ILog[];
	isTaskNameVisible?: boolean;
}) {
	let channelClassName = 'col-start-1 col-end-3';
	let taskClassName = '';
	let timestampClassName = 'col-start-3 col-end-5';
	let textClassName = 'col-start-5 col-end-13';
	if (isTaskNameVisible) {
		channelClassName = 'col-start-1 col-end-2';
		taskClassName = 'col-start-2 col-end-4';
		timestampClassName = 'col-start-4 col-end-6';
		textClassName = 'col-start-6 col-end-13';
	}
	return (
		<div className="grid grid-cols-12">
			<Header className={channelClassName}>Channel</Header>
			{isTaskNameVisible && <Header className={taskClassName}>Task</Header>}
			<Header className={timestampClassName}>Timestamp</Header>
			<Header className={textClassName}>Text</Header>
			{logs.map((log, index) => (
				<Fragment key={`${log.task}/${index}`}>
					<Item index={index} className={channelClassName}>
						{log.channel}
					</Item>
					{isTaskNameVisible && (
						<Item index={index} className={taskClassName}>
							<Link
								className="text-purple-500"
								to={`/terminal/task/${log.task}`}
							>
								{log.task}
							</Link>
						</Item>
					)}
					<Item index={index} className={timestampClassName}>
						{log.formattedTimestamp}
					</Item>
					<Item index={index} className={textClassName}>
						{log.text}
					</Item>
				</Fragment>
			))}
		</div>
	);
}
