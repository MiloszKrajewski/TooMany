<template>
	<form @submit.prevent="onSave">
		<pre>
			{{ processes }}
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
				<tr v-for="pro in processes" :key="pro.name">
					<td>{{ pro.name }}</td>
					<td>
						<input
							v-model="pro.stdOut"
							:disabled="!isFormReady"
							type="checkbox"
						/>
					</td>
					<td>
						<input
							v-model="pro.stdErr"
							:disabled="!isFormReady"
							type="checkbox"
						/>
					</td>
					<td>
						<input v-model="pro.filter" :disabled="!isFormReady" type="text" />
					</td>
					<td>
						<input
							v-model="pro.include"
							:disabled="!isFormReady"
							type="checkbox"
						/>
					</td>
				</tr>
			</tbody>
		</table>
		<input id="save" :disabled="!isFormReady" type="submit" value="Save" />
	</form>
</template>

<script lang="ts">
import {
	defineComponent,
	ref,
	inject,
	computed,
} from '@nuxtjs/composition-api';
import { Terminal } from '../types';
import { Ref, Task } from '~/types';
import { StateSymbol as TaskMetadataState } from '~/components/TaskMetadataProvider.vue';

export default defineComponent({
	props: {
		initialState: {
			type: Object as () => Terminal.Manifest,
			default: () => ({}),
		},
		isNew: {
			type: Boolean,
			default: false,
		},
	},
	setup(props, { emit }) {
		const tasks = inject<Ref<Task.Meta>>(TaskMetadataState);

		const isFormReady = computed(() => typeof tasks?.value !== 'undefined');
		const defaultProcesses = computed(() => {
			if (typeof tasks?.value === 'undefined') {
				return [];
			}
			return tasks.value.map((task) => ({
				name: task.name,
				stdOut: true,
				stdErr: true,
				filter: '',
				include: false,
			}));
		});

		const name = ref(props.initialState.name || '');
		const processes = ref(
			props.isNew ? defaultProcesses.value : props.initialState.processes,
		);

		function onSave() {
			emit('onSave', { name: name.value, processes: processes.value });
		}

		return { isFormReady, name, onSave, processes };
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
