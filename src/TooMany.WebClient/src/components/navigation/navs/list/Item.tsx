import { ReactNode } from 'react';

export default function ({
	children,
	isSelected = false,
	isAssociated = false,
	isNoPadding = false,
}: {
	children: ReactNode;
	isAssociated?: boolean;
	isSelected?: boolean;
	isNoPadding?: boolean;
}) {
	let className = 'text-white';
	let leftAdornment = null;
	if (!isNoPadding) {
		className += ' pl-3';
	}
	if (isSelected) {
		className += ' text-purple-500';
		leftAdornment = <span>&gt;</span>;
	}
	if (isAssociated) {
		className += ' text-purple-300';
		leftAdornment = <span>-</span>;
	}
	return (
		<li className={className}>
			{leftAdornment}
			{children}
		</li>
	);
}