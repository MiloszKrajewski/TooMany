import { HomeNav, TagNav, TaskNav } from './navs';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import { useVersion } from '@hooks/API';

export default () => {
	const { data: version = '0.0.0' } = useVersion();
	return (
		<nav className="pl-1 min-w-full min-h-screen overflow-y-auto">
			<h1>v{version}</h1>
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
