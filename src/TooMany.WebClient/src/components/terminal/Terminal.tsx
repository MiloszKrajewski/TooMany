import { Fragment, ReactNode } from 'react';
import * as Task from '@hooks/API/Task';

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

const channelClassName = 'col-start-1 col-end-2';
const timestampClassName = 'col-start-2 col-end-5';
const textClassName = 'col-start-5 col-end-13';
export default function ({ name }: { name: string }) {
	const { data: logs = [], isLoading } = Task.useLogs(name);
	console.log(logs, isLoading);
	if (isLoading) return null;
	return (
		<div className="grid grid-cols-12">
			<Header className={channelClassName}>Channel</Header>
			<Header className={timestampClassName}>Timestamp</Header>
			<Header className={textClassName}>Text</Header>
			{logs.map((log, index) => (
				<Fragment key={`${name}/${index}`}>
					<Item index={index} className={channelClassName}>
						{log.channel}
					</Item>
					<Item index={index} className={timestampClassName}>
						{log.timestamp}
					</Item>
					<Item index={index} className={textClassName}>
						{log.text}
					</Item>
				</Fragment>
			))}
		</div>
	);
}
