import { useMemo } from 'react';
import * as Task from '@hooks/API/Task';
import { Link } from 'react-router-dom';
import { Header, Item } from './list';
import { useParams } from 'react-router-dom';

interface ITag {
	name: string;
	isSelected: boolean;
	isAssociated: boolean;
}

export default () => {
	const { type, name } = useParams();
	const { data: allTasks = [], isLoading } = Task.useAllTasks();

	const tags = useMemo<ITag[]>(() => {
		const isTagAssociated: Record<string, boolean> = {};
		const uniqueTagNames: Set<string> = new Set(
			allTasks.flatMap((task) => {
				if (type === 'task') {
					for (const tag of task.tags) {
						if (!tag) continue;
						isTagAssociated[tag] = task.name === name;
					}
				}
				return task.tags;
			}),
		);
		return Array.from(uniqueTagNames).map((tagName) => ({
			name: tagName,
			isSelected: type === 'tag' && name === tagName,
			isAssociated: type === 'task' && isTagAssociated[tagName],
		}));
	}, [allTasks, name, type]);

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
