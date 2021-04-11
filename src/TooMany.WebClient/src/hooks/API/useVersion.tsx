import { useQuery } from 'react-query';
import useFetcher from '@tm/hooks/useFetcher';

export default function () {
	const fetcher = useFetcher();
	return useQuery<string>(
		'version',
		async () => {
			const result = await fetcher.getRequest<string>(
				`${env.apiV1Url}/version`,
			);
			return result;
		},
		{
			suspense: false,
			initialData: '0.0.0',
		},
	);
}
