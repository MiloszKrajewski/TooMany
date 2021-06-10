import { useParams } from 'react-router-dom';

import SuspenseQuery from '@components/helpers/SuspenseQuery';
import { Tag as Header } from '@components/monitor/header';
import { Tag as Terminal } from '@components/monitor/terminal';

export default function () {
	const { name } = useParams();
	return (
		<section className="h-screen flex flex-col">
			<Header />
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}>
				<Terminal name={name} />
			</SuspenseQuery>
		</section>
	);
}
