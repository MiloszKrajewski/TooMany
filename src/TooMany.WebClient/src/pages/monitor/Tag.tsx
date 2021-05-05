import SuspenseQuery from '@components/helpers/SuspenseQuery';
import ScrollToBottom from '@components/helpers/ScrollToBottom';
import { Tag as Header } from '@components/monitor/header';
import { Tag as Terminal } from '@components/monitor/terminal';

export default function () {
	return (
		<section className="max-h-screen flex flex-col">
			<Header />
			<SuspenseQuery fallback={<h1>Loading Terminal...</h1>}>
				<ScrollToBottom>
					<Terminal />
				</ScrollToBottom>
			</SuspenseQuery>
		</section>
	);
}
