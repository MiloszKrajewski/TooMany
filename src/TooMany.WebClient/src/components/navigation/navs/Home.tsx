import Link from '@components/link';
import { Item } from './list';

export default () => {
	return (
		<ul>
			<Item isNoPadding>
				<Link to="/">Home</Link>
			</Item>
		</ul>
	);
};
