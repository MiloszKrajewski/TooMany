import { useQuery } from 'react-query';
import useFetcher from '@tm/hooks/useFetcher';

export default function () {
	const fetcher = useFetcher();
	return useQuery<number>(
		'version',
		async () => {
			const result = await fetcher.getRequest<number>(
				`${env.apiV1Url}/version`,
			);
			return result;
		},
		{
			suspense: false,
		},
	);
}
