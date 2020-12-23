/* eslint-disable no-console */
import { Plugin } from '@nuxt/types';
import { ref } from '@nuxtjs/composition-api';

export enum FirstPartyThemes {
	light = 'light',
	dark = 'dark',
	system = 'system',
}

export type Themes = FirstPartyThemes | string;

type SetTheme = (theme: Themes) => void;

declare module 'vue/types/vue' {
	interface Vue {
		$Theme: {
			set: SetTheme;
			current: Themes;
		};
	}
}

declare module '@nuxt/types' {
	interface NuxtAppOptions {
		$Theme: {
			set: SetTheme;
			current: Themes;
		};
	}
	interface Context {
		$Theme: {
			set: SetTheme;
			current: Themes;
		};
	}
}

const $Theme: Plugin = (_, inject) => {
	const currentTheme = ref(
		localStorage.getItem('currentTheme') || FirstPartyThemes.system,
	);

	function set(theme: Themes = FirstPartyThemes.system): void {
		currentTheme.value = theme;
		localStorage.setItem('currentTheme', theme);
	}

	inject('Theme', { set, current: currentTheme });
};

export default $Theme;
