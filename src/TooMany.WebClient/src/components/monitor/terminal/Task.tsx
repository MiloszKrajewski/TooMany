import { Task } from '@hooks/API';
import Terminal from '@components/terminal';
import { useParams } from 'react-router-dom';

export default function () {
	const { name } = useParams();
	const { data: logs = [], isLoading } = Task.log.useLog(name);
	if (isLoading) return null;
	return <Terminal logs={logs} />;
}
