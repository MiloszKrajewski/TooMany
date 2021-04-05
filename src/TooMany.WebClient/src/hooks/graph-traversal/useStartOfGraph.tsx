import { useMemo } from 'react';

import type * as Types from './types.d';

export default function (relationships: Types.Edge[]) {
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
