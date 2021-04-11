import { useMemo } from 'react';
import * as Task from '@hooks/API/Task';
import Link from '@components/link';
import { Header, Item } from './list';
import * as Navigation from '@hooks/Navigation';
import { useRoutes } from '@hooks/Navigation';

interface ITask {
	name: string;
	sortKey: string;
	isSelected: boolean;
	isAssociated: boolean;
}

function TaskTypeMap(selectedName: string, name: string) {
	return {
		name: name,
		sortKey: name.toLowerCase(),
		isSelected: selectedName === name,
		isAssociated: false,
	};
}

export default () => {
	const isMonitor = Navigation.useIsMonitor();
	const isDefine = Navigation.useIsDefine();

	const { data: allTasks = [], isLoading } = Task.useAll();

	const tasks = useMemo<ITask[]>(() => {
		const isTaskAssociated: Record<string, boolean> = {};
		if (isMonitor) {
			const { params } = isMonitor;
			const isTag = params.type === 'tag';
			const isTask = params.type === 'task';
			return allTasks.map(
				isTag
					? (task) => {
							for (const tag of task.tags) {
								if (!tag || isTaskAssociated[task.name] === true) continue;
								isTaskAssociated[task.name] = tag === params.name;
							}
							return {
								name: task.name,
								sortKey: task.name.toLowerCase(),
								isSelected: isTask,
								isAssociated: isTag && isTaskAssociated[task.name],
							};
					  }
					: (task) => TaskTypeMap(params.name, task.name),
			);
		}
		if (isDefine) {
			const { params } = isDefine;
			return allTasks.map((task) => TaskTypeMap(params.name, task.name));
		}
		return allTasks.map((task) => ({
			name: task.name,
			sortKey: task.name.toLowerCase(),
			isSelected: false,
			isAssociated: false,
		}));
	}, [allTasks, isMonitor, isDefine]);

	const sortedTasks = useMemo(
		() =>
			tasks.sort((a, b) => {
				if (a.sortKey < b.sortKey) {
					return -1;
				}
				if (a.sortKey > b.sortKey) {
					return 1;
				}
				return 0;
			}),
		[tasks],
	);

	const routes = useRoutes();

	if (isLoading) return <ul></ul>;
	if (!tasks.length) return <ul></ul>;
	return (
		<ul>
			<Header>Tasks</Header>
			{sortedTasks.map((t) => (
				<Item
					isSelected={t.isSelected}
					isAssociated={t.isAssociated}
					key={t.name}
				>
					<Link to={routes.monitor({ type: 'task', name: t.name })}>
						{t.name}
					</Link>
				</Item>
			))}
		</ul>
	);
};
