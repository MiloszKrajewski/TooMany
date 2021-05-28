import * as routes from '@tm/helpers/routes';

import Link from '@components/link';

export default function () {
	return (
		<div className="flex min-h-screen min-w-full pl-52 pt-28">
			<article>
				<h1 className="text-2xl font-bold">How'd you get here?!</h1>
				<p>
					<Link className="text-purple-500" to={routes.home()}>
						Go home!
					</Link>
				</p>
			</article>
		</div>
	);
}
