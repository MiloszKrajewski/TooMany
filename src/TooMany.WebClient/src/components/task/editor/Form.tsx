import { FormEventHandler } from 'react';
import { memo, useState, useCallback } from 'react';
import * as Task from '@hooks/API/Task';
import EnvironmentVariables from './EnvironmentVariables';
import Tags from './Tags';

const increment = (x: number) => x + 1;
const decrement = (x: number) => {
	if (x < 1) return 0;
	return x - 1;
};

const Form = ({ name = '' }: { name?: string }) => {
	const { data, isLoading } = Task.useTask(name);
	const {
		executable = '',
		arguments: args = '',
		directory = '',
		environment: environmentVariables = {},
		tags = [],
	} = data || {};

	const [environmentVariableCount, setEnvironmentVariableCount] = useState(
		Object.keys(environmentVariables).length,
	);
	const incrementEnvironmentVariableCount = () =>
		setEnvironmentVariableCount(increment);
	const decrementEnvironmentVariableCount = () =>
		setEnvironmentVariableCount(decrement);

	const [tagCount, setTagCount] = useState(tags.length);
	const incrementTagCount = () => setTagCount(increment);
	const decrementTagCount = () => setTagCount(decrement);

	const { mutateAsync: createTask } = Task.useCreate();
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
				environment[key] = value;
			}

			const tagInputs: HTMLInputElement[] = target.tags?.elements || [];
			const tags: string[] = Array(tagCount)
				.fill(null)
				.map((_, index) => tagInputs[index].value);

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
					defaultValue={name}
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
				className="bg-green-400 w-min-content w-20"
				type="submit"
				value="Submit"
			/>
		</form>
	);
};

export default memo(Form);
