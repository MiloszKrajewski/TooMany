import Link from '@components/link';
import * as routes from '@tm/helpers/routes';

export default function () {
	return (
		<div className="flex min-h-screen min-w-full pl-52 pt-28">
			<article>
				<h1 className="text-2xl font-bold">Welcome!</h1>
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
