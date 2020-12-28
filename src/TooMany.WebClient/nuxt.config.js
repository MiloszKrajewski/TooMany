const apiUrl = process.env.API_URL || 'http://localhost:31337';

export default {
	// Disable server-side rendering (https://go.nuxtjs.dev/ssr-mode)
	ssr: false,

	// Target (https://go.nuxtjs.dev/config-target)
	target: 'static',

	// Global page headers (https://go.nuxtjs.dev/config-head)
	head: {
		title: 'TooMany',
		meta: [
			{ charset: 'utf-8' },
			{ name: 'viewport', content: 'width=device-width, initial-scale=1' },
			{ hid: 'description', name: 'description', content: '' },
		],
		link: [{ rel: 'icon', type: 'image/x-icon', href: '/favicon.ico' }],
	},

	// Global CSS (https://go.nuxtjs.dev/config-css)
	css: ['~/static/root.css'],

	// Plugins to run before rendering page (https://go.nuxtjs.dev/config-plugins)
	plugins: [
		'~/plugins/SignalR.ts',
		{ src: '~/plugins/Theme.ts', mode: 'client' },
	],

	// Modules for dev and build (recommended) (https://go.nuxtjs.dev/config-modules)
	buildModules: [
		'@nuxtjs/composition-api',
		// https://go.nuxtjs.dev/typescript
		'@nuxt/typescript-build',
		// https://go.nuxtjs.dev/stylelint
		'@nuxtjs/stylelint-module',
	],
	generate: {
		// choose to suit your project
		interval: 2000,
	},

	// Modules (https://go.nuxtjs.dev/config-modules)
	modules: [
		// https://go.nuxtjs.dev/pwa
		'@nuxtjs/pwa',
	],

	// Build Configuration (https://go.nuxtjs.dev/config-build)
	build: {
		postcss: {
			plugins: {
				'postcss-nested': {},
			},
		},
	},

	env: {
		baseUrl: process.env.BASE_URL || 'http://localhost:3000',
		realtimeUrl: `${apiUrl}/monitor`,
		apiV1Url: `${apiUrl}/api/v1`,
	},
};
