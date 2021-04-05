import type { IEdge, ISankeyProps } from './types';
import type { ReactNode } from 'react';

import Edge from './edge/Edge';
import Node from './node/Node';
import useBreadthFirstSearch from '../../hooks/useBreadthFirstSearch';
import { useState, useMemo, useRef } from 'react';
import { useDebouncedCallback } from 'use-debounce';
interface IColumnNode {
	id: string;
	depth: number;
	inputs?: string[];
	outputs?: string[];
}

const twColors = [
	'indigo',
	'grey',
	'red',
	'blue',
	'green',
	'purple',
	'yellow',
	'pink',
];

const Edges = ({
	edges,
	bounds,
}: {
	edges: IEdge[];
	bounds: Record<string, Record<string, number>>;
}) => {
	const [selectedId, setSelectedId] = useState<string>('');

	const colors = useMemo(() => {
		let c = [...twColors];
		const result: Record<string, string> = {};
		for (const [output] of edges) {
			if (c.length === 0) c = [...twColors];
			const current = c.pop();
			if (current) {
				result[output] = current;
			}
		}
		return result;
	}, [edges]);

	const paths = useMemo(() => {
		return edges
			.filter(
				([output, input]) =>
					typeof input !== 'undefined' &&
					typeof bounds[output] !== 'undefined' &&
					typeof bounds[input] !== 'undefined',
			)
			.map((edge) => {
				const [output, input] = edge as [string, string];
				const outBounds = bounds[output];
				const inBounds = bounds[input];
				const outWidth = Math.round(outBounds.width);
				const outHeight = Math.round(outBounds.height);
				const x = outBounds.x + outWidth;
				const left = {
					top: {
						x,
						y: outBounds.y,
					},
					bottom: {
						x: 0,
						y: outHeight,
					},
				};
				const inHeight = Math.round(inBounds.height);
				const right = {
					top: {
						x: inBounds.x - left.top.x,
						y: inBounds.y - left.top.y,
					},
					bottom: {
						x: 0,
						y: 0 - inHeight,
					},
				};
				return { left, right, output, input };
			});
	}, [edges, bounds]);

	const setSelection = (value: string) => {
		setSelectedId((selectedId) => {
			if (selectedId === value) return selectedId;
			return value;
		});
	};

	return (
		<svg className="absolute top-0 left-0 w-full h-full pointer-events-none">
			{paths.map(({ left, right, output, input }) => {
				const id = `${output}/${input}`;
				return (
					<g key={id} className="pointer-events-auto">
						<Edge
							className="pointer-events-auto"
							id={id}
							left={left}
							right={right}
							color={colors[output]}
							onMouseEnter={setSelection}
							isHighlighted={id === selectedId}
						/>
					</g>
				);
			})}
			<use
				xlinkHref={`#${selectedId}`}
				onMouseLeave={() => setSelectedId('')}
			/>
		</svg>
	);
};

export default ({ edges = [] }: ISankeyProps) => {
	const [orphans, nodes] = useBreadthFirstSearch(edges);
	const [updates, edgeUpdateCount] = useState(0);
	const nodeLords = useRef<Record<string, ReactNode>>({});

	const de_resize = useDebouncedCallback(() => {
		edgeUpdateCount((c) => c + 1);
	}, 100);
	window.addEventListener('resize', de_resize);

	const bounds = useMemo(() => {
		const result: Record<string, any> = {};
		for (const [id, node] of Object.entries(nodeLords.current)) {
			if (!node) {
				continue;
			}
			if (node instanceof HTMLElement) {
				result[id] = node.getBoundingClientRect();
			}
		}
		return result;
	}, [updates]);

	const [outputMap, inputMap] = useMemo(() => {
		const o: Record<string, string[]> = {};
		const i: Record<string, string[]> = {};
		for (const [output, input] of edges) {
			if (!input) continue;
			if (!o[output]) o[output] = [];
			if (!i[input]) i[input] = [];
			o[output].push(input);
			i[input].push(output);
		}
		return [o, i];
	}, [edges]);

	const columns = useMemo(() => {
		const columns: IColumnNode[][] = [];
		for (const node of nodes) {
			const index = node.depth;
			if (!columns[index]) columns[index] = [];
			columns[index].push(node);
		}
		return columns;
	}, [nodes]);

	return (
		<div>
			<div>
				{orphans.map((node) => (
					<Node key={node.id} id={node.id}>
						{node.id}
					</Node>
				))}
			</div>
			<Edges edges={edges} bounds={bounds} />
			<div
				className="grid"
				style={{ gridTemplateColumns: `repeat(${columns.length}, 1fr)` }}
			>
				{columns.map((nodes, column) => (
					<div key={column} id={`column-${column}`}>
						{nodes.map((node) => (
							<div className="mt-10" key={node.id}>
								<Node
									id={node.id}
									inputs={inputMap[node.id]}
									outputs={outputMap[node.id]}
									ref={(el: ReactNode) => {
										nodeLords.current[node.id] = el;
										if (
											Object.entries(nodeLords.current).length === nodes.length
										) {
											edgeUpdateCount((c) => c + 1);
										}
									}}
								>
									{node.id}
								</Node>
							</div>
						))}
					</div>
				))}
			</div>
		</div>
	);
};
