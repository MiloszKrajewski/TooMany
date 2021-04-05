import { useMemo } from 'react';

import type * as Types from './types.d';

export default function (edges: Types.Edge[]) {
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
