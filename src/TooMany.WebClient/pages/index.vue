<template>
	<div class="root">
		C O N T E N T
		<div v-for="task in tasks" :key="task.name">
			<h5>{{ task.name }}</h5>
			<h5>{{ task.arguments }}</h5>
			<h5>{{ task.directory }}</h5>
			<h5>{{ task.expected_state }}</h5>
			<h5>{{ task.actual_state }}</h5>
			<Terminal :task="task.name" :status="task.actual_state" />
		</div>
	</div>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';
import { useTaskMeta } from '~/hooks';

import TaskEditor from '~/components/TaskEditor.vue';
import Terminal from '~/components/Terminal.vue';

export default defineComponent({
	components: { Terminal, TaskEditor },
	setup() {
		const tasks = useTaskMeta(null);
		return { tasks };
	},
});
</script>

<style lang="postcss" scoped>
.root {
	padding: 0.5rem;
	position: relative;
	width: 100%;
	height: 100%;
	transition: color 300ms, background-color 300ms;
	background: var(--background-color);
	color: var(--text-color);
}
</style>
