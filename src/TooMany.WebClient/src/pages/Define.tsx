import { useParams } from 'react-router-dom';
import { memo, lazy } from 'react';
import SuspenseQuery from '@components/helpers/SuspenseQuery';

const TaskEditor = lazy(() => import('@components/task/editor'));

const DefinePage = function () {
	const { name } = useParams();
	return (
		<div className="h-full">
			<SuspenseQuery fallback={<h1>Loading Tasks...</h1>}>
				<TaskEditor name={name} />
			</SuspenseQuery>
		</div>
	);
};

export default memo(DefinePage);
