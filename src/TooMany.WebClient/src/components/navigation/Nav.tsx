import SuspenseQuery from '@components/helpers/SuspenseQuery';

import { HomeNav, TagNav, TaskNav } from './navs';

export default () => {
	return (
		<nav className="pl-1 min-w-full min-h-screen overflow-y-auto">
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
