import { useMemo } from 'react';
import * as Task from '@hooks/API/Task';
import { Link } from 'react-router-dom';
import { Header, Item } from './list';
import { useParams } from 'react-router-dom';

interface ITask {
	name: string;
	isSelected: boolean;
	isAssociated: boolean;
}

export default () => {
	const { type, name } = useParams();
	const { data: allTasks = [], isLoading } = Task.useAllTasks();

	const tasks = useMemo<ITask[]>(() => {
		const isTaskAssociated: Record<string, boolean> = {};
		return allTasks.map((task) => {
			if (type === 'tag') {
				for (const tag of task.tags) {
					if (!tag) continue;
					isTaskAssociated[task.name] = tag === name;
				}
			}
			return {
				name: task.name,
				isSelected: type === 'task' && name === task.name,
				isAssociated: type === 'tag' && isTaskAssociated[task.name],
			};
		});
	}, [allTasks, name, type]);

	if (isLoading) return <ul></ul>;
	if (!tasks.length) return <ul></ul>;
	return (
		<ul>
			<Header>Tasks</Header>
			{tasks.map((t) => (
				<Item
					isSelected={t.isSelected}
					isAssociated={t.isAssociated}
					key={t.name}
				>
					<Link to={`/terminal/task/${t.name}`}>{t.name}</Link>
				</Item>
			))}
		</ul>
	);
};
