import Link from '@components/link';
import * as routes from '@tm/helpers/routes';
import { useVersion as useServerVersion } from '@hooks/API/Server';
import { useVersion as useClientVersion } from '@hooks/API/Client';
import { useScreenType } from '@hooks/index';

export default function () {
	const screenType = useScreenType();
	const { data: serverVersion } = useServerVersion();
	const { data: clientVersion } = useClientVersion();

	let layoutClassName = '';
	let sectionClassName = '';
	let articleClassName = 'mt-4';
	if (screenType.isPortrait) {
		layoutClassName = 'flex-col align-baseline p-10';
		sectionClassName = '';
	} else {
		layoutClassName = 'flex-row justify-center px-10';
		sectionClassName = 'w-6/12 my-4 mx-8';
	}

	return (
		<div className={`flex min-h-screen min-w-full ${layoutClassName}`}>
			<section className={sectionClassName}>
				<article className={articleClassName}>
					<h1 className="text-2xl underline mb-4">Welcome</h1>
					<p className="text-l">
						TooMany is a task runner inspired by Docker but with a lighter
						touch.
					</p>
					<p>
						To create a new task simply&nbsp;
						<Link className="text-purple-500" to={routes.define()}>
							click here
						</Link>
						.
					</p>
				</article>
				<article className={articleClassName}>
					<h1 className="text-2xl underline mb-4">Developers</h1>
					<ul>
						<li className="my-4 flex items-center flex-auto">
							<img
								className="rounded-lg"
								src="https://avatars.githubusercontent.com/u/4333436?v=4"
								loading="lazy"
								width="54"
								height="54"
							/>
							<p className="pl-4">
								Primary Developer:{' '}
								<a
									rel="noreferrer noopener"
									className="text-purple-500"
									href="https://github.com/MiloszKrajewski"
								>
									Milosz Krajewski
								</a>
							</p>
						</li>
						<li className="my-4 flex items-center flex-auto">
							<img
								className="rounded-lg"
								src="https://avatars.githubusercontent.com/u/22887074?v=4"
								loading="lazy"
								width="54"
								height="54"
							/>
							<p className="pl-4">
								Frontend Developer:&nbsp;
								<a
									className="text-purple-500"
									rel="noreferrer noopener"
									href="https://github.com/jonathonhawkins92"
								>
									Jonathon Hawkins
								</a>
							</p>
						</li>
					</ul>
				</article>
				<article className={articleClassName}>
					<h1 className="text-2xl underline mb-4">Licence</h1>
					<p>
						TooMany is released under the&nbsp;
						<a
							className="text-purple-500"
							rel="noreferrer noopener"
							href="https://github.com/MiloszKrajewski/TooMany/blob/main/LICENSE"
						>
							MIT License
						</a>
						.
					</p>
					<p>
						A short and simple permissive license with conditions only requiring
						preservation of copyright and license notices. Licensed works,
						modifications, and larger works may be distributed under different
						terms and without source code.
					</p>
				</article>
			</section>
			<section className={sectionClassName}>
				<article className={articleClassName}>
					<h1 className="text-2xl underline mb-4">Version</h1>
					<h1>TooMany - {serverVersion}</h1>
					<h1>Client - {clientVersion}</h1>
				</article>
				<article className={articleClassName}>
					<h1 className="text-2xl underline mb-4">Dependencies</h1>
					<ul className=" pl-5">
						{env.dependencies.map((name) => (
							<li className="my-1 list-disc" key={name}>
								{name}
							</li>
						))}
					</ul>
				</article>
			</section>
		</div>
	);
}
