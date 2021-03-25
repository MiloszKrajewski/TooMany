import { useMemo } from 'react';
import usePartition from './usePartition';

namespace Types {
	export type Edge = [string, string] | [string];
	export type AdjacencyList = Record<string, string[]>;
	export interface EdgeWithDepth {
		id: string;
		depth: number;
	}
}

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
		const result: Record<string, boolean> = {};
		for (const [a] of relationships) {
			if (typeof a !== 'undefined') {
				if (!result[a]) {
					result[a] = true;
				}
			}
		}
		for (const [, b] of relationships) {
			if (typeof b !== 'undefined') {
				delete result[b];
			}
		}
		return Object.keys(result);
	}, [relationships]);
}

function useBreadthFirstSearch(
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
		adjacencyList[current.id].forEach((neighbor) => {
			if (!visited[neighbor]) {
				visited[neighbor] = true;
				queue.push({ id: neighbor, depth: current.depth + 1 });
			}
		});
	}
	return result;
}

export default function (edges: Types.Edge[]) {
	const adjacencyList = useAdjacencyList(edges);
	const [relationships, orphans] = usePartition<Types.Edge>(
		edges,
		([, b]) => typeof b !== 'undefined',
	);
	const start = useStartOfGraph(relationships);
	const edgesWithDepth = useBreadthFirstSearch(start, adjacencyList);
	const orphansWithDepth = orphans.map((o) => ({ id: o, depth: -1 }));
	return [...orphansWithDepth, ...edgesWithDepth];
}
