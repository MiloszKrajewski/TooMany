import { useMatch } from 'react-router-dom';

export default function () {
	return useMatch('/terminal/:type/:name');
}
