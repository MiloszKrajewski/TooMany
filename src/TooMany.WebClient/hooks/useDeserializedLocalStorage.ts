export default function <T>(key: string) {
	const serialized = localStorage.getItem(key);
	if (!serialized) return null;
	const deserialized: T = JSON.parse(serialized);
	return deserialized;
}
