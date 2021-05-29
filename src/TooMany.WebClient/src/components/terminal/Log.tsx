import { memo } from 'react';
import type { ReactNode } from 'react';

import type * as Task from 'types/task';

import * as routes from '@tm/helpers/routes';

import Link from '@components/link';

function Cell({
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

const MemoCell = memo(Cell);

export default memo(function ({
	isEven = false,
	task,
	timestamp,
	text,
	isTaskNameVisible = false,
}: {
	isEven: boolean;
	channel: Task.ILog['channel'];
	task: Task.ILog['task'];
	timestamp: Task.ILog['timestamp'];
	text: Task.ILog['text'];
	isTaskNameVisible?: boolean;
}) {
	return (
		<MemoCell isEven={isEven}>
			{isTaskNameVisible && (
				<Link
					className="text-purple-500"
					to={routes.monitor({
						type: 'task',
						name: task,
					})}
				>
					{task}
				</Link>
			)}
			{timestamp}
			{text}
		</MemoCell>
	);
});
