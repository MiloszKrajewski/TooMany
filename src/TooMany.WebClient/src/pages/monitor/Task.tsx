import { useParams } from 'react-router-dom';

import ScrollToBottom from '@components/helpers/ScrollToBottom';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import { Task as Header } from '@components/monitor/header';
import { Task as Terminal } from '@components/monitor/terminal';

function HeaderProxy() {
	const { name } = useParams();
	return <Header name={name} />;
}

export default function () {
	return (
		<section className="max-h-screen flex flex-col">
			<HeaderProxy />
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}>
				<ScrollToBottom>
					<Terminal />
				</ScrollToBottom>
			</SuspenseQuery>
		</section>
	);
}
