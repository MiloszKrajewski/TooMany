import { ref } from '@nuxtjs/composition-api';
import { Ref } from '~/@types';

export enum FirstPartyThemes {
	light = 'light',
	dark = 'dark',
	system = 'system',
}

export type Themes = FirstPartyThemes | string;

interface SupportedCssProperties {
	'text-color': string;
	'background-color': string;
	'dark-background-color'?: string;
}

enum localStorageKeys {
	current = 'selectedTheme',
	customThemes = 'customThemes',
}

type TTheme = Themes | string;
type TSelection = Ref<TTheme>;
type TAll = Ref<Record<string, SupportedCssProperties>>;
type TInlineProps = Ref<string>;

function transformProperties(selection: TSelection, all: TAll): string {
	let output = '';
	if (!all.value || !selection.value) return output;
	const selectedTheme = all.value[selection.value];
	if (!selectedTheme) return output;
	for (const [property, value] of Object.entries(selectedTheme)) {
		if (!property || !value) continue;
		output += `--${property}: ${value}; `;
	}
	return output;
}

function getAll() {
	const themes = localStorage.getItem(localStorageKeys.customThemes);
	if (!themes) return null;
	return JSON.parse(themes);
}

export default function useTheme() {
	const selection: TSelection = ref(
		localStorage.getItem('selectedTheme') || FirstPartyThemes.system,
	);

	const all: TAll = ref(getAll());
	const inlineProperties: TInlineProps = ref(
		transformProperties(selection, all),
	);

	function onSelect(theme: TTheme = FirstPartyThemes.system) {
		selection.value = theme;
		localStorage.setItem('selectedTheme', theme);
		inlineProperties.value = transformProperties(selection, all);
	}

	function onSave(payload: SupportedCssProperties): void {
		localStorage.setItem(
			localStorageKeys.customThemes,
			JSON.stringify({ [selection.value]: payload }),
		);
	}

	return {
		all: [...Object.keys(FirstPartyThemes), ...Object.keys(all.value || {})],
		inlineProperties,
		onSave,
		onSelect,
		selection,
	};
}
