export default {
	app: {
		version: () => `${env.apiV1Url}/version`,
	},
	task: {
		list: () => `${env.apiV1Url}/task`,
		create: (task: string) => `${env.apiV1Url}/task/${task}`,
		start: (task: string) => `${env.apiV1Url}/task/${task}/start`,
		restart: (task: string) => `${env.apiV1Url}/task/${task}/start?force=true`,
		stop: (task: string) => `${env.apiV1Url}/task/${task}/stop`,
		delete: (task: string) => `${env.apiV1Url}/task/${task}`,
		meta: (task: string) => `${env.apiV1Url}/task/${task}`,
		logs: (task: string) => `${env.apiV1Url}/task/${task}/logs`,
	},
};
