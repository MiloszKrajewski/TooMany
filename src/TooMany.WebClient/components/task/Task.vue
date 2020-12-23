<template>
	<div class="root">
		<header>
			<h3>Editor</h3>
		</header>
		<Select :options="names" :value="name" @onChange="onSelect" />
		<Form
			v-if="name"
			:name="name"
			:executable="task.executable"
			:args="task.arguments"
			:directory="task.directory"
			:env-vars="task.environment"
			:tags="task.tags"
		/>
	</div>
</template>

<script lang="ts">
import { defineComponent, computed, ref, watch } from '@nuxtjs/composition-api';
import Select from './../Select.vue';
import Form from './Form.vue';
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
	components: { Select, Form },
	setup() {
		const tasks = useTaskMeta(null);
		const name = ref('');

		watch(
			() => tasks.value,
			(tasks) => {
				if (!name.value && tasks.length) {
					name.value = tasks[0].name;
				}
			},
		);

		const names = computed(() => tasks.value.map((t) => t.name));
		const task = computed(() => tasks.value.find((t) => t.name === name.value));

		function onSelect(value: string) {
			name.value = value;
		}

		// TODO: create task
		// TODO: edit task
		// TODO: delete task
		return { name, names, onSelect, task };
	},
});
</script>
