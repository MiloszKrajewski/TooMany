import { useMemo } from 'react';
import { useMatch } from 'react-router-dom';

import * as routes from '@tm/helpers/routes';

export default function () {
	const definePathMatch = useMatch(routes.define());
	const redefinePathMatch = useMatch(routes.redefine());

	return useMemo(() => {
		const params = definePathMatch?.params || redefinePathMatch?.params;
		if (typeof params === 'undefined') {
			return undefined;
		}
		return { name: params.name };
	}, [definePathMatch, redefinePathMatch]);
}
