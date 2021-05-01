import { memo, useMemo, useEffect, useRef } from 'react';
import type { ReactNode } from 'react';
import { Link, useParams } from 'react-router-dom';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import ScrollToBottom from '@components/helpers/ScrollToBottom';
import { TaskTerminal, TagTerminal } from '@components/terminal';
import { Task } from '@hooks/API';
import { useRoutes } from '@hooks/Navigation';
import { getCulture } from '@tm/helpers/culture';

const culture = getCulture();

function Title({ children }: { children: ReactNode }) {
	return <h3 className="font-bold">{children}</h3>;
}

function TagHeader({ name }: { name: string }) {
	const { data: metas = [], isLoading: isLoadingMetas } = Task.meta.useMeta(
		false,
	);

	const { isStarted, names } = useMemo(() => {
		let isStarted = false;
		if (isLoadingMetas) return { isStarted, names: [] };
		const names = metas
			.filter((meta) => meta.tags.includes(name))
			.map((meta) => {
				if (meta.actual_state === 'Started') {
					isStarted = true;
				}
				return meta.name;
			});

		return { isStarted, names };
	}, [isLoadingMetas, metas]);

	return (
		<>
			<header>
				<Title>{name}</Title>
				{isStarted ? 'Started' : 'Stopped'}
			</header>
			{names.map((name) => (
				<TaskHeader key={name} name={name} />
			))}
		</>
	);
}
function TaskHeader({ name }: { name: string }) {
	const { data: meta, isLoading: isLoadingMeta } = Task.meta.useMetaByName(
		name,
		false,
	);

	const {
		actual_state: actualState = 'Error',
		expected_state: expectedState = 'Error',
		executable,
		directory,
		arguments: args,
		started_time,
	} = meta || {};

	const startedTime = useRef('');

	useEffect(() => {
		if (typeof started_time === 'undefined') return;
		const value = new Date(started_time).toLocaleString(culture, {
			hour12: false,
		});
		startedTime.current = value;
	}, [started_time]);

	const routes = useRoutes();
	const { mutateAsync: start } = Task.useStart(name);
	const { mutateAsync: stop } = Task.useStop(name);
	const { mutateAsync: restart } = Task.useRestart(name);
	const onStart = () => start();
	const onStop = () => stop();
	const onRestart = () => restart();

	return (
		<header className="flex">
			<div>
				<Title>{name}</Title>
				<h5 className="inline-block">
					<Link to={routes.redefine({ name })}>edit</Link>
				</h5>
				{isLoadingMeta ? null : (
					<span className="mx-2">
						<button className="mx-1" onClick={onStart}>
							start
						</button>
						<button className="mx-1" onClick={onStop}>
							stop
						</button>
						<button className="mx-1" onClick={onRestart}>
							restart
						</button>
					</span>
				)}
			</div>
			<ul className="inline-flex mx-2">
				<li className="mx-2">
					<dt>Expected State:</dt>
					<pre>{isLoadingMeta ? 'Loading' : expectedState}</pre>
				</li>
				<li className="mx-2">
					<div>Actual State:</div>
					<pre>{isLoadingMeta ? 'Loading' : actualState}</pre>
				</li>
				<li className="mx-2">
					<dt>Executable:</dt>
					<pre>{isLoadingMeta ? 'Loading' : executable}</pre>
				</li>
				<li className="mx-2">
					<dt>Directory:</dt>
					<pre>{isLoadingMeta ? 'Loading' : directory}</pre>
				</li>
				<li className="mx-2">
					<dt>Arguments:</dt>
					<pre>{isLoadingMeta ? 'Loading' : args}</pre>
				</li>
				<li className="mx-2">
					<dt>Last Start Time:</dt>
					<pre>{isLoadingMeta ? 'Loading' : startedTime.current}</pre>
				</li>
			</ul>
		</header>
	);
}

function Header() {
	const { type, name } = useParams();
	switch (type) {
		case 'task':
			return <TaskHeader name={name} />;
		case 'tag':
			return <TagHeader name={name} />;
		default:
			return null;
	}
}

function MonitorPage() {
	const { name, type } = useParams();
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

export default memo(MonitorPage);
