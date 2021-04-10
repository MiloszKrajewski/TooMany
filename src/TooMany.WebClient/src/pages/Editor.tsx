import { useParams } from 'react-router-dom';
import { memo, lazy } from 'react';
import SuspenseQuery from '@components/helpers/SuspenseQuery';

const Form = lazy(() => import('@components/task/editor/Form'));

const Editor = function () {
	const params = useParams();
	console.log(params);
	return (
		<SuspenseQuery fallback={<h1>Loading Tasks...</h1>}>
			<Form name={params.name} />
		</SuspenseQuery>
	);
};

export default memo(Editor);
