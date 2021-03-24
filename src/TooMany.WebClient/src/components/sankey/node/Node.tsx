import type { INodeProps, INodeButtonProps } from '../types';

const NodeButton = ({ id, onClick }: INodeButtonProps) => (
	<button
		type="button"
		className="w-2.5 h-10 bg-gray-300 inline-block z-10 align-baseline"
		id={id}
		onClick={() => onClick(id)}
	></button>
);

export default function ({
	inputs = [],
	outputs = [],
	children,
	label,
	id,
}: INodeProps) {
	const hasInputs = inputs.length;
	const hasOutputs = outputs.length;
	const checkboxId = `${id}Checkbox`;
	const labelId = `${id}Label`;
	let roundedLeftClass = 'rounded-l';
	let roundedRightClass = 'rounded-r';
	let textAlignClass = 'text-center';
	let inputButton = null;
	let outputButton = null;
	if (hasInputs) {
		roundedLeftClass = '';
		textAlignClass = 'text-left';
		const inputId = `${id}InputButton`;
		inputButton = <NodeButton id={inputId} onClick={console.log} />;
	}
	if (hasOutputs) {
		roundedRightClass = '';
		textAlignClass = 'text-right';
		const outputId = `${id}OutputButton`;
		outputButton = <NodeButton id={outputId} onClick={console.log} />;
	}
	if (hasInputs && hasOutputs) {
		textAlignClass = 'text-center';
	}

	return (
		<div className="flex">
			{inputButton}
			<div
				id={id}
				className={` 
					bg-gray-100
					inline-block
					h-10
					p-2.5
					cursor-pointer
					select-none
					font-light
					leading-5
					font-mono
					z-0
					${roundedLeftClass}
					${roundedRightClass}
					${textAlignClass}
				`}
			>
				<label
					id={labelId}
					htmlFor={checkboxId}
					className="cursor-pointer border-box"
				>
					{children || label}
					<input type="checkbox" className="hidden" id={checkboxId} />
				</label>
			</div>
			{outputButton}
		</div>
	);
}
