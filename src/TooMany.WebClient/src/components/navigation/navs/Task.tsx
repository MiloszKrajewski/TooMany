import { memo, useMemo } from 'react';
import { useParams } from 'react-router-dom';

import * as routes from '@tm/helpers/routes';

import Link from '@components/link';

import { useMeta } from '@hooks/API/Task/meta';

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

export default function TaskContainer({ parent }: { parent?: string }) {
	const { type, name } = useParams();

	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITask[]>(() => {
		const isTaskAssociated: Record<string, boolean> = {};
		switch (parent) {
			case 'monitor': {
				const isTag = type === 'tag';
				const isTask = type === 'task';

				return metas.map(
					isTag
						? (meta) => {
								for (const tag of meta.tags) {
									if (
										!tag ||
										isTaskAssociated[meta.name] === true
									)
										continue;
									isTaskAssociated[meta.name] = tag === name;
								}
								return {
									name: meta.name,
									sortKey: meta.name.toLowerCase(),
									isSelected: isTask,
									isAssociated:
										isTag && isTaskAssociated[meta.name],
								};
						  }
						: (meta) => TaskTypeMap(name, meta.name),
				);
			}
			case 'define': {
				return metas.map((meta) => TaskTypeMap(name, meta.name));
			}
			default: {
				return metas.map((meta) => ({
					name: meta.name,
					sortKey: meta.name.toLowerCase(),
					isSelected: false,
					isAssociated: false,
				}));
			}
		}
	}, [metas, name, type, parent]);

	const sortedItems = useMemo(() => {
		return items.sort((a, b) => {
			if (a.sortKey < b.sortKey) {
				return -1;
			}
			if (a.sortKey > b.sortKey) {
				return 1;
			}
			return 0;
		});
	}, [items]);

	return <Tasks isLoading={isLoading} tasks={sortedItems} />;
}

const Tasks = memo(
	({ isLoading, tasks }: { isLoading: boolean; tasks: ITask[] }) => {
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
						<Link
							to={routes.monitor({ type: 'task', name: t.name })}
						>
							{t.name}
						</Link>
					</Item>
				))}
			</ul>
		);
	},
);
