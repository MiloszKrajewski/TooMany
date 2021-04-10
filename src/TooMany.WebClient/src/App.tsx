import './App.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from 'react-query';
import { ReactNode, useEffect } from 'react';
import Home from '@tm/pages/Home';
import Editor from '@tm/pages/Editor';
import Terminal from '@tm/pages/Terminal';
import SignalR from './SignalR';
import useScreenType from '@hooks/useScreenType';
import Nav from '@components/navigation/Nav';

function Layout({ children }: { children: ReactNode }) {
	return (
		<div className="bg-white text-gray-900 dark:bg-gray-900 dark:text-white min-h-screen min-w-screen grid grid-cols-8">
			<aside className="col-span-1 dark:bg-gray-500 bg-opacity-40">
				<Nav />
			</aside>
			<main className="col-start-2 col-end-9">{children}</main>
		</div>
	);
}

function AppContent() {
	useEffect(() => {
		SignalR.start();
		return () => {
			SignalR.stop();
		};
	}, []);

	const screenType = useScreenType();
	console.log(screenType);

	return (
		<Routes>
			<Route
				path="/"
				element={
					<Layout>
						<Home />
					</Layout>
				}
			/>
			<Route
				path="/editor"
				element={
					<Layout>
						<Editor />
					</Layout>
				}
			/>
			<Route
				path="/editor/:name"
				element={
					<Layout>
						<Editor />
					</Layout>
				}
			/>
			<Route
				path="/terminal/:type/:name"
				element={
					<Layout>
						<Terminal />
					</Layout>
				}
			/>
		</Routes>
	);
}

const queryClient = new QueryClient({
	defaultOptions: {
		queries: {
			suspense: true,
		},
	},
});
function App() {
	return (
		<QueryClientProvider client={queryClient}>
			<Router>
				<AppContent />
			</Router>
		</QueryClientProvider>
	);
}

export default App;
