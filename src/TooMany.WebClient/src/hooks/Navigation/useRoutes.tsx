export default function () {
	return {
		home: () => '/',
		define: () => '/define',
		redefine: ({ name = ':name' }: { name?: string } = {}) => `/define/${name}`,
		monitor: ({
			type = ':type',
			name = ':name',
		}: {
			type?: string;
			name?: string;
		} = {}) => `/monitor/${type}/${name}`,
	};
}
