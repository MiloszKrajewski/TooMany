import { lazy } from 'react';

import SuspenseQuery from '@components/helpers/SuspenseQuery';

const TaskEditor = lazy(() => import('@components/task/editor'));

export default function () {
	return (
		<div className="h-full">
			<SuspenseQuery fallback={<h1>Loading Tasks...</h1>}>
				<TaskEditor />
			</SuspenseQuery>
		</div>
	);
}
