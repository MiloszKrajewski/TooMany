export const home = () => '/';
export const define = () => '/define';
export const redefine = ({ name = ':name' }: { name?: string } = {}) =>
	`/define/${name}`;
export const monitor = ({
	type = ':type',
	name = ':name',
}: {
	type?: string;
	name?: string;
} = {}) => `/monitor/${type}/${name}`;
