import { useMatch } from 'react-router-dom';

export default function () {
	return useMatch('/editor/:name');
}
