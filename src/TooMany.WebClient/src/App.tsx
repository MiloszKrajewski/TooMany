import { useEffect } from 'react';
import { QueryClient, QueryClientProvider } from 'react-query';
import { ReactQueryDevtools } from 'react-query/devtools';
import {
	BrowserRouter as Router,
	Routes,
	Route,
	Outlet,
} from 'react-router-dom';

import type * as Realtime from 'types/realtime';

import SignalR from '@tm/SignalR';

import { Home, Define, NotFound, Monitor } from '@pages/index';

import Navigation from '@components/navigation';

import { useRealtimeCache as useMetaRealtimeCache } from '@hooks/API/Task/meta';

import { noop } from '@helpers/general';

function Layout({ parent }: { parent?: string }) {
	return (
		<div className=" bg-gray-900 text-white min-h-screen min-w-screen grid grid-cols-8">
			<aside className="col-span-1 bg-gray-800 border-r-2 border-gray-200">
				<div className="sticky top-0">
					<Navigation parent={parent} />
				</div>
			</aside>
			<main className="col-start-2 col-end-9">
				<Outlet />
			</main>
		</div>
	);
}

function useRealtime() {
	const setMetaRealtimeCache = useMetaRealtimeCache();

	useEffect(() => {
		let taskMetaFn: Realtime.onMetaFn;
		let taskLogFn: Realtime.onLogFn;
		SignalR.start().then(() => {
			taskMetaFn = SignalR.onTaskMeta(null, setMetaRealtimeCache);
			taskLogFn = SignalR.onTaskLog(null, noop); // SR moans like crazy if there is no handler
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

	return (
		<Routes>
			<Route path="define">
				<Route element={<Layout />}>
					<Route element={<Define />} />
					<Route path=":name" element={<Define />} />
				</Route>
			</Route>
			<Route
				path="/monitor/:type/:name"
				element={<Layout parent="monitor" />}
			>
				<Route element={<Monitor />} />
			</Route>
			<Route path="/" element={<Home />} />
			<Route path="*" element={<NotFound />} />
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
			<ReactQueryDevtools initialIsOpen={false} />
		</QueryClientProvider>
	);
}

export default App;
