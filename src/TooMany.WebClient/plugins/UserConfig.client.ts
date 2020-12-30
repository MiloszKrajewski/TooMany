/* eslint-disable no-console */
import { Plugin } from '@nuxt/types';
import { reactive } from '@nuxtjs/composition-api';

interface ITask {
	executable: string;
	directory: string;
}

enum localStorageKeys {
	taskConfig = 'taskConfig',
}

interface IUserConfig {
	setTaskConfig(payload: ITask): void;
	getTaskConfig(): void;
	task: ITask;
}

function useUserConfig(): IUserConfig {
	const task = reactive({
		executable: '',
		directory: '',
	});

	function setTaskConfig(payload: ITask) {
		task.executable = payload.executable || '';
		task.directory = payload.directory || '';

		localStorage.setItem(localStorageKeys.taskConfig, JSON.stringify(payload));
	}

	function getTaskConfig() {
		const serializedTaskConfig = localStorage.getItem(
			localStorageKeys.taskConfig,
		);
		if (!serializedTaskConfig) {
			return;
		}
		const deserializedTaskConfig = JSON.parse(serializedTaskConfig);
		task.executable = deserializedTaskConfig.executable || '';
		task.directory = deserializedTaskConfig.directory || '';
	}

	return {
		setTaskConfig,
		getTaskConfig,
		task,
	};
}

declare module 'vue/types/vue' {
	interface Vue {
		$UserConfig: IUserConfig;
	}
}

declare module '@nuxt/types' {
	interface NuxtAppOptions {
		$UserConfig: IUserConfig;
	}
	interface Context {
		$UserConfig: IUserConfig;
	}
}

const $UserConfig: Plugin = (_, inject) => {
	const userConfig = useUserConfig();
	userConfig.getTaskConfig();

	inject('UserConfig', userConfig);
};

export default $UserConfig;
