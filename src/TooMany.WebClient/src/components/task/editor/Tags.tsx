import { memo, useRef } from 'react';

const Tags = ({
	count = 0,
	tags = [],
}: {
	count?: number;
	tags?: string[];
}) => {
	const initialTags = useRef(tags);
	if (count === 0) return null;
	return (
		<fieldset id="tags" className="flex flex-col">
			{new Array(count).fill(null).map((_, index) => {
				const tag = initialTags.current[index] || '';
				const id = `tag${index}`;
				return (
					<label htmlFor={id} key={id}>
						value:
						<input
							className="text-gray-900"
							id={id}
							defaultValue={tag}
							type="text"
						/>
					</label>
				);
			})}
		</fieldset>
	);
};
export default memo(Tags);
