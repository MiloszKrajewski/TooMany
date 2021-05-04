import { memo } from 'react';
import { useParams } from 'react-router-dom';
import SuspenseQuery from '@components/helpers/SuspenseQuery';
import ScrollToBottom from '@components/helpers/ScrollToBottom';
import { TagTerminal } from '@components/terminal';
import { Header } from '@components/monitor';

function MonitorTagPage() {
	const { name } = useParams();
	return (
		<section className="max-h-screen flex flex-col">
			<Header.Tag name={name} />
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}>
				<ScrollToBottom>
					<TagTerminal name={name} />
				</ScrollToBottom>
			</SuspenseQuery>
		</section>
	);
}

export default memo(MonitorTagPage);
