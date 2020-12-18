<template>
	<pre>
			{{ JSON.stringify(Tasks, null, 2) }}
	</pre
	>
</template>
<script lang="ts">
import {
	defineComponent,
	useContext,
	ref,
	onMounted,
	useFetch,
	onUnmounted,
} from '@nuxtjs/composition-api';
import SignalR from './signalr';

interface ILogData {
	channel: string;
	text: string;
	timestamp: string;
}

interface ITask {
	task: string;
	data: ILogData;
}

type ITasks = ITask[];

interface Ref<T> {
	value: T;
}

export default defineComponent({
	setup() {
		const Tasks: Ref<ITasks> = ref([]);
		const { env } = useContext();

		useFetch(async () => {
			const res = await fetch(`${env.apiV1Url}/task/taskX/logs`);
			const data = await res.json();
			Tasks.value.push({ task: 'taskX', data });
		});

		const signalR = new SignalR(env.realtimeUrl);
		signalR.onLog((task, data) => {
			Tasks.value.push({ task, data });
		});

		onMounted(signalR.start);
		onUnmounted(signalR.stop);

		return { Tasks };
	},
});
</script>

<style scoped>
pre {
	width: 100%;
}
</style>
