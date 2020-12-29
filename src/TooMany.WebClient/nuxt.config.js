import pkg from './package.json';
const apiUrl = process.env.API_URL || 'http://localhost:31337';

const isInstallable = process.argv.includes('--install');
const isProd = process.env.NODE_ENV === 'production';
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
		'~/plugins/Theme.client.ts',
		'~/plugins/Notification.client.ts',
		'~/plugins/UserConfig.client.ts',
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

	pwa: {
		meta: {
			name: 'TooMany',
			author: 'TooManyDevs',
			description: 'TooMany terminals TooMany Problems.',
			lang: 'en',
		},
		manifest: {
			name: 'TooMany',
			short_name: '2Many',
			description: 'TooMany terminals TooMany Problems.',
			lang: 'en',
			useWebmanifestExtension: false,
			display: 'fullscreen',
		},
		workbox: {
			enabled: isProd || isInstallable,
			swURL: '/sw.js',
			swDest: './static/sw.js',
			publicPath: '/.nuxt',
			autoRegister: true,
			cacheOptions: {
				cacheId: pkg.name,
				directoryIndex: '/',
				revision: pkg.version,
			},
		},
	},
};
