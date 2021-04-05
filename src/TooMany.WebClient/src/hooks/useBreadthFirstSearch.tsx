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
