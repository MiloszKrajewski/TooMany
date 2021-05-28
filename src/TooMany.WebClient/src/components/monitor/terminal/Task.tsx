import { useParams } from 'react-router-dom';

import Terminal from '@components/terminal';

import { useLog } from '@hooks/API/Task/log';

export default function () {
	const { name } = useParams();
	const { data: logs = [], isLoading } = useLog(name);
	if (isLoading) return null;
	return <Terminal logs={logs} />;
}
