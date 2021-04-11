import { useMemo } from 'react';
import * as Task from '@hooks/API/Task';
import { Link } from 'react-router-dom';
import { Header, Item } from './list';
import * as Navigation from '@hooks/Navigation';

interface ITask {
	name: string;
	isSelected: boolean;
	isAssociated: boolean;
}

export default () => {
	const isTerminal = Navigation.useIsTerminal();
	const isEditor = Navigation.useIsEditor();

	const { data: allTasks = [], isLoading } = Task.useAllTasks();

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
								isSelected: isTask,
								isAssociated: isTag && isTaskAssociated[task.name],
							};
					  }
					: (task) => ({
							name: task.name,
							isSelected: params.name === task.name,
							isAssociated: false,
					  }),
			);
		}
		if (isEditor) {
			const { params } = isEditor;
			return allTasks.map((task) => ({
				name: task.name,
				isSelected: params.name === task.name,
				isAssociated: false,
			}));
		}
		return allTasks.map((task) => ({
			name: task.name,
			isSelected: false,
			isAssociated: false,
		}));
	}, [allTasks, isTerminal, isEditor]);

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
