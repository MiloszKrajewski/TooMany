import { useMatch } from 'react-router-dom';
import useRoutes from './useRoutes';

export default function () {
	const routes = useRoutes();
	const isDefine = useMatch(routes.define());
	const isRedefine = useMatch(routes.redefine());
	return isDefine || isRedefine;
}
