export const getQueryKey = (name?: string) =>
	typeof name === 'undefined' ? ['tasks', 'meta'] : ['tasks', 'meta', name];
