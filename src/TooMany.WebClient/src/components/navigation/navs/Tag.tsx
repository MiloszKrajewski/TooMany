import { memo, useMemo } from 'react';
import { useParams } from 'react-router-dom';

import * as routes from '@tm/helpers/routes';

import Link from '@components/link';

import { useMeta } from '@hooks/API/Task/meta';

import { Header, Item } from './list';

interface ITag {
	name: string;
	sortKey: string;
	isSelected: boolean;
	isAssociated: boolean;
}

export default ({ parent }: { parent?: string }) => {
	const { type, name } = useParams();

	const { data: metas = [], isLoading } = useMeta();

	const items = useMemo<ITag[]>(() => {
		const isTagAssociated: Record<string, boolean> = {};

		if (parent === 'monitor') {
			const isTag = type === 'tag';
			const isTask = type === 'task';
			const uniqueTagNames: Set<string> = new Set(
				metas.flatMap(
					isTask
						? (meta) => {
								for (const tag of meta.tags) {
									if (!tag || isTagAssociated[tag] === true)
										continue;
									isTagAssociated[tag] = meta.name === name;
								}
								return meta.tags;
						  }
						: (meta) => meta.tags,
				),
			);
			return Array.from(uniqueTagNames).map((tagName) => ({
				name: tagName,
				sortKey: tagName.toLowerCase(),
				isSelected: isTag && name === tagName,
				isAssociated: isTask && isTagAssociated[tagName],
			}));
		} else if (parent === 'define') {
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
		} else {
			const uniqueTagNames: Set<string> = new Set(
				metas.flatMap((meta) => meta.tags),
			);
			return Array.from(uniqueTagNames).map((tagName) => ({
				name: tagName,
				sortKey: tagName.toLowerCase(),
				isSelected: false,
				isAssociated: false,
			}));
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

	return <Tags isLoading={isLoading} tags={sortedItems} />;
};

const Tags = memo(
	({ isLoading, tags }: { isLoading: boolean; tags: ITag[] }) => {
		if (isLoading) return <ul></ul>;
		if (!tags.length) return <ul></ul>;
		return (
			<ul>
				<Header>Tags</Header>
				{tags.map((t) => (
					<Item
						isSelected={t.isSelected}
						isAssociated={t.isAssociated}
						key={t.name}
					>
						<Link
							to={routes.monitor({ type: 'tags', name: t.name })}
						>
							{t.name}
						</Link>
					</Item>
				))}
			</ul>
		);
	},
);
