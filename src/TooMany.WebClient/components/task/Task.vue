<template>
	<Fragment>
		<header>
			<h3>Task Editor</h3>
		</header>
		<Select v-model="name" :options="names" />
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
import {
	defineComponent,
	computed,
	ref,
	watch,
	inject,
} from '@nuxtjs/composition-api';
import Form from './form/Form.vue';
import Select from '~/components/Select.vue';
import { useApi } from '~/hooks';
import { Ref, Task } from '~/types';
import {
	StateSymbol as TaskMetadataState,
	NamesSymbol as TaskMetadataNames,
} from '~/components/TaskMetadataProvider.vue';

interface ITask {
	name: string;
	executable?: string;
	arguments?: string;
	directory?: string;
	environment?: Record<string, string>;
	tags?: string[];
}

type TTasks = ITask[];
const newName = 'New Task';

function getTask(name: string, tasks?: TTasks) {
	if (!tasks) {
		return {};
	}
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
		const tasks = inject<Ref<Task.Meta>>(TaskMetadataState);
		const taskNames = inject<Ref<string[]>>(TaskMetadataNames);
		const name = ref(newName);
		const task = ref(getTask(name.value, tasks?.value));
		const names = computed(() => {
			if (!taskNames?.value) return [newName];
			return [newName, ...taskNames.value];
		});

		watch(
			// I only want to update the task when the selection changes
			() => name.value,
			(name) => {
				task.value = getTask(name, tasks?.value);
			},
		);

		function deleteTask(payload: { name: string }) {
			if (payload.name === newName) return;
			const taskApi = api.task(payload.name);
			name.value = newName;
			taskApi.delete();
		}
		async function saveTask(payload: ITask) {
			const taskApi = api.task(payload.name);
			await taskApi.create<ITask>(payload);
			name.value = payload.name;
		}

		const isNewTask = computed(() => name.value === newName);

		return { name, names, task, deleteTask, saveTask, isNewTask };
	},
});
</script>
