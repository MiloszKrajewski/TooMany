module.exports = {
	root: true,
	env: {
		browser: true,
		node: true,
	},
	extends: ['prettier', 'plugin:prettier/recommended'],
	plugins: ['prettier'],
	rules: {
		camelcase: 'off',
	},
	globals: { env: 'readonly' },
};
