import { useMemo } from 'react';
import * as Task from '@hooks/API/Task';
import { Link } from 'react-router-dom';
import { Header, Item } from './list';
import * as Navigation from '@hooks/Navigation';

interface ITag {
	name: string;
	isSelected: boolean;
	isAssociated: boolean;
}

export default () => {
	const isTerminal = Navigation.useIsTerminal();
	const isEditor = Navigation.useIsEditor();

	const { data: allTasks = [], isLoading } = Task.useAll();

	const tags = useMemo<ITag[]>(() => {
		const isTagAssociated: Record<string, boolean> = {};

		if (isTerminal) {
			const { params } = isTerminal;
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
		if (isEditor) {
			const { params } = isEditor;
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
	}, [allTasks, isTerminal, isEditor]);

	if (isLoading) return <ul></ul>;
	if (!tags.length) return <ul></ul>;
	return (
		<ul>
			<Header>Tags</Header>
			{tags.map((tag) => (
				<Item
					key={tag.name}
					isSelected={tag.isSelected}
					isAssociated={tag.isAssociated}
				>
					<Link to={`/terminal/tag/${tag.name}`}>{tag.name}</Link>
				</Item>
			))}
		</ul>
	);
};
