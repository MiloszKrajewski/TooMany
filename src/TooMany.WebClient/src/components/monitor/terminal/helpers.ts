import { getCulture } from '@tm/helpers/culture';

const culture = getCulture();
export function formatLine(text: string, timestamp: string, parent?: string) {
	const formattedTimestamp = new Date(timestamp).toLocaleString(culture, {
		hour12: false,
	});
	let label = formattedTimestamp;
	if (typeof parent === 'string') {
		label += ` ${parent}`;
	}
	return `[${label}] - ${text}`;
}
