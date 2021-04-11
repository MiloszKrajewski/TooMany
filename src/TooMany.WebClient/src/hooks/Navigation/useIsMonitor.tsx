import { useMatch } from 'react-router-dom';
import useRoutes from './useRoutes';

export default function () {
	const routes = useRoutes();
	return useMatch(routes.monitor());
}
