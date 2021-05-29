import { useParams, Route } from 'react-router-dom';

import Tag from './Tag';
import Task from './Task';

export default function () {
	const params = useParams();
	switch (params.type) {
		case 'tags':
			return <Route element={<Tag />} />;
		case 'task':
			return <Route element={<Task />} />;
		default:
			return null;
	}
}
