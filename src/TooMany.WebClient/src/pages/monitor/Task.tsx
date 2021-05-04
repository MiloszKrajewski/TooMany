import { memo } from 'react';
import { useParams } from 'react-router-dom';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import ScrollToBottom from '@components/helpers/ScrollToBottom';
import { TaskTerminal } from '@components/terminal';
import { Header } from '@components/monitor';

function MonitorPage() {
	const { name } = useParams();
	return (
		<section className="max-h-screen flex flex-col">
			<Header.Task name={name} />
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}>
				<ScrollToBottom>
					<TaskTerminal name={name} />
				</ScrollToBottom>
			</SuspenseQuery>
		</section>
	);
}

export default memo(MonitorPage);
