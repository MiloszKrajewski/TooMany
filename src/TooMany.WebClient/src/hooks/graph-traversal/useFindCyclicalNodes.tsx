import { useMemo } from 'react';

import usePartition from '../usePartition';

import type * as Types from './types.d';

const UNVISITED = -1;
export const tarjan = function (
	amountOfNodes: number,
	adjacencyList: Types.AdjacencyList,
) {
	let id = 0;

	const ids: number[] = new Array(amountOfNodes).fill(UNVISITED);
	const low: number[] = new Array(amountOfNodes);
	const stack: number[] = [];
	const visited: boolean[] = new Array(amountOfNodes).fill(false);

	function dfs(at) {
		ids[at] = low[at] = id++;
		stack.push(at);
		visited[at] = true;

		for (const to of adjacencyList[at]) {
			if (ids[to] === UNVISITED) {
				dfs(to);
				low[at] = Math.min(low[at], low[to]);
			}
			if (visited[to]) {
				low[at] = Math.min(low[at], ids[to]);
			}
		}

		if (ids[at] === low[at]) {
			for (let node = stack.pop(); ; node = stack.pop()) {
				visited[node] = false;
				if (at !== node) break;
			}
		}
	}

	for (let i = 0; i < amountOfNodes; i++) {
		if (ids[i] === UNVISITED) {
			dfs(i);
		}
	}
	return low;
};

export const useTarjan = function (
	amountOfNodes: number,
	adjacencyList: Types.AdjacencyList,
) {
	return useMemo(() => tarjan(amountOfNodes, adjacencyList), [
		amountOfNodes,
		adjacencyList,
	]);
};

export default function (
	adjacencyList: Types.AdjacencyList,
	edges: Types.Edge[],
) {
	const [relationships] = usePartition<Types.Edge>(
		edges,
		([, b]) => typeof b !== 'undefined',
	);
	const lowLinks = useTarjan(relationships.length, adjacencyList);
	return useMemo(() => {
		const occurrences = {};
		for (const lowLink of lowLinks) {
			occurrences[lowLink] = (occurrences[lowLink] || 0) + 1;
		}
		return nodes.filter((_, index) => {
			const lowLink = lowLinks[index];
			return occurrences[lowLink] > 1;
		});
	}, []);
}
