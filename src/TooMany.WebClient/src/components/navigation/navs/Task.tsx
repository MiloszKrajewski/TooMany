import { useMemo } from 'react';
import * as Task from '@hooks/API/Task';
import { Link } from 'react-router-dom';
import { Header, Item } from './list';
import * as Navigation from '@hooks/Navigation';

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
	const isTerminal = Navigation.useIsTerminal();
	const isEditor = Navigation.useIsEditor();

	const { data: allTasks = [], isLoading } = Task.useAll();

	const tasks = useMemo<ITask[]>(() => {
		const isTaskAssociated: Record<string, boolean> = {};
		if (isTerminal) {
			const { params } = isTerminal;
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
		if (isEditor) {
			const { params } = isEditor;
			return allTasks.map((task) => TaskTypeMap(params.name, task.name));
		}
		return allTasks.map((task) => ({
			name: task.name,
			sortKey: task.name.toLowerCase(),
			isSelected: false,
			isAssociated: false,
		}));
	}, [allTasks, isTerminal, isEditor]);

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
					<Link to={`/terminal/task/${t.name}`}>{t.name}</Link>
				</Item>
			))}
		</ul>
	);
};
