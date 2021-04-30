import { useMemo } from 'react';
import { Task } from '@hooks/API';
import Link from '@components/link';
import { Header, Item } from './list';
import * as Navigation from '@hooks/Navigation';
import { useRoutes } from '@hooks/Navigation';

interface ITag {
	name: string;
	sortKey: string;
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
				sortKey: tagName.toLowerCase(),
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
				sortKey: tagName.toLowerCase(),
				isSelected: params.name === tagName,
				isAssociated: isTagAssociated[tagName],
			}));
		}
		const uniqueTagNames: Set<string> = new Set(
			allTasks.flatMap((task) => task.tags),
		);
		return Array.from(uniqueTagNames).map((tagName) => ({
			name: tagName,
			sortKey: tagName.toLowerCase(),
			isSelected: false,
			isAssociated: false,
		}));
	}, [allTasks, isMonitor, isDefine]);

	const sortedTags = useMemo(
		() =>
			tags.sort((a, b) => {
				if (a.sortKey < b.sortKey) {
					return -1;
				}
				if (a.sortKey > b.sortKey) {
					return 1;
				}
				return 0;
			}),
		[tags],
	);

	const routes = useRoutes();

	if (isLoading) return <ul></ul>;
	if (!tags.length) return <ul></ul>;
	return (
		<ul>
			<Header>Tags</Header>
			{sortedTags.map((t) => (
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