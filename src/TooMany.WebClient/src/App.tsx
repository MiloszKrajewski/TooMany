import { useEffect } from 'react';
import type { ReactNode } from 'react';
import { QueryClient, QueryClientProvider } from 'react-query';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';

import type * as Realtime from 'types/realtime';

import SignalR from '@tm/SignalR';

import { Home, Define, NotFound } from '@pages/index';
import { Tag as MonitorTag, Task as MonitorTask } from '@pages/monitor';

import Navigation from '@components/navigation';

import { useRealtimeCache as useLogRealtimeCache } from '@hooks/API/Task/log';
import { useRealtimeCache as useMetaRealtimeCache } from '@hooks/API/Task/meta';

function Layout({ children }: { children?: ReactNode }) {
	return (
		<div className=" bg-gray-900 text-white min-h-screen min-w-screen grid grid-cols-8">
			<aside className="col-span-1 bg-gray-800 border-r-2 border-gray-200">
				<div className="sticky top-0">
					<Navigation />
				</div>
			</aside>
			<main className="col-start-2 col-end-9">{children}</main>
		</div>
	);
}

function useRealtime() {
	const setMetaRealtimeCache = useMetaRealtimeCache();
	const setLogRealtimeCache = useLogRealtimeCache();

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

	return (
		<Layout>
			<Routes>
				<Route path="define">
					<Route element={<Define />} />
					<Route path=":name" element={<Define />} />
				</Route>
				<Route path="monitor">
					<Route path="tag/:name" element={<MonitorTag />}></Route>
					<Route path="task/:name" element={<MonitorTask />}></Route>
				</Route>
				<Route path="/" element={<Home />} />
				<Route path="*" element={<NotFound />} />
			</Routes>
		</Layout>
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
