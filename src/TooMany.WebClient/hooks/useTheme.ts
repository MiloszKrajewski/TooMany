import { ref, computed } from '@nuxtjs/composition-api';
import { Ref } from '~/@types';

export enum FirstPartyThemes {
	light = 'light',
	dark = 'dark',
	system = 'system',
}

export type Themes = FirstPartyThemes | string;

export enum SupportedCssPropertyKeys {
	'text-color' = 'text-color',
	'background-color' = 'background-color',
}

export enum SupportedCssPropertyTypes {
	'text-color' = 'color',
	'background-color' = 'color',
}

enum localStorageKeys {
	current = 'selectedTheme',
	customThemes = 'customThemes',
}

type TTheme = Themes | string;
type TSelection = Ref<TTheme>;
type TAllCustom = Ref<Record<Themes, SupportedCssPropertyKeys>>;
type TInlineProps = Ref<string>;

export default function useTheme() {
	const selection: TSelection = ref(
		localStorage.getItem('selectedTheme') || FirstPartyThemes.system,
	);

	const allCustom: TAllCustom = ref(
		(() => {
			const themes = localStorage.getItem(localStorageKeys.customThemes);
			if (!themes) return {};
			return JSON.parse(themes);
		})(),
	);

	const selectedProperties = computed((): string => {
		return allCustom.value[selection.value];
	});

	const isFirstPartySelection = computed((): boolean => {
		return selection.value in FirstPartyThemes;
	});

	const inlineProperties = computed((): string => {
		let output = '';
		if (!selectedProperties.value) return output;
		for (const [property, value] of Object.entries(selectedProperties.value)) {
			if (!property || !value) continue;
			output += `--${property}: ${value}; `;
		}
		return output;
	});

	function onSelect(theme: TTheme = FirstPartyThemes.system) {
		selection.value = theme;
		localStorage.setItem('selectedTheme', theme);
	}

	function onSave(payload: SupportedCssPropertyKeys): void {
		allCustom.value[selection.value] = payload;

		localStorage.setItem(
			localStorageKeys.customThemes,
			JSON.stringify(allCustom.value),
		);
	}

	const all = computed(() => {
		let custom: string[] = [];
		if (allCustom.value) {
			custom = Object.keys(allCustom.value);
		}
		return [...Object.keys(FirstPartyThemes), ...custom];
	});

	return {
		all,
		selectedProperties,
		inlineProperties,
		onSave,
		onSelect,
		selection,
		isFirstPartySelection,
	};
}
