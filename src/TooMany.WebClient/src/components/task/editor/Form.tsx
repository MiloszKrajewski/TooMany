import { FormEventHandler, useRef, Fragment } from 'react';
import { memo, useState, useCallback } from 'react';
import * as Task from '@hooks/API/Task';

const EnvironmentVars = memo(
	({ count = 0, envVars = [] }: { count?: number; envVars?: IEnvVars[] }) => {
		console.log('render env var', envVars);
		if (count === 0) return null;
		return (
			<fieldset id="envVars" className="grid grid-cols-3">
				{new Array(count).fill(null).map((_, index) => {
					const envVar = envVars[index] || { key: '', value: '' };
					const key = `envVar${index}`;
					const keyId = `envVarKey${index}`;
					const valueId = `envVarValue${index}`;
					return (
						<Fragment key={key}>
							<label
								className="col-start-1 col-end-2"
								htmlFor={keyId}
								key={keyId}
							>
								key:
								<input
									className="text-gray-900"
									id={keyId}
									defaultValue={envVar.key}
									type="text"
								/>
							</label>
							<label
								className="col-start-2 col-end-3"
								htmlFor={valueId}
								key={valueId}
							>
								value:
								<input
									className="text-gray-900"
									id={valueId}
									defaultValue={envVar.value}
									type="text"
								/>
							</label>
						</Fragment>
					);
				})}
			</fieldset>
		);
	},
);
const Tags = memo(
	({ count = 0, tags = [] }: { count?: number; tags?: string[] }) => {
		console.log('render tags');
		if (count === 0) return null;
		return (
			<fieldset id="tags">
				{new Array(count).fill(null).map((_, index) => {
					const tag = tags[index] || '';
					const id = `tag${index}`;
					return (
						<label htmlFor={id} key={id}>
							value:
							<input
								className="text-gray-900"
								id={id}
								defaultValue={tag}
								type="text"
							/>
						</label>
					);
				})}
			</fieldset>
		);
	},
);

interface IEnvVars {
	key: string;
	value: string;
}

export default memo(
	({
		name,
		executable,
		arguments: args,
		directory,
		envVars = {},
		tags = [],
	}: {
		name?: string;
		executable?: string;
		arguments?: string;
		directory?: string;
		envVars?: Record<string, string>;
		tags?: string[];
	}) => {
		console.log('render form');
		const { mutateAsync: createTask } = Task.useCreate();

		const environment = useRef(
			Object.entries(envVars).map(([key, value]) => ({ key, value })) || [],
		);
		const [envVarCount, setEnvVarCount] = useState(environment.current.length);
		const handleEnvVarIncrementVarCount = () => setEnvVarCount((x) => x + 1);
		const handleEnvVarDecrementVarCount = () =>
			setEnvVarCount((x) => {
				if (x < 1) return 0;
				return x - 1;
			});

		const [tagCount, setTagCount] = useState(tags.length);
		const handleTagIncrementVarCount = () => setTagCount((x) => x + 1);
		const handleTagDecrementVarCount = () =>
			setTagCount((x) => {
				if (x < 1) return 0;
				return x - 1;
			});

		const handleSubmit = useCallback<FormEventHandler<HTMLFormElement>>(
			(event) => {
				event.preventDefault();

				const target = event.target as HTMLFormElement;
				const envVarInputs: HTMLInputElement[] = target.envVars?.elements || [];
				const tagInputs: HTMLInputElement[] = target.tags?.elements || [];

				let envVarsCount = 0;
				const environment: Record<string, string> = {};
				for (let index = 0; index < envVarCount; index++) {
					const key = envVarInputs[envVarsCount++].value;
					const value = envVarInputs[envVarsCount++].value;
					environment[key] = value;
				}

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
			[envVarCount, tagCount, createTask],
		);

		for (let index = 0; index < envVarCount; index++) {}

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
				<EnvironmentVars count={envVarCount} envVars={environment.current} />
				<br />
				<button
					className="bg-green-400 w-10"
					onClick={handleEnvVarDecrementVarCount}
				>
					-
				</button>
				<button
					className="bg-green-400 w-10"
					onClick={handleEnvVarIncrementVarCount}
				>
					+
				</button>
				<br />
				<br />
				<Tags count={tagCount} tags={tags} />
				<br />
				<button
					className="bg-green-400 w-10"
					onClick={handleTagDecrementVarCount}
				>
					-
				</button>
				<button
					className="bg-green-400 w-10"
					onClick={handleTagIncrementVarCount}
				>
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
	},
);
