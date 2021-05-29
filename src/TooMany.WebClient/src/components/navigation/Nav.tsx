import SuspenseQuery from '@components/helpers/SuspenseQuery';

import { HomeNav, TagNav, TaskNav } from './navs';

export default ({ parent }: { parent?: string }) => {
	return (
		<nav className="pl-1 min-w-full min-h-screen overflow-y-auto">
			<HomeNav />
			<SuspenseQuery>
				<TagNav parent={parent} />
			</SuspenseQuery>
			<SuspenseQuery>
				<TaskNav parent={parent} />
			</SuspenseQuery>
		</nav>
	);
};
