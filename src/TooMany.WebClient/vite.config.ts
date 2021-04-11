import { defineConfig } from 'vite';
import reactRefresh from '@vitejs/plugin-react-refresh';
import path from 'path';
import pkg from './package.json';

const apiUrl = process.env.API_URL || 'http://localhost:31337';

// https://vitejs.dev/config/
export default defineConfig({
	plugins: [reactRefresh()],
	esbuild: {
		jsxInject: `import React from 'react'`,
	},
	resolve: {
		alias: {
			'@tm': path.resolve(__dirname, 'src'),
			'@types': path.resolve(__dirname, 'src', 'types'),
			'@enums': path.resolve(__dirname, 'src', 'enums'),
			'@pages': path.resolve(__dirname, 'src', 'pages'),
			'@components': path.resolve(__dirname, 'src', 'components'),
			'@hooks': path.resolve(__dirname, 'src', 'hooks'),
		},
	},
	define: {
		env: JSON.stringify({
			version: pkg.version,
			baseUrl: process.env.BASE_URL || 'http://localhost:3000',
			realtimeUrl: `${apiUrl}/monitor`,
			apiV1Url: `${apiUrl}/api/v1`,
		}),
	},
});
