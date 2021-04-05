import { useMemo } from 'react';

import usePartition from '../usePartition';

import type * as Types from './types.d';
import useStartOfGraph from './useStartOfGraph';

export const breadthFirstSearch = function (
	start: string[],
	adjacencyList: Types.AdjacencyList,
) {
	const result: Types.EdgeWithDepth[] = [];
	const visited: Record<string, boolean> = {};
	const queue = start.map((id) => {
		visited[id] = true;
		return { id, depth: 0 };
	});

	while (queue.length > 0) {
		let current = queue.shift() || { id: '', depth: 0 };
		result.push({ id: current.id, depth: current.depth });
		if (!adjacencyList[current.id]) {
			continue;
		}
		for (const neighbor of adjacencyList[current.id]) {
			if (!visited[neighbor]) {
				visited[neighbor] = true;
				queue.push({ id: neighbor, depth: current.depth + 1 });
			}
		}
	}
	return result;
};

export const useBreadthFirstSearch = function (
	start: string[],
	adjacencyList: Types.AdjacencyList,
) {
	return useMemo(() => breadthFirstSearch(start, adjacencyList), [
		start,
		adjacencyList,
	]);
};

export default function (
	adjacencyList: Types.AdjacencyList,
	edges: Types.Edge[],
): [Types.EdgeWithDepth[], Types.EdgeWithDepth[]] {
	const [relationships, orphans] = usePartition<Types.Edge>(
		edges,
		([, b]) => typeof b !== 'undefined',
	);
	const start = useStartOfGraph(relationships);
	const edgesWithDepth = useBreadthFirstSearch(start, adjacencyList);
	const orphansWithDepth = orphans.map(([id]) => ({ id, depth: -1 }));
	return [orphansWithDepth, edgesWithDepth];
}
