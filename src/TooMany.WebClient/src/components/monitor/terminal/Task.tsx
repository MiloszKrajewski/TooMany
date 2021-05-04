import { useLog } from '@hooks/API/Task/log';
import Terminal from '@components/terminal';
import { useParams } from 'react-router-dom';

export default function () {
	const { name } = useParams();
	const { data: logs = [], isLoading } = useLog(name);
	if (isLoading) return null;
	return <Terminal logs={logs} />;
}
