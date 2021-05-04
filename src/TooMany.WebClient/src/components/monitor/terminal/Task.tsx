import { Task } from '@hooks/API';
import Terminal from '@components/terminal';

export default function ({ name }: { name: string }) {
	const { data: logs = [], isLoading } = Task.log.useLog(name);
	if (isLoading) return null;
	return <Terminal logs={logs} />;
}
