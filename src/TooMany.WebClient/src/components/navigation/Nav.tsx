import { HomeNav, TagNav, TaskNav } from './navs';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import { Server, Client } from '@hooks/API';

export default () => {
	const { data: serverVersion } = Server.useVersion();
	const { data: clientVersion } = Client.useVersion();
	return (
		<nav className="pl-1 min-w-full min-h-screen overflow-y-auto">
			<h1>TooMany version - {serverVersion}</h1>
			<h1>Web App version - {clientVersion}</h1>
			<HomeNav />
			<SuspenseQuery>
				<TagNav />
			</SuspenseQuery>
			<SuspenseQuery>
				<TaskNav />
			</SuspenseQuery>
		</nav>
	);
};
