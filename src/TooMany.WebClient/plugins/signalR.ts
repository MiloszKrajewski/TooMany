import { onGlobalSetup, provide, ref } from '@nuxtjs/composition-api';
import { HubConnectionBuilder } from '@microsoft/signalr';

const signalR = new HubConnectionBuilder().withUrl('/monitor').build();

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
			if (!Tasks.value[task]) Tasks.value[task] = { ...defaultTask };
			Tasks.value[task].outputs.push(data);
			provide('Task.Log', { task, data: Tasks.value[task] });
		});

		signalR.on('Task', (task: string, data: string) => {
			if (!Tasks.value[task]) Tasks.value[task] = { ...defaultTask };
			Tasks.value[task].outputs.push(data);
			provide('Task.Data', { task, data: Tasks.value[task] });
		});

		signalR.onclose(start);
		start();
	});
};
