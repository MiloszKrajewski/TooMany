import { memo, useMemo } from 'react';

import * as routes from '@tm/helpers/routes';

import Link from '@components/link';

import { useMeta } from '@hooks/API/Task/meta';
import { useDefineParams, useMonitorParams } from '@hooks/Navigation';

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

export default () => {
	const monitorParams = useMonitorParams();
	const defineParams = useDefineParams();
	if (typeof monitorParams !== 'undefined') {
		return (
			<MonitorTasks name={monitorParams.name} type={monitorParams.type} />
		);
	} else if (typeof defineParams !== 'undefined') {
		return <DefineTasks name={defineParams.name} />;
	} else {
		return <DefaultTasks />;
	}
};

function MonitorTasks({ type, name }: { type: string; name: string }) {
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

function DefineTasks({ name }: { name: string }) {
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
