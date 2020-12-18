/* eslint-disable no-console */
import {
	onGlobalSetup,
	provide,
	ref,
	useContext,
} from '@nuxtjs/composition-api';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';

let instance: any;
function getSignalR(realtimeUrl: string) {
	if (!instance) {
		instance = new HubConnectionBuilder()
			.configureLogging(LogLevel.Debug)
			.withUrl(realtimeUrl)
			.build();
	}
	return instance;
}

interface Task {
	outputs: string[];
}

type Tasks = Record<string, Task>;

interface Ref<T> {
	value: T;
}

const defaultTask = Object.freeze({
	outputs: [],
});

export default () => {
	onGlobalSetup(() => {
		const Tasks: Ref<Tasks> = ref({});

		const { env } = useContext();
		const signalR = getSignalR(env.realtimeUrl);

		async function start() {
			try {
				console.log('SignalR Connecting.');
				await signalR.start();
				console.log('SignalR Connected.');
			} catch (ex) {
				console.error('SignalR connection exception:', ex);
			}
		}

		signalR.on('Log', (task: string, data: string) => {
			console.log('Log', { task, data });

			if (!Tasks.value[task]) Tasks.value[task] = { ...defaultTask };
			Tasks.value[task].outputs.push(data);

			provide('Task.Log', { task, data: Tasks.value[task] });
		});

		signalR.on('Task', (task: string, data: string) => {
			console.log('Task', { task, data });

			if (!Tasks.value[task]) Tasks.value[task] = { ...defaultTask };
			Tasks.value[task].outputs.push(data);

			provide('Task.Data', { task, data: Tasks.value[task] });
		});

		signalR.onclose(start);
		start();
	});
};
