<template>
	<Fragment>
		<header>
			<h3>Task Editor</h3>
		</header>
		<Select :options="names" :value="name" @onChange="onSelect" />
		<Form
			:names="names"
			:task="task"
			:is-new-task="isNewTask"
			@onSave="saveTask"
			@onDelete="deleteTask"
		/>
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import { defineComponent, computed, ref, watch } from '@nuxtjs/composition-api';
import Select from './../Select.vue';
import Form from './form/Form.vue';
import { useTaskMeta, useApi } from '~/hooks';

interface ITask {
	name: string;
	executable?: string;
	arguments?: string;
	directory?: string;
	environment?: Record<string, string>;
	tags?: string[];
}

type TTasks = ITask[];
const newTaskName = 'New Task';

function getTask(tasks: TTasks, name: string) {
	const task = tasks.find((t) => t.name === name);
	if (!task) {
		return {};
	}
	return {
		...task,
		name,
	};
}
export default defineComponent({
	components: { Fragment, Select, Form },
	setup() {
		const api = useApi();
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

		function deleteTask(payload: { name: string }) {
			if (payload.name === newTaskName) return;
			const taskApi = api.task(payload.name);
			name.value = newTaskName;
			taskApi.delete();
		}
		async function saveTask(payload: ITask) {
			const taskApi = api.task(payload.name);
			await taskApi.create<ITask>(payload);
			name.value = payload.name;
		}

		const isNewTask = computed(() => name.value === newTaskName);

		return { name, names, onSelect, task, deleteTask, saveTask, isNewTask };
	},
});
</script>
