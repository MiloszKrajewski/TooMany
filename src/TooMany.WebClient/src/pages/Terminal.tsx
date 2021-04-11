import { memo, ReactNode } from 'react';
import { Link, useParams } from 'react-router-dom';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import ScrollToBottom from '@components/helpers/ScrollToBottom';
import { TaskTerminal, TagTerminal } from '@components/terminal';
import SignalR from '@tm/SignalR';
import { useEffect } from 'react';

function Title({ children }: { children: ReactNode }) {
	return <h3 className="font-bold">{children}</h3>;
}

function TagHeader() {
	const { name } = useParams();
	return (
		<header>
			<Title>{name}</Title>
		</header>
	);
}
function TaskHeader() {
	const { name } = useParams();
	return (
		<header>
			<Title>{name}</Title>
			<h5>
				<Link to={`/editor/${name}`}>edit</Link>
			</h5>
		</header>
	);
}

function Header() {
	const { type } = useParams();
	switch (type) {
		case 'task':
			return <TaskHeader />;
		case 'tag':
			return <TagHeader />;
		default:
			return null;
	}
}

function TerminalPage() {
	const { name, type } = useParams();
	useEffect(() => {
		// TODO: set task monitor specific instances of SignalR
		// TODO: move into Task/Tag terminals
		SignalR.start();
		return () => {
			SignalR.stop();
		};
	}, []);
	return (
		<section className="max-h-screen flex flex-col">
			<Header />
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}>
				<ScrollToBottom>
					{type === 'task' && <TaskTerminal name={name} />}
					{type === 'tag' && <TagTerminal name={name} />}
				</ScrollToBottom>
			</SuspenseQuery>
		</section>
	);
}

export default memo(TerminalPage);
