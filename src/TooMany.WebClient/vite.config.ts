import path from 'path';

import reactRefresh from '@vitejs/plugin-react-refresh';
import { defineConfig } from 'vite';

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
			types: path.resolve(__dirname, 'types'),
			'@tm': path.resolve(__dirname, 'src'),
			'@enums': path.resolve(__dirname, 'src', 'enums'),
			'@pages': path.resolve(__dirname, 'src', 'pages'),
			'@components': path.resolve(__dirname, 'src', 'components'),
			'@helpers': path.resolve(__dirname, 'src', 'helpers'),
			'@hooks': path.resolve(__dirname, 'src', 'hooks'),
		},
	},
	define: {
		env: JSON.stringify({
			version: pkg.version,
			dependencies: Object.keys(pkg.dependencies),
			baseUrl: process.env.BASE_URL || 'http://localhost:3000',
			realtimeUrl: `${apiUrl}/monitor`,
			apiV1Url: `${apiUrl}/api/v1`,
		}),
	},
	build: {
		outDir: path.join(__dirname, '..', 'TooMany.WebServer', 'wwwroot'),
		emptyOutDir: true,
	},
});
