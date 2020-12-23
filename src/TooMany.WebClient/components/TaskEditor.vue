<template>
	<div>Editor</div>
</template>

<script lang="ts">
import { defineComponent, useContext, computed } from '@nuxtjs/composition-api';
import { useTaskMeta } from '~/hooks';

interface Task {
	name: string;
	executable?: string;
	arguments?: string;
	directory?: string;
	environment?: Record<string, string>;
	tags?: string[];
}

type Tasks = Task[];

export default defineComponent({
	setup() {
		const ctx = useContext();
		const tasks = useTaskMeta(null);

		const names = computed(() => tasks.value.map((t) => t.name));
		console.log(names);
		// TODO: create task
		// TODO: edit task
		// TODO: delete task
		return { names };
	},
});
</script>
