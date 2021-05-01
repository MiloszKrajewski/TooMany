import { memo, useMemo, useEffect, useRef, useState } from 'react';
import type { ReactNode, MouseEventHandler } from 'react';
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

function TagHeader({ name: tag }: { name: string }) {
	const [isOpen, setIsOpen] = useState(false);
	const { data: metas = [], isLoading: isLoadingMetas } = Task.meta.useMeta(
		false,
	);

	const toggleIsOpen: MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		setIsOpen((x) => !x);
	};

	const { isActuallyToStarted, isExpectedToStarted, names } = useMemo<{
		isActuallyToStarted: boolean;
		isExpectedToStarted: boolean;
		names: string[];
	}>(() => {
		let isActuallyToStarted = false;
		let isExpectedToStarted = false;
		if (isLoadingMetas)
			return {
				isActuallyToStarted,
				isExpectedToStarted,
				names: [],
			};
		const names = metas
			.filter((meta) => meta.tags.includes(tag))
			.map((meta) => {
				if (meta.actual_state === 'Started') {
					isActuallyToStarted = true;
				}
				if (meta.expected_state === 'Started') {
					isExpectedToStarted = true;
				}
				return meta.name;
			})
			.sort((a, b) => {
				if (a < b) {
					return -1;
				}
				if (a > b) {
					return 1;
				}
				return 0;
			});

		return { isActuallyToStarted, isExpectedToStarted, names };
	}, [isLoadingMetas, metas]);

	const { mutateAsync: start } = Task.useStart(tag);
	const { mutateAsync: stop } = Task.useStop(tag);
	const { mutateAsync: restart } = Task.useRestart(tag);
	const onStart = () => {
		for (const name of names) {
			start(name);
		}
	};
	const onStop = () => {
		for (const name of names) {
			stop(name);
		}
	};
	const onRestart = () => {
		for (const name of names) {
			restart(name);
		}
	};

	return (
		<>
			<header className="flex p-1">
				<div>
					<Title>{tag}</Title>
					<span className="mr-2">
						<button className="mr-1" onClick={onStart}>
							Start
						</button>
						<button className="mx-1" onClick={onStop}>
							Stop
						</button>
						<button className="mx-1" onClick={onRestart}>
							Restart
						</button>
					</span>
				</div>
				<ul className="inline-flex mx-2">
					<li className="mx-2">
						<dt>Expected State:</dt>
						<pre>{isExpectedToStarted ? 'Started' : 'Stopped'}</pre>
					</li>
					<li className="mx-2">
						<dt>Actual State:</dt>
						<pre>{isActuallyToStarted ? 'Started' : 'Stopped'}</pre>
					</li>
				</ul>
			</header>
			<button
				className={`
					border-t-2
					${isOpen ? 'border-b-2' : ''} 
					border-gray-200
					focus:outline-none
				`}
				onClick={toggleIsOpen}
			>
				{isOpen ? '▲' : '▼'}
			</button>
			<section
				className={`
					divide-y
					divide-gray-600
					${isOpen ? '' : 'hidden'}
				`}
			>
				{names.map((name) => (
					<TaskHeader key={name} name={name} />
				))}
			</section>
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
	const onStart = () => start(name);
	const onStop = () => stop(name);
	const onRestart = () => restart(name);

	return (
		<header className="flex p-1">
			<div>
				<Title>{name}</Title>
				<h5 className="inline-block">
					<Link to={routes.redefine({ name })}>edit</Link>
				</h5>
				{isLoadingMeta ? null : (
					<span className="mx-2">
						<button className="mx-1" onClick={onStart}>
							Start
						</button>
						<button className="mx-1" onClick={onStop}>
							Stop
						</button>
						<button className="mx-1" onClick={onRestart}>
							Restart
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
