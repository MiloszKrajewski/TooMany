import type { IEdgeProps } from '../types';

export default ({
	id,
	left,
	right,
	color = 'red',
	isHighlighted,
}: IEdgeProps) => {
	let colorDepth = '100';
	let opacity = '25';
	let zIndex = '0';
	const highlightedColor = '400';
	const highlightedOpacity = '100';
	const highlightedZIndex = '50';
	if (isHighlighted) {
		colorDepth = highlightedColor;
		opacity = highlightedOpacity;
		zIndex = highlightedZIndex;
	}

	return (
		<path
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
                bg-opacity-${opacity}
                hover:bg-opacity-${highlightedOpacity}
                z-${zIndex}
                hover:z-${highlightedZIndex}
            `}
		/>
	);
};
