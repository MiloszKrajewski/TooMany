<template>
	<form>
		<pre>
			{{ tasks }}
		</pre
		>
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
							:disabled="!isFormReady"
							type="checkbox"
						/>
					</td>
					<td>
						<input
							v-model="task.stdErr"
							:disabled="!isFormReady"
							type="checkbox"
						/>
					</td>
					<td>
						<input v-model="task.filter" :disabled="!isFormReady" type="text" />
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
			:disabled="!isFormReady"
			type="button"
			value="Update"
			@click="onUpdate"
		/>
		<input
			id="create"
			:disabled="!isFormReady"
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
import { Terminal } from '../types';
import { Ref, Task } from '~/types';
import { NamesSymbol as TaskMetadataNames } from '~/components/TaskMetadataProvider.vue';

function MapTerminalPropToState({
	initialManifest,
	taskNames,
}: {
	initialManifest: Terminal.Manifest;
	taskNames: string[];
}): Terminal.Manifest {
	const initialManifestMap: Record<string, Terminal.Task> = {};
	for (const task of initialManifest.tasks) {
		initialManifestMap[task.name] = task;
	}
	const output = {
		name: initialManifest.name,
		tasks: taskNames.map((name) => {
			const initialValue = initialManifestMap[name];
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
				stdOut: initialValue.stdOut || true,
				stdErr: initialValue.stdErr || true,
				filter: initialValue.filter || '',
				include: initialValue.include || false,
			};
		}),
	};
	return output;
}

export default defineComponent({
	props: {
		terminal: {
			type: Object as () => Terminal.Manifest,
			default: () => ({}),
		},
	},
	setup(props, { emit }) {
		const taskNames = inject<Ref<string[]>>(TaskMetadataNames);
		const state = reactive<Terminal.Manifest>(
			MapTerminalPropToState({
				initialManifest: props.terminal,
				taskNames: taskNames?.value || [],
			}),
		);
		watch(
			() => props.terminal.name,
			() => {
				const terminal = MapTerminalPropToState({
					initialManifest: props.terminal,
					taskNames: taskNames?.value || [],
				});
				state.name = terminal.name;
				state.tasks = terminal.tasks;
			},
		);
		watch(
			() => taskNames?.value || [],
			(names) => {
				/**
				 * For when someone updates on a different window
				 */
				const nameMap: Record<string, boolean> = {};
				for (const name of names) {
					nameMap[name] = true;
				}
				const validTasks = state.tasks.filter((t) => nameMap[t.name]);

				const currentTaskNameMap: Record<string, boolean> = {};
				for (const task of validTasks) {
					nameMap[task.name] = true;
				}

				const newTasks = names
					.filter((name) => {
						const hasNameBeenUsed = currentTaskNameMap[name];
						return !hasNameBeenUsed;
					})
					.map((name) => ({
						name,
						stdOut: true,
						stdErr: true,
						filter: '',
						include: false,
					}));

				state.tasks = [...validTasks, ...newTasks];
			},
		);

		const isFormReady = computed(() => typeof taskNames?.value !== 'undefined');

		function onSave(type: string) {
			emit(type, { name: state.name, tasks: state.tasks });
		}

		function onCreate() {
			onSave('onCreate');
		}
		function onUpdate() {
			onSave('onUpdate');
		}

		return { isFormReady, onCreate, onUpdate, ...toRefs(state) };
	},
});
</script>

<style lang="postcss" scoped>
form {
	max-width: 350px;
	table {
		display: grid;
		border-collapse: collapse;
		width: fit-content;
		min-width: 100%;
		grid-template-columns: repeat(5, minmax(50px, 1fr));

		thead,
		tbody,
		tr {
			display: contents;
		}

		th,
		td {
			overflow: hidden;
			text-overflow: ellipsis;
			white-space: nowrap;
		}

		th {
			position: sticky;
			top: 0;
			text-align: left;
			font-weight: normal;
			&:last-child {
				border: 0;
			}
		}

		td {
			padding-top: 10px;
			padding-bottom: 10px;
		}
	}
}
</style>
