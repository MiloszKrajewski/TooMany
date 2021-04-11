import { memo } from 'react';
import Link from '@components/link';
import { useRoutes } from '@hooks/Navigation';

function HomePage() {
	const routes = useRoutes();
	return (
		<div className="flex min-h-screen min-w-full pl-52 pt-28">
			<article>
				<h1 className="text-2xl font-bold">Welcome to TooMany!</h1>
				<p>
					TooMany is a task runner inspired by Docker but with a lighter touch.
				</p>
				<br />
				<p>
					To create a new task simply{' '}
					<Link className="text-purple-500" to={routes.define()}>
						click here
					</Link>
					.
				</p>
			</article>
		</div>
	);
}

export default memo(HomePage);
