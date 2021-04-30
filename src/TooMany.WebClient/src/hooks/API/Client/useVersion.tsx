import { useQuery } from 'react-query';

export default function () {
	return useQuery<string>(['client', 'version'], () => env.version, {
		suspense: false,
		initialData: '0.0.0',
	});
}
