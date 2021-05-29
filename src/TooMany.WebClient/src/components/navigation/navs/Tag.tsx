import { memo, useMemo } from 'react';
import { useParams } from 'react-router-dom';

import * as routes from '@tm/helpers/routes';

import Link from '@components/link';

import { useMeta } from '@hooks/API/Task/meta';

import { useSortedItems } from './hooks';
import { Header, Item } from './list';

interface ITag {
	name: string;
	sortKey: string;
	isSelected: boolean;
	isAssociated: boolean;
}

export default ({ parent }: { parent?: string }) => {
	switch (parent) {
		case 'monitor':
			return <MonitorTags />;
		case 'define':
			return <DefineTags />;
		default:
			return <DefaultTags />;
	}
};

function MonitorTags() {
	const { type, name } = useParams();
	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITag[]>(() => {
		const isTagAssociated: Record<string, boolean> = {};
		const isTag = type === 'tag';
		const isTask = type === 'task';

		let tagNames: string[];
		if (isTask) {
			tagNames = metas.flatMap((meta) => {
				for (const tag of meta.tags) {
					if (!tag || isTagAssociated[tag] === true) continue;
					isTagAssociated[tag] = meta.name === name;
				}
				return meta.tags;
			});
		} else {
			tagNames = metas.flatMap((meta) => meta.tags);
		}

		const uniqueTagNames = new Set(tagNames);

		return Array.from(uniqueTagNames).map((tagName) => ({
			name: tagName,
			sortKey: tagName.toLowerCase(),
			isSelected: isTag && name === tagName,
			isAssociated: isTask && isTagAssociated[tagName],
		}));
	}, [metas, name, type]);

	if (isLoading || !items.length) return null;
	return <Tags items={items} />;
}

function DefineTags() {
	const { name } = useParams();
	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITag[]>(() => {
		const isTagAssociated: Record<string, boolean> = {};
		const uniqueTagNames: Set<string> = new Set(
			metas.flatMap((meta) => {
				for (const tag of meta.tags) {
					if (!tag || isTagAssociated[tag] === true) continue;
					isTagAssociated[tag] = meta.name === name;
				}
				return meta.tags;
			}),
		);
		return Array.from(uniqueTagNames).map((tagName) => ({
			name: tagName,
			sortKey: tagName.toLowerCase(),
			isSelected: name === tagName,
			isAssociated: isTagAssociated[tagName],
		}));
	}, [metas, name]);

	if (isLoading || !items.length) return null;
	return <Tags items={items} />;
}

function DefaultTags() {
	const { name } = useParams();
	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITag[]>(() => {
		const uniqueTagNames: Set<string> = new Set(
			metas.flatMap((meta) => meta.tags),
		);
		return Array.from(uniqueTagNames).map((tagName) => ({
			name: tagName,
			sortKey: tagName.toLowerCase(),
			isSelected: false,
			isAssociated: false,
		}));
	}, [metas, name]);

	if (isLoading || !items.length) return null;
	return <Tags items={items} />;
}

const Tags = memo(({ items }: { items: ITag[] }) => {
	const sortedItems = useSortedItems<ITag>(items);
	return (
		<ul>
			<Header>Tags</Header>
			{sortedItems.map((item) => (
				<Item
					isSelected={item.isSelected}
					isAssociated={item.isAssociated}
					key={item.name}
				>
					<Link
						to={routes.monitor({ type: 'tags', name: item.name })}
					>
						{item.name}
					</Link>
				</Item>
			))}
		</ul>
	);
});
