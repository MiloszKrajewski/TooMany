import { useMemo } from 'react';

export function useSortedItems<Item extends { sortKey: string }>(
	items: Item[],
) {
	return useMemo(() => {
		return items.sort((a, b) => {
			if (a.sortKey < b.sortKey) {
				return -1;
			}
			if (a.sortKey > b.sortKey) {
				return 1;
			}
			return 0;
		});
	}, [items]);
}
