import { HomeNav, TagNav, TaskNav } from './navs';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import { useVersion as useServerVersion } from '@hooks/API/Server';
import { useVersion as useClientVersion } from '@hooks/API/Client';

export default () => {
	const { data: serverVersion } = useServerVersion();
	const { data: clientVersion } = useClientVersion();
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
