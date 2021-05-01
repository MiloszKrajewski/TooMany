import { ReactNode } from 'react';
import type * as Task from '@tm/types/task';
import Link from '@components/link';
import { useRoutes } from '@hooks/Navigation';

function Item({
	children,
	className,
	isEven = false,
}: {
	children: ReactNode;
	className?: string;
	isEven: boolean;
}) {
	let bg = '';
	if (isEven) {
		bg = 'bg-gray-800';
	}
	return <div className={`${bg} ${className}`}>{children}</div>;
}

export default function ({
	isEven = false,
	channel,
	task,
	timestamp,
	text,
	isTaskNameVisible = false,
	channelClassName,
	taskClassName,
	timestampClassName,
	textClassName,
}: {
	isEven: boolean;
	channel: Task.ILog['channel'];
	task: Task.ILog['task'];
	timestamp: Task.ILog['timestamp'];
	text: Task.ILog['text'];
	isTaskNameVisible?: boolean;
	channelClassName: string;
	taskClassName: string;
	timestampClassName: string;
	textClassName: string;
}) {
	const routes = useRoutes();
	return (
		<>
			<Item isEven={isEven} className={channelClassName}>
				{channel}
			</Item>
			{isTaskNameVisible && (
				<Item isEven={isEven} className={taskClassName}>
					<Link
						className="text-purple-500"
						to={routes.monitor({
							type: 'task',
							name: task,
						})}
					>
						{task}
					</Link>
				</Item>
			)}
			<Item isEven={isEven} className={timestampClassName}>
				{timestamp}
			</Item>
			<Item isEven={isEven} className={textClassName}>
				{text}
			</Item>
		</>
	);
}