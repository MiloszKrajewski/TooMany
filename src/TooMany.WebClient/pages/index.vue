<template>
	<div class="root">
		<Theme />
		<div v-for="task in tasks" :key="task.name">
			<h5>{{ task.name }}</h5>
			<h5>{{ task.arguments }}</h5>
			<h5>{{ task.directory }}</h5>
			<h5>{{ task.expected_state }}</h5>
			<h5>{{ task.actual_state }}</h5>
			<Terminal
				:task="task.name"
				:status="task.actual_state"
				@delete="onDelete"
			/>
		</div>
	</div>
</template>

<script lang="ts">
import { defineComponent, useFetch, ref } from '@nuxtjs/composition-api';
import { useRealtime, useApi } from '~/hooks';
import { Ref, Task, Realtime } from '~/@types';

import Theme from '~/components/theme/Theme.vue';
import Terminal from '~/components/Terminal.vue';

function useTaskMeta(id: Realtime.ChannelId) {
	const InitialMeta: Ref<Task.Meta> = ref([]);
	const api = useApi();
	useFetch(async () => {
		const result = await api.tasks();
		InitialMeta.value = result;
	});

	const RealtimeMeta = useRealtime.TaskMeta(id, InitialMeta);

	return RealtimeMeta;
}

export default defineComponent({
	components: { Theme, Terminal },
	setup() {
		const tasks = useTaskMeta(null);

		function onDelete(task: string) {
			tasks.value = tasks.value.filter((t) => t.name !== task);
		}

		return { tasks, onDelete };
	},
});
</script>

<style>
@import './index.css';
</style>
