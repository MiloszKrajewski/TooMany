<template>
	<ul>
		<li v-for="task in tasks" :key="task.name">
			{{ task.name }}
		</li>
	</ul>
</template>
<script lang="ts">
import {
	defineComponent,
	useFetch,
	useContext,
	ref,
	inject,
} from '@nuxtjs/composition-api';
interface Task {
	name: string;
}
type Tasks = Task[];

interface Ref<T> {
	value: T;
}

export default defineComponent({
	setup() {
		const tasks: Ref<Tasks> = ref([]);

		const { env } = useContext();

		useFetch(async () => {
			const res = await fetch(`${env.apiV1Url}/task`);
			const data = await res.json();
			tasks.value = data?.result || [];
		});

		const TasksLog = inject('Task.Log') || {};
		const TasksData = inject('Task.Data') || {};

		console.log({
			TasksLog,
			TasksData,
		});

		return { tasks };
	},
});
</script>
