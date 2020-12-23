import { ref, computed } from '@nuxtjs/composition-api';
import { Ref } from '~/@types';

enum FirstPartyThemeNames {
	system = 'system',
	light = 'light',
	dark = 'dark',
}

type TThemesNames = FirstPartyThemeNames | string;

export namespace SupportedCssProperty {
	export enum Keys {
		'text-color' = 'text-color',
		'background-color' = 'background-color',
	}

	export enum Types {
		'text-color' = 'color',
		'background-color' = 'color',
	}
}

enum localStorageKeys {
	current = 'selectedTheme',
	customThemes = 'customThemes',
}

type TSelection = Ref<TThemesNames>;
type TAllCustom = Record<TThemesNames, SupportedCssProperty.Keys>;
type TRefAllCustom = Ref<TAllCustom>;

function saveThemeName(name: string) {
	localStorage.setItem(localStorageKeys.current, name);
}

function saveThemeValues(theme: TAllCustom) {
	localStorage.setItem(localStorageKeys.customThemes, JSON.stringify(theme));
}

export default function useTheme() {
	const selection: TSelection = ref(
		localStorage.getItem(localStorageKeys.current) ||
			FirstPartyThemeNames.system,
	);

	const allCustom: TRefAllCustom = ref(
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
		return selection.value in FirstPartyThemeNames;
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

	const all = computed(() => {
		let custom: string[] = [];
		if (allCustom.value) {
			custom = Object.keys(allCustom.value);
		}
		return [...Object.keys(FirstPartyThemeNames), ...custom];
	});

	function onSelect(name: TThemesNames = FirstPartyThemeNames.system) {
		selection.value = name;
		saveThemeName(name);
	}

	function onSave(
		name: TThemesNames,
		payload: SupportedCssProperty.Keys,
		isNew: boolean,
	): void {
		const isSelectedName = selection.value === name;
		const isFirstPartyName = name in FirstPartyThemeNames;
		const isThirdPartyName = Boolean(allCustom.value[name]);

		const isNameTaken =
			!isSelectedName && (isFirstPartyName || isThirdPartyName);

		if (isNameTaken) return;

		if (!isNew && !isSelectedName) {
			delete allCustom.value[selection.value];
		}

		selection.value = name;
		allCustom.value = { ...allCustom.value, [name]: payload };
		saveThemeName(selection.value);
		saveThemeValues(allCustom.value);
	}

	function onDelete(): void {
		// eslint-disable-next-line @typescript-eslint/no-unused-vars
		const { [selection.value]: _, ...remainingCustom } = allCustom.value;

		selection.value = FirstPartyThemeNames.system;
		allCustom.value = remainingCustom;
		saveThemeName(selection.value);
		saveThemeValues(allCustom.value);
	}

	return {
		all,
		selectedProperties,
		inlineProperties,
		onDelete,
		onSave,
		onSelect,
		selection,
		isFirstPartySelection,
	};
}
