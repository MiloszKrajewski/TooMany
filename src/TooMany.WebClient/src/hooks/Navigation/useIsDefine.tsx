import { useMatch } from 'react-router-dom';
import * as routes from '@tm/helpers/routes';

export default function () {
	const isDefine = useMatch(routes.define());
	const isRedefine = useMatch(routes.redefine());
	return isDefine || isRedefine;
}
