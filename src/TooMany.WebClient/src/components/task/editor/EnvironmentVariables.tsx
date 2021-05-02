import { Fragment } from 'react';
import { memo, useRef } from 'react';

interface IEnvironmentVariable {
	key: string;
	value: string;
}

const defaultEnvironmentVariable = {
	key: '',
	value: '',
};

const EnvironmentVariables = ({
	count = 0,
	environmentVariables = {},
}: {
	count?: number;
	environmentVariables?: Record<string, string>;
}) => {
	const initialEnvironmentVariables = useRef<IEnvironmentVariable[]>(
		Object.entries(environmentVariables).map(([key, value]) => ({
			key,
			value,
		})) || [],
	);

	if (count === 0) return null;
	return (
		<fieldset id="environmentVariables" className="grid grid-cols-3">
			{new Array(count).fill(null).map((_, index) => {
				const environmentVariable =
					initialEnvironmentVariables.current[index] ||
					defaultEnvironmentVariable;
				const key = `environmentVariable/${index}`;
				const keyId = `${key}/Key`;
				const valueId = `${key}/Value`;
				return (
					<Fragment key={key}>
						<label className="col-start-1 col-end-2" htmlFor={keyId}>
							key:
							<input
								className="text-gray-900"
								id={keyId}
								defaultValue={environmentVariable.key}
								type="text"
							/>
						</label>
						<label className="col-start-2 col-end-3" htmlFor={valueId}>
							value:
							<input
								className="text-gray-900"
								id={valueId}
								defaultValue={environmentVariable.value}
								type="text"
							/>
						</label>
					</Fragment>
				);
			})}
		</fieldset>
	);
};
export default memo(EnvironmentVariables);
