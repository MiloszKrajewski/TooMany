import { useEffect, useRef } from 'react';
import { Link } from 'react-router-dom';
import { Task } from '@hooks/API';
import * as routes from '@tm/helpers/routes';
import Title from './title';
import { getCulture } from '@tm/helpers/culture';
import { useParams } from 'react-router-dom';

const culture = getCulture();

export default function () {
	const { name } = useParams();
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
