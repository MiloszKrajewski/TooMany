import { useMemo } from 'react';
import usePartition from './usePartition';
import type * as Types from './useBreadthFirstSearch.d';

function useAdjacencyList(edges: Types.Edge[]) {
	return useMemo<Types.AdjacencyList>(() => {
		const result: Types.AdjacencyList = {};
		for (const [a, b] of edges) {
			if (typeof b !== 'undefined') {
				if (!result[a]) result[a] = [];
				result[a].push(b);
			}
		}
		return result;
	}, [edges]);
}

function useStartOfGraph(relationships: Types.Edge[]) {
	return useMemo<string[]>(() => {
		const start: Record<string, boolean> = {};
		const end: Record<string, boolean> = {};
		for (const [a] of relationships) {
			if (typeof a !== 'undefined') {
				if (!start[a]) {
					start[a] = true;
				}
			}
		}
		for (const [, b] of relationships) {
			if (typeof b !== 'undefined') {
				delete start[b];
			}
		}
		return Object.keys(start);
	}, [relationships]);
}

function useBreadthFirstSearch(
	start: string[],
	adjacencyList: Types.AdjacencyList,
) {
	return useMemo(() => {
		const depthById: Record<string, number> = {};
		const visited: Record<string, boolean> = {};
		const queue = start.map((id) => ({ id, depth: 0 }));

		while (queue.length > 0) {
			let current = queue.shift() || { id: '', depth: 0 };
			depthById[current.id] = current.depth;
			if (!adjacencyList[current.id]) {
				continue;
			}
			for (const neighbor of adjacencyList[current.id]) {
				const vId = `${current.id}/${neighbor}`;
				if (visited[vId] && depthById[neighbor]) {
					const diff = depthById[current.id] - depthById[neighbor];
					if (diff > 0) {
						depthById[neighbor] += current.depth;
						continue;
					}
				}
				visited[vId] = true;
				queue.push({ id: neighbor, depth: current.depth + 1 });
			}
		}
		const result: Types.EdgeWithDepth[] = [];
		for (const [id, depth] of Object.entries(depthById)) {
			result.push({ id, depth });
		}
		return result;
	}, [start, adjacencyList]);
}

export default function (
	edges: Types.Edge[],
): [Types.EdgeWithDepth[], Types.EdgeWithDepth[]] {
	const adjacencyList = useAdjacencyList(edges);
	const [relationships, orphans] = usePartition<Types.Edge>(
		edges,
		([, b]) => typeof b !== 'undefined',
	);
	const start = useStartOfGraph(relationships);
	const edgesWithDepth = useBreadthFirstSearch(start, adjacencyList);
	const orphansWithDepth = orphans.map(([id]) => ({ id, depth: -1 }));
	return [orphansWithDepth, edgesWithDepth];
}
