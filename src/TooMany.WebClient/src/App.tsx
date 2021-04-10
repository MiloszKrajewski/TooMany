import './App.css';
import Nav from '@components/navigation/Nav';
import Form from '@components/task/editor/Form';

import SignalR from './SignalR';
import { useEffect } from 'react';
import { QueryClient, QueryClientProvider } from 'react-query';
import useTaskList from '@hooks/API/useTaskList';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import useScreenType from '@hooks/useScreenType';

function Home() {
	const { data = [], isLoading } = useTaskList();
	if (isLoading) {
		return <h1>loading...</h1>;
	}
	const {
		name,
		executable,
		arguments: args,
		directory,
		environment,
		tags,
	} = data[0];
	return (
		<div>
			<Form
				name={name}
				executable={executable}
				arguments={args}
				directory={directory}
				envVars={environment}
				tags={tags}
			/>
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
	const list = useTaskList();
	console.log(list);

	const screenType = useScreenType();
	console.log(list);

	return (
		<div className="bg-white text-gray-900 dark:bg-gray-900 dark:text-white min-h-screen min-w-screen grid grid-cols-8">
			<aside className="col-span-1 dark:bg-gray-500 bg-opacity-40">
				<header>
					<button>create</button>
				</header>
				<Nav />
			</aside>
			<main className="col-start-2 col-end-9">
				<Routes>
					<Route path="/" element={<Home />} />
				</Routes>
			</main>
		</div>
	);
}

const queryClient = new QueryClient();
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
