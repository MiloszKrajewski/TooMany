import { FormEventHandler, MouseEventHandler, useMemo } from 'react';
import { useState, useCallback } from 'react';
import { Task } from '@hooks/API';
import EnvironmentVariables from './EnvironmentVariables';
import Tags from './Tags';
import { useParams } from 'react-router-dom';

const increment = (x: number) => x + 1;
const decrement = (x: number) => {
	if (x < 1) return 0;
	return x - 1;
};

export default function () {
	const { name } = useParams();
	const { data: metas = [], isLoading } = Task.meta.useMeta();

	const data = metas.find((m) => m.name === name);

	const {
		executable = '',
		arguments: args = '',
		directory = '',
		environment: environmentVariables = {},
		tags = [],
	} = data || {};

	const [taskName, setTaskName] = useState(name);
	const onTaskNameChange = useCallback((event) => {
		const { value = '' } = event.target;
		setTaskName(value);
	}, []);

	const isEdit = useMemo(() => {
		if (isLoading || name === '') return false;
		return taskName === name;
	}, [isLoading, taskName, name]);

	const isDisabled = useMemo(() => {
		if (isLoading || taskName === '') return true;
		const allTaskNames = metas.map((t) => t.name.toLowerCase());
		return allTaskNames.includes(taskName.toLowerCase());
	}, [isLoading, taskName, metas]);

	const [environmentVariableCount, setEnvironmentVariableCount] = useState(
		Object.keys(environmentVariables).length,
	);
	const incrementEnvironmentVariableCount: MouseEventHandler<HTMLButtonElement> = (
		event,
	) => {
		event.preventDefault();
		setEnvironmentVariableCount(increment);
	};
	const decrementEnvironmentVariableCount: MouseEventHandler<HTMLButtonElement> = (
		event,
	) => {
		event.preventDefault();
		setEnvironmentVariableCount(decrement);
	};

	const [tagCount, setTagCount] = useState(tags.length);
	const incrementTagCount: MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		setTagCount(increment);
	};
	const decrementTagCount: MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		setTagCount(decrement);
	};

	const { mutateAsync: createTask } = Task.useCreate(name);
	const handleSubmit = useCallback<FormEventHandler<HTMLFormElement>>(
		(event) => {
			event.preventDefault();

			const target = event.target as HTMLFormElement;

			const environmentVariableInputs: HTMLInputElement[] =
				target.environmentVariables?.elements || [];
			let inputCount = 0;
			const environment: Record<string, string> = {};
			for (let index = 0; index < environmentVariableCount; index++) {
				const key = environmentVariableInputs[inputCount++].value;
				const value = environmentVariableInputs[inputCount++].value;
				if (!key || !value) continue;
				environment[key] = value;
			}

			const tagInputs: HTMLInputElement[] = target.tags?.elements || [];
			const tags: string[] = Array(tagCount)
				.fill(null)
				.map((_, index) => tagInputs[index].value)
				.filter(Boolean);

			createTask({
				name: target.taskName.value,
				executable: target.executable.value,
				arguments: target.arguments.value,
				directory: target.directory.value,
				environment,
				tags,
			});
		},
		[environmentVariableCount, tagCount, createTask],
	);

	if (isLoading) {
		return <h5>Loading...</h5>;
	}

	return (
		<form onSubmit={handleSubmit}>
			<label htmlFor="taskName">
				name:
				<input
					className="text-gray-900"
					name="taskName"
					type="text"
					value={taskName}
					onChange={onTaskNameChange}
				/>
			</label>
			<br />
			<br />
			<label htmlFor="executable">
				executable:
				<input
					className="text-gray-900"
					name="executable"
					type="text"
					defaultValue={executable}
				/>
			</label>
			<br />
			<br />
			<label htmlFor="arguments">
				arguments:
				<input
					className="text-gray-900"
					name="arguments"
					type="text"
					defaultValue={args}
				/>
			</label>
			<br />
			<br />
			<label htmlFor="directory">
				directory:
				<input
					className="text-gray-900"
					name="directory"
					type="text"
					defaultValue={directory}
				/>
			</label>
			<br />
			<br />
			Environment Variables:
			<EnvironmentVariables
				count={environmentVariableCount}
				environmentVariables={environmentVariables}
			/>
			<br />
			<button
				className="bg-green-400 w-10"
				onClick={decrementEnvironmentVariableCount}
			>
				-
			</button>
			<button
				className="bg-green-400 w-10"
				onClick={incrementEnvironmentVariableCount}
			>
				+
			</button>
			<br />
			<br />
			Tags:
			<Tags count={tagCount} tags={tags} />
			<br />
			<button className="bg-green-400 w-10" onClick={decrementTagCount}>
				-
			</button>
			<button className="bg-green-400 w-10" onClick={incrementTagCount}>
				+
			</button>
			<br />
			<br />
			<input
				className="bg-green-400 active:bg-green-500 hover:bg-green-600 disabled:opacity-50 disabled:pointer-events-none w-min-content w-20 0 cursor-pointer"
				disabled={!isEdit && isDisabled}
				type="submit"
				value={isEdit ? 'Save' : 'Create'}
			/>
		</form>
	);
}
