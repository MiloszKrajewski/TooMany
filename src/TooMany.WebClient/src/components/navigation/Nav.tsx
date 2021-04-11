import { HomeNav, TagNav, TaskNav } from './navs';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import { useVersion } from '@hooks/API';

export default () => {
	const { data: version } = useVersion();
	return (
		<nav className="pl-1 min-w-full min-h-screen overflow-y-auto">
			<h1>TooMany version - {version}</h1>
			<h1>Web App version - {env.version}</h1>
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
