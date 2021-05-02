export function getCulture() {
	let culture: string;
	switch (typeof navigator.languages) {
		case 'string':
			culture = navigator.languages;
			break;
		case 'object':
			culture = navigator.languages[0];
			break;
		default:
			culture = 'en-GB';
			break;
	}
	return culture;
}
