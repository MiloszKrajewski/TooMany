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
	// useFetch,
	onUnmounted,
} from '@nuxtjs/composition-api';
import { LogChannel } from '@/plugins/SignalR';

interface ILogData {
	channel: LogChannel;
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
		const ctx = useContext();

		// useFetch(async () => {
		// 	const res = await fetch(`${env.apiV1Url}/task/taskX/logs`);
		// 	const data = await res.json();
		// 	Tasks.value.push({ task: 'taskX', data });
		// });
		console.log(ctx);

		ctx.$SignalR.onLog((task, data) => {
			Tasks.value.push({ task, data });
		});

		onMounted(ctx.$SignalR.start);
		onUnmounted(ctx.$SignalR.stop);

		return { Tasks };
	},
});
</script>

<style scoped>
pre {
	width: 100%;
}
</style>
