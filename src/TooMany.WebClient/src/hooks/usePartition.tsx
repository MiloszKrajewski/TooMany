import { useMemo } from 'react';

export default function <Data>(
	data: Data[],
	condition: (value: Data) => boolean,
) {
	return useMemo<[Data[], Data[]]>(() => {
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
