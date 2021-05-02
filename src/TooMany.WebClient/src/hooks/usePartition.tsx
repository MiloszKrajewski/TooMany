import { useMemo } from 'react';

export default function <T>(data: T[], condition: (value: T) => boolean) {
	return useMemo<[T[], T[]]>(() => {
		const a = [];
		const b = [];
		for (const value of data) {
			if (condition(value)) {
				a.push(value);
			} else {
				b.push(value);
			}
		}
		return [a, b];
	}, [data]);
}
