import './App.css';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from 'react-query';
import type { ReactNode } from 'react';
import Home from '@tm/pages/Home';
import Define from '@tm/pages/Define';
import Monitor from '@tm/pages/Monitor';
import useScreenType from '@hooks/useScreenType';
import Navigation from '@components/navigation';
import SignalR from '@tm/SignalR';
import { useEffect } from 'react';
import { useRoutes } from '@hooks/Navigation';

function Layout({ children }: { children: ReactNode }) {
	const screenType = useScreenType();
	console.log(screenType);

	return (
		<div className="bg-white text-gray-900 dark:bg-gray-900 dark:text-white min-h-screen min-w-screen grid grid-cols-8">
			<aside className="col-span-1 dark:bg-gray-500 bg-opacity-40">
				<div className="sticky top-0">
					<Navigation />
				</div>
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
	const routes = useRoutes();

	return (
		<Routes>
			<Route
				path={routes.home()}
				element={
					<Layout>
						<Home />
					</Layout>
				}
			/>
			<Route
				path={routes.define()}
				element={
					<Layout>
						<Define />
					</Layout>
				}
			/>
			<Route
				path={routes.redefine()}
				element={
					<Layout>
						<Define />
					</Layout>
				}
			/>
			<Route
				path={routes.monitor()}
				element={
					<Layout>
						<Monitor />
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
			refetchOnReconnect: false, // it's a local app, internet connection doesn't matter
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
