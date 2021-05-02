import { useQuery } from 'react-query';
import useApi from '../useApi';

export default function () {
	const api = useApi();
	return useQuery<string>(['server', 'version'], () => api.app.version(), {
		suspense: false,
		initialData: '0.0.0',
	});
}
