<template>
	<ul>
		<li v-for="log in logs" :key="log.id">>{{ log.task }} {{ log.text }}</li>
	</ul>
</template>
<script lang="ts">
import {
	defineComponent,
	ref,
	useContext,
	onUnmounted,
	watch,
	onMounted,
} from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import { useApi } from '~/hooks';
import { Ref, Task, Realtime, Terminal } from '~/types';

export default defineComponent({
	props: {
		tasks: {
			type: Array as () => Terminal.Task[],
			default: () => [],
		},
	},
	setup(props) {
		const { $SignalR } = useContext();
		const logs: Ref<Task.Logs> = ref([]);
		const Listeners = ref<Record<string, Realtime.onLogFn>>({});

		function transformData(data: Task.ILog, task: string) {
			return {
				...data,
				id: uuidv4(),
				task,
				time: new Date(data.timestamp).getTime(),
			};
		}

		function filterData(task: Terminal.Task) {
			return function (data: Task.ILog) {
				if (task.stdOut === false && data.channel === 'StdOut') {
					return false;
				}
				if (task.stdErr === false && data.channel === 'StdErr') {
					return false;
				}
				if (task.filter && !data.text) {
					return false;
				}
				if (data.text && task.filter && !data.text.match(task.filter)) {
					return false;
				}
				return true;
			};
		}

		const api = useApi();
		function handleTasks(
			tasks: Terminal.Task[],
			prevTasks: Terminal.Task[] = [],
		) {
			const prevTaskMap: Record<string, boolean> = {};
			for (const prev of prevTasks) {
				prevTaskMap[prev.name] = true;
			}

			// clear down data
			logs.value = [];

			for (const task of tasks) {
				const { refresh } = api.task(task.name);

				// get all updated data
				refresh().then((result = []) => {
					const transformedResults = result
						.map((data) => transformData(data, task.name))
						.filter(filterData(task));
					logs.value = [...logs.value, ...transformedResults].sort(
						(a, b) => a.time - b.time,
					);
				});

				if (prevTaskMap[task.name]) continue;
				// stop listening to old data
				$SignalR.offTaskLog(Listeners.value[task.name]);
				// start listening to new data
				Listeners.value[task.name] = $SignalR.onTaskLog(task.name, (data) => {
					const payload = transformData(data, task.name);
					if (!filterData(task)(payload)) return;
					logs.value = [...logs.value, payload];
				});
			}
		}

		onMounted(() => handleTasks(props.tasks));
		watch(() => props.tasks, handleTasks);
		onUnmounted(() => {
			for (const Listener of Object.values(Listeners.value)) {
				$SignalR.offTaskLog(Listener);
			}
		});

		return { logs };
	},
});
</script>

<style lang="postcss" scoped>
ul {
	padding: 0;
	margin: 0;
	list-style: none;
}
</style>
