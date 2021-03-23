import React, { useState } from 'react';
import './App.css';
import Nav from '@components/navigation/Nav';
import logo from './assets/icon.png';

function App() {
	const [count, setCount] = useState(0);

	return (
		<div className="bg-white dark:bg-gray-800">
			<div className="p-6 max-w-sm mx-auto bg-white rounded-xl shadow-md flex items-center space-x-4">
				<div className="flex-shrink-0">
					<img className="h-12 w-12" src={logo} alt="ChitChat Logo" />
				</div>
				<div>
					<div className="text-xl font-medium text-black">ChitChat</div>
					<p className="text-gray-500">You have a new message!</p>
				</div>
			</div>
			<Nav />
			<header className="App-header">
				<p>Hello Vite + React!</p>
				<p>
					<button onClick={() => setCount((count) => count + 1)}>
						count is: {count}
					</button>
				</p>
				<p>
					Edit <code>App.tsx</code> and save to test HMR updates.
				</p>
				<p>
					<a
						className="App-link"
						href="https://reactjs.org"
						target="_blank"
						rel="noopener noreferrer"
					>
						Learn React
					</a>
					{' | '}
					<a
						className="App-link"
						href="https://vitejs.dev/guide/features.html"
						target="_blank"
						rel="noopener noreferrer"
					>
						Vite Docs
					</a>
				</p>
			</header>
		</div>
	);
}

export default App;
