import Link from '@components/link';
import { Item } from './list';
import * as routes from '@tm/helpers/routes';

export default () => {
	return (
		<ul>
			<Item isNoPadding>
				<Link to={routes.home()}>Home</Link>
			</Item>
		</ul>
	);
};
