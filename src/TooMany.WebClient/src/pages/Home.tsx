import { memo } from 'react';
import { Link } from 'react-router-dom';

function Home() {
	return (
		<div>
			<h1>Welcome to TooMany!</h1>
			<p>
				TooMany is a task runner inspired by Docker but with a lighter touch.
			</p>
			<br />
			<p>
				To create a new task simply{' '}
				<Link to="/editor" className="text-purple-500">
					click here
				</Link>
				.
			</p>
		</div>
	);
}

export default memo(Home);
