<template>
	<div class="root">
		<header>
			<h3>Editor</h3>
		</header>
		<Select :options="names" :value="name" @onChange="onSelect" />
		<Form
			:names="names"
			:name="task.name"
			:executable="task.executable"
			:args="task.arguments"
			:directory="task.directory"
			:env-vars="task.environment"
			:tags="task.tags"
			@onDelete="deleteTask"
			@onCreate="createTask"
			@onUpdate="updateTask"
		/>
	</div>
</template>

<script lang="ts">
import { defineComponent, computed, ref, watch } from '@nuxtjs/composition-api';
import Select from './../Select.vue';
import Form from './form/Form.vue';
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
const newTaskName = 'New Task';

function getTask(tasks: Tasks, name: string) {
	const task = tasks.find((t) => t.name === name);
	if (!task) {
		return {};
	}
	return {
		...task,
		name,
		environment: { ...task.environment, x: 'y', a: 'b' },
	};
}
export default defineComponent({
	components: { Select, Form },
	setup() {
		const tasks = useTaskMeta(null);
		const name = ref(newTaskName);
		const task = ref(getTask(tasks.value, name.value));

		watch(
			// I only want to update the task when the selection changes
			() => name.value,
			(name) => {
				task.value = getTask(tasks.value, name);
			},
		);

		const names = computed(() => [
			newTaskName,
			...tasks.value.map((t) => t.name),
		]);

		function onSelect(value: string) {
			name.value = value;
		}

		function deleteTask(payload) {
			console.log('deleteTask', payload);
		}
		function createTask(payload) {
			console.log('createTask', payload);
		}
		function updateTask(payload) {
			console.log('updateTask', payload);
		}
		return { name, names, onSelect, task, deleteTask, createTask, updateTask };
	},
});
</script>
