import { memo } from 'react';
import { useParams } from 'react-router-dom';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import ScrollToBottom from '@components/helpers/ScrollToBottom';
import { Task as Header } from '@components/monitor/header';
import { Task as Terminal } from '@components/monitor/terminal';

function MonitorPage() {
	const { name } = useParams();
	return (
		<section className="max-h-screen flex flex-col">
			<Header name={name} />
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}>
				<ScrollToBottom>
					<Terminal name={name} />
				</ScrollToBottom>
			</SuspenseQuery>
		</section>
	);
}

export default memo(MonitorPage);
