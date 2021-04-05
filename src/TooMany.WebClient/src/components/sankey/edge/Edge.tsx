import type { IEdgeProps } from '../types';
import type { MouseEvent } from 'react';

export default ({
	className = '',
	id,
	left,
	right,
	color = 'red',
	isHighlighted = false,
	onMouseEnter,
	onMouseLeave,
}: IEdgeProps) => {
	let colorDepth = '100';
	let opacity = '60';
	const highlightedColor = '400';
	const highlightedOpacity = '100';
	if (isHighlighted) {
		colorDepth = highlightedColor;
		opacity = highlightedOpacity;
	}

	const onMouseEnterById =
		typeof onMouseEnter === 'undefined'
			? undefined
			: (e: MouseEvent) => {
					e.preventDefault();
					onMouseEnter(id);
			  };
	const onMouseLeaveById =
		typeof onMouseLeave === 'undefined'
			? undefined
			: (e: MouseEvent) => {
					e.preventDefault();
					onMouseLeave(id);
			  };

	return (
		<path
			onMouseEnter={onMouseEnterById}
			onMouseLeave={onMouseLeaveById}
			d={`
                m ${left.top.x},${left.top.y}
                l ${left.bottom.x},${left.bottom.y}
                l ${right.top.x},${right.top.y}
                l ${right.bottom.x},${right.bottom.y}
                z
            `}
			id={id}
			className={`
                fill-current
                stroke-current
                transition-all
                text-${color}-${colorDepth}
                hover:text-${color}-${highlightedColor}
                opacity-${opacity}
                hover:opacity-${highlightedOpacity}
				${className}
            `}
		/>
	);
};
