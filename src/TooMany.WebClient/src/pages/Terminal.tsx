import { memo } from 'react';
import { useParams } from 'react-router-dom';
import SuspenseQuery from '@components/helpers/SuspenseQuery';

function Terminal() {
	const params = useParams();
	return (
		<section>
			<header>
				<h3>{params.taskName}</h3>
			</header>
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}></SuspenseQuery>
		</section>
	);
}

export default memo(Terminal);
