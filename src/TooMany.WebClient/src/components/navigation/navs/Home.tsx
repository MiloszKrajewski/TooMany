import Link from '@components/link';
import { Item } from './list';
import { useRoutes } from '@hooks/Navigation';

export default () => {
	const routes = useRoutes();

	return (
		<ul>
			<Item isNoPadding>
				<Link to={routes.home()}>Home</Link>
			</Item>
		</ul>
	);
};
