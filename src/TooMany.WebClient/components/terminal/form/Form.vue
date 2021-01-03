<template>
	<form>
		<dl>
			<dt><label for="name">name</label></dt>
			<dd>
				<input id="name" v-model="name" type="text" />
			</dd>
		</dl>
		<table>
			<thead>
				<tr>
					<th>Process</th>
					<th>StdOut</th>
					<th>StdErr</th>
					<th>Filter</th>
					<th>Include</th>
				</tr>
			</thead>
			<tbody>
				<tr v-for="task in tasks" :key="task.name">
					<td>{{ task.name }}</td>
					<td>
						<input
							v-model="task.stdOut"
							:disabled="!isFormReady || !task.include"
							type="checkbox"
						/>
					</td>
					<td>
						<input
							v-model="task.stdErr"
							:disabled="!isFormReady || !task.include"
							type="checkbox"
						/>
					</td>
					<td>
						<input
							v-model="task.filter"
							:disabled="!isFormReady || !task.include"
							type="text"
						/>
					</td>
					<td>
						<input
							v-model="task.include"
							:disabled="!isFormReady"
							type="checkbox"
						/>
					</td>
				</tr>
			</tbody>
		</table>
		<input
			id="update"
			:disabled="
				isNew || !isFormReady || isFormValid || (isNameTaken && !isInitialName)
			"
			type="button"
			value="Update"
			@click="onUpdate"
		/>
		<input
			id="create"
			:disabled="!isFormReady || isFormValid || isNameTaken"
			type="button"
			value="Create"
			@click="onCreate"
		/>
	</form>
</template>

<script lang="ts">
import {
	defineComponent,
	inject,
	computed,
	reactive,
	watch,
	toRefs,
} from '@nuxtjs/composition-api';
import { Ref, Task, Terminal } from '~/types';
import { NamesSymbol as TaskMetadataNames } from '~/components/TaskMetadataProvider.vue';

interface Terminal {
	name: string;
	tasks: Terminal.Task[];
}

function MapTerminalPropToState({
	name,
	tasks,
	taskNames,
}: {
	name: string;
	tasks: Terminal.Task[];
	taskNames: string[];
}): Terminal {
	const taskMap: Record<string, Terminal.Task> = {};
	for (const task of tasks) {
		taskMap[task.name] = task;
	}
	return {
		name,
		tasks: taskNames.map((name) => {
			const initialValue = taskMap[name];
			if (!initialValue) {
				return {
					name,
					stdOut: true,
					stdErr: true,
					filter: '',
					include: false,
				};
			}
			return {
				name,
				stdOut: initialValue.stdOut ?? true,
				stdErr: initialValue.stdErr ?? true,
				filter: initialValue.filter || '',
				include: true,
			};
		}),
	};
}

export default defineComponent({
	props: {
		terminal: {
			type: Object as () => Terminal,
			default: () => ({}),
		},
		isNew: {
			type: Boolean,
			default: false,
		},
		names: {
			type: Array as () => string[],
			default: [],
		},
	},
	setup(props, { emit }) {
		const taskNames = inject<Ref<string[]>>(TaskMetadataNames) || { value: [] };
		const state = reactive<Terminal>(
			MapTerminalPropToState({
				name: props.terminal.name,
				tasks: props.terminal.tasks,
				taskNames: taskNames.value,
			}),
		);
		watch(
			() => props.terminal.name,
			() => {
				const terminal = MapTerminalPropToState({
					name: props.terminal.name,
					tasks: props.terminal.tasks,
					taskNames: taskNames.value,
				});
				state.name = terminal.name;
				state.tasks = terminal.tasks;
			},
		);

		function onCreate() {
			emit('onCreate', state);
		}
		function onUpdate() {
			emit('onUpdate', state);
		}

		const isFormReady = computed(() => taskNames.value.length > 0);
		const isFormValid = computed(() => state.tasks.every((t) => !t.include));
		const isNameTaken = computed(() =>
			props.names.some((name) => name === state.name),
		);
		const isInitialName = computed(() => props.terminal.name === state.name);

		return {
			isFormReady,
			isFormValid,
			isNameTaken,
			isInitialName,
			onCreate,
			onUpdate,
			...toRefs(state),
		};
	},
});
</script>
