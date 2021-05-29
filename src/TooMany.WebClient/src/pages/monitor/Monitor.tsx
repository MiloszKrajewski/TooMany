import { useParams } from 'react-router-dom';

import Tag from './Tag';
import Task from './Task';

export default function () {
	const params = useParams();
	switch (params.type) {
		case 'tags':
			return <Tag />;
		case 'task':
			return <Task />;
		default:
			return null;
	}
}
