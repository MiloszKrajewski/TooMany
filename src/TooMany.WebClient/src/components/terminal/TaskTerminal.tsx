import { Task } from '@hooks/API';
import Logs from './Logs';

export default function ({ name }: { name: string }) {
	const { data: logs = [], isLoading } = Task.useLogs(name);
	if (isLoading) return null;
	return <Logs logs={logs} />;
}
