import { useMatch } from 'react-router-dom';

import * as routes from '@tm/helpers/routes';

export default function () {
	return useMatch(routes.monitor());
}
