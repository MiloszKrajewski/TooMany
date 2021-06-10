module.exports = {
	extends: ['stylelint-config-standard', 'stylelint-config-prettier'],
	// add your custom config here
	// https://stylelint.io/user-guide/configuration
	rules: {},
	purge: ['./index.html', './src/**/*.{vue,js,ts,jsx,tsx}'],
	darkMode: false, // or 'media' or 'class'
	theme: {
		extend: {},
	},
	variants: {
		extend: {},
	},
	plugins: [],
};
