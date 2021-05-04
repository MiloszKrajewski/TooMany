import { useMemo } from 'react';
import type { MouseEventHandler } from 'react';
import { Task } from '@hooks/API';
import Title from './title';
import TaskHeader from './Task';
import { useParams } from 'react-router-dom';
import { useToggle } from '@tm/hooks';

export default function () {
	const { name: tag } = useParams();
	const [isOpen, toggleOpen] = useToggle();
	const { data: metas = [], isLoading: isLoadingMetas } = Task.meta.useMeta(
		false,
	);

	const toggleIsOpen: MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		toggleOpen();
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
