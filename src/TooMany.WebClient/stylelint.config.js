module.exports = {
	extends: [
		'stylelint-config-standard',
		'stylelint-config-prettier',
		'stylelint-config-recommended',
		'stylelint-prettier/recommended',
	],
	// add your custom config here
	// https://stylelint.io/user-guide/configuration
	purge: ['./index.html', './src/**/*.{js,ts,jsx,tsx}'],
	darkMode: false, // or 'media' or 'class'
	theme: {
		extend: {},
	},
	variants: {
		extend: {},
	},
	plugins: ['stylelint-prettier'],
	rules: {
		'prettier/prettier': true,
		'at-rule-no-unknown': [
			true,
			{
				ignoreAtRules: ['extends', 'tailwind'],
			},
		],
		'block-no-empty': null,
		'unit-whitelist': ['em', 'rem', '%'],
	},
};
