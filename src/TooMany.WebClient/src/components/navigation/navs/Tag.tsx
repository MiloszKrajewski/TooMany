import { useMemo } from 'react';
import * as Task from '@hooks/API/Task';
import Link from '@components/link';
import { Header, Item } from './list';
import * as Navigation from '@hooks/Navigation';
import { useRoutes } from '@hooks/Navigation';

interface ITag {
	name: string;
	isSelected: boolean;
	isAssociated: boolean;
}

export default () => {
	const isMonitor = Navigation.useIsMonitor();
	const isDefine = Navigation.useIsDefine();

	const { data: allTasks = [], isLoading } = Task.useAll();

	const tags = useMemo<ITag[]>(() => {
		const isTagAssociated: Record<string, boolean> = {};

		if (isMonitor) {
			const { params } = isMonitor;
			const isTag = params.type === 'tag';
			const isTask = params.type === 'task';
			const uniqueTagNames: Set<string> = new Set(
				allTasks.flatMap(
					isTask
						? (task) => {
								for (const tag of task.tags) {
									if (!tag || isTagAssociated[tag] === true) continue;
									isTagAssociated[tag] = task.name === params.name;
								}
								return task.tags;
						  }
						: (task) => task.tags,
				),
			);
			return Array.from(uniqueTagNames).map((tagName) => ({
				name: tagName,
				isSelected: isTag && params.name === tagName,
				isAssociated: isTask && isTagAssociated[tagName],
			}));
		}
		if (isDefine) {
			const { params } = isDefine;
			const uniqueTagNames: Set<string> = new Set(
				allTasks.flatMap((task) => {
					for (const tag of task.tags) {
						if (!tag || isTagAssociated[tag] === true) continue;
						isTagAssociated[tag] = task.name === params.name;
					}
					return task.tags;
				}),
			);
			return Array.from(uniqueTagNames).map((tagName) => ({
				name: tagName,
				isSelected: params.name === tagName,
				isAssociated: isTagAssociated[tagName],
			}));
		}
		const uniqueTagNames: Set<string> = new Set(
			allTasks.flatMap((task) => task.tags),
		);
		return Array.from(uniqueTagNames).map((tagName) => ({
			name: tagName,
			isSelected: false,
			isAssociated: false,
		}));
	}, [allTasks, isMonitor, isDefine]);

	const routes = useRoutes();

	if (isLoading) return <ul></ul>;
	if (!tags.length) return <ul></ul>;
	return (
		<ul>
			<Header>Tags</Header>
			{tags.map((t) => (
				<Item
					key={t.name}
					isSelected={t.isSelected}
					isAssociated={t.isAssociated}
				>
					<Link to={routes.monitor({ type: 'tag', name: t.name })}>
						{t.name}
					</Link>
				</Item>
			))}
		</ul>
	);
};
