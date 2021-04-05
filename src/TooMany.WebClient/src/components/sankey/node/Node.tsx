import type { INodeProps, INodeButtonProps } from '../types';
import { forwardRef } from 'react';

const NodeButton = ({ id, onClick }: INodeButtonProps) => (
	<button
		className="w-2.5 h-10 bg-gray-300"
		id={id}
		onClick={() => onClick(id)}
	></button>
);

export default forwardRef<HTMLDivElement, INodeProps>(
	({ className, inputs = [], outputs = [], children, label, id }, ref) => {
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
			<div
				id={id}
				className={`
						inline-flex
						bg-gray-100
						h-10
						cursor-pointer
						select-none
						font-light
						font-mono
						${roundedLeftClass}
						${roundedRightClass}
						${textAlignClass}
						${className}
					`}
				ref={ref}
			>
				{inputButton}
				<label
					id={labelId}
					htmlFor={checkboxId}
					className="cursor-pointer border-box px-2.5 m-auto"
				>
					{children || label}
					<input type="checkbox" className="hidden" id={checkboxId} />
				</label>
				{outputButton}
			</div>
		);
	},
);
