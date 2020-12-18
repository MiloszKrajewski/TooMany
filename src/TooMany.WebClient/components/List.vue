<template>
	<ul>
		{{
			todoLength
		}}
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
			try {
				const res = await fetch(`${env.apiUrl}/api/v1/task`);
				const data = await res.json();
				tasks.value = data?.result || [];
			} catch (e) {
				const res = await fetch(`${env.baseUrl}/tasks.dummy.json`);
				const data = await res.json();
				tasks.value = data?.result || [];
			}
		});
		const todoLength = inject('todoLength');
		const TasksLog = inject('Task.Log') || {};
		const TasksData = inject('Task.Data') || {};

		console.log({
			TasksLog,
			TasksData,
		});

		return { tasks, todoLength };
	},
});
</script>
