import { useMemo } from 'react';
import { useMeta } from '@hooks/API/Task/meta';
import Link from '@components/link';
import { Header, Item } from './list';
import { useIsMonitor, useIsDefine } from '@hooks/Navigation';
import * as routes from '@tm/helpers/routes';

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
	const isMonitor = useIsMonitor();
	const isDefine = useIsDefine();

	const { data: metas = [], isLoading } = useMeta();

	const tasks = useMemo<ITask[]>(() => {
		const isTaskAssociated: Record<string, boolean> = {};
		if (isMonitor) {
			const { params } = isMonitor;
			const isTag = params.type === 'tag';
			const isTask = params.type === 'task';
			return metas.map(
				isTag
					? (meta) => {
							for (const tag of meta.tags) {
								if (!tag || isTaskAssociated[meta.name] === true) continue;
								isTaskAssociated[meta.name] = tag === params.name;
							}
							return {
								name: meta.name,
								sortKey: meta.name.toLowerCase(),
								isSelected: isTask,
								isAssociated: isTag && isTaskAssociated[meta.name],
							};
					  }
					: (meta) => TaskTypeMap(params.name, meta.name),
			);
		}
		if (isDefine) {
			const { params } = isDefine;
			return metas.map((meta) => TaskTypeMap(params.name, meta.name));
		}
		return metas.map((meta) => ({
			name: meta.name,
			sortKey: meta.name.toLowerCase(),
			isSelected: false,
			isAssociated: false,
		}));
	}, [metas, isMonitor, isDefine]);

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
	if (!sortedTasks.length) return <ul></ul>;
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
