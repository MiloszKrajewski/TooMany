import { memo, useMemo } from 'react';
import { useParams } from 'react-router-dom';

import * as routes from '@tm/helpers/routes';

import Link from '@components/link';

import { useMeta } from '@hooks/API/Task/meta';

import { useSortedItems } from './hooks';
import { Header, Item } from './list';

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

export default ({ parent }: { parent?: string }) => {
	switch (parent) {
		case 'monitor':
			return <MonitorTasks />;
		case 'define':
			return <DefineTasks />;
		default:
			return <DefaultTasks />;
	}
};

function MonitorTasks() {
	const { type, name } = useParams();

	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITask[]>(() => {
		const isTaskAssociated: Record<string, boolean> = {};
		const isTag = type === 'tag';
		const isTask = type === 'task';

		return metas.map(
			isTag
				? (meta) => {
						for (const tag of meta.tags) {
							if (!tag || isTaskAssociated[meta.name] === true)
								continue;
							isTaskAssociated[meta.name] = tag === name;
						}
						return {
							name: meta.name,
							sortKey: meta.name.toLowerCase(),
							isSelected: isTask,
							isAssociated: isTag && isTaskAssociated[meta.name],
						};
				  }
				: (meta) => TaskTypeMap(name, meta.name),
		);
	}, [metas, name, type]);

	if (isLoading || !items.length) return null;
	return <Tasks items={items} />;
}

function DefineTasks() {
	const { name } = useParams();

	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITask[]>(() => {
		return metas.map((meta) => TaskTypeMap(name, meta.name));
	}, [metas, name]);

	if (isLoading || !items.length) return null;
	return <Tasks items={items} />;
}

function DefaultTasks() {
	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITask[]>(() => {
		return metas.map((meta) => ({
			name: meta.name,
			sortKey: meta.name.toLowerCase(),
			isSelected: false,
			isAssociated: false,
		}));
	}, [metas]);

	if (isLoading || !items.length) return null;
	return <Tasks items={items} />;
}

const Tasks = memo(({ items }: { items: ITask[] }) => {
	const sortedItems = useSortedItems<ITask>(items);
	return (
		<ul>
			<Header>Tasks</Header>
			{sortedItems.map((item) => (
				<Item
					isSelected={item.isSelected}
					isAssociated={item.isAssociated}
					key={item.name}
				>
					<Link
						to={routes.monitor({ type: 'task', name: item.name })}
					>
						{item.name}
					</Link>
				</Item>
			))}
		</ul>
	);
});
