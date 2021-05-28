module.exports = {
	root: true,
	parser: '@typescript-eslint/parser',
	parserOptions: {
		ecmaFeatures: { jsx: true },
		ecmaVersion: 2021,
		sourceType: 'module',
	},
	extends: [
		'eslint:recommended',
		'plugin:prettier/recommended',
		'plugin:@typescript-eslint/eslint-recommended',
		'plugin:@typescript-eslint/recommended',
		'plugin:import/recommended',
		'plugin:import/typescript',
	],
	plugins: [
		'@typescript-eslint',
		'react',
		'react-hooks',
		'import',
		'prettier',
	],
	env: {
		browser: true,
		node: true,
	},
	settings: {
		'import/resolver': {
			typescript: { alwaysTryTypes: true },
		},
	},
	globals: { env: 'readonly' },
	rules: {
		'prettier/prettier': 'error',

		'@typescript-eslint/explicit-module-boundary-types': 'off',
		camelcase: 'off',

		'import/default': 'error',
		'import/no-unresolved': 'error',
		'import/order': [
			'error',
			{
				'newlines-between': 'always',
				alphabetize: {
					order: 'asc',
					caseInsensitive: true,
				},
				groups: [
					'builtin',
					'external',
					'internal',
					'parent',
					'sibling',
					'index',
				],
				pathGroups: [
					{
						pattern: 'types/**',
						group: 'internal',
						position: 'after',
					},
					{
						pattern: '@tm/**',
						group: 'internal',
						position: 'after',
					},
					{
						pattern: '@enums/**',
						group: 'internal',
						position: 'after',
					},
					{
						pattern: '@pages/**',
						group: 'internal',
						position: 'after',
					},
					{
						pattern: '@components/**',
						group: 'internal',
						position: 'after',
					},
					{
						pattern: '@hooks/**',
						group: 'internal',
						position: 'after',
					},
				],
			},
		],
	},
};
