import { memo } from 'react';
import type { ReactNode } from 'react';

function Item({
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
	let textColor = 'text-white';
	let padding = '';
	let leftAdornment = null;
	if (!isNoPadding) {
		padding += 'pl-3';
	}
	if (isSelected) {
		textColor = 'text-purple-500';
		leftAdornment = <span>&gt;</span>;
	}
	if (isAssociated) {
		textColor = 'text-purple-300';
		leftAdornment = <span>-</span>;
	}
	return (
		<li className={`${textColor} ${padding}`}>
			{leftAdornment} {children}
		</li>
	);
}

export default memo(Item);
