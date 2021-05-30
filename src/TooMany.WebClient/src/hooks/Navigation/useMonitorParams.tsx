import { useMemo } from 'react';
import { useMatch } from 'react-router-dom';

import * as routes from '@tm/helpers/routes';

export default function () {
	const monitor = useMatch(routes.monitor());

	return useMemo(() => {
		const params = monitor?.params;
		if (typeof params === 'undefined') {
			return undefined;
		}
		return { name: params.name, type: params.type };
	}, [monitor]);
}
