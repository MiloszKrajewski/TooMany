import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from 'react-query';
import type { ReactNode } from 'react';
import { Home, Define, Monitor } from '@pages/index';
import { useScreenType } from '@hooks/index';
import Navigation from '@components/navigation';
import SignalR from '@tm/SignalR';
import { useEffect } from 'react';
import { useRoutes } from '@hooks/Navigation';
import { Task } from '@hooks/API';
import type * as Realtime from '@tm/types/realtime';

function Layout({ children }: { children: ReactNode }) {
	const screenType = useScreenType();
	console.log(screenType);

	return (
		<div className="bg-white text-gray-900 dark:bg-gray-900 dark:text-white min-h-screen min-w-screen grid grid-cols-8">
			<aside className="col-span-1 bg-gray-200 dark:bg-gray-800 border-r-2 border-gray-800 dark:border-gray-200">
				<div className="sticky top-0">
					<Navigation />
				</div>
			</aside>
			<main className="col-start-2 col-end-9">{children}</main>
		</div>
	);
}

function useRealtime() {
	const setMetaRealtimeCache = Task.meta.useRealtimeCache();
	const setLogRealtimeCache = Task.log.useRealtimeCache();

	useEffect(() => {
		let taskMetaFn: Realtime.onMetaFn;
		let taskLogFn: Realtime.onLogFn;
		SignalR.start().then(() => {
			taskMetaFn = SignalR.onTaskMeta(null, setMetaRealtimeCache);
			taskLogFn = SignalR.onTaskLog(null, setLogRealtimeCache);
		});
		return () => {
			if (typeof taskMetaFn === 'function') {
				SignalR.offTaskMeta(taskMetaFn);
			}
			if (typeof taskLogFn === 'function') {
				SignalR.offTaskLog(taskLogFn);
			}
			SignalR.stop();
		};
	}, []);
}

function AppContent() {
	useRealtime();

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
