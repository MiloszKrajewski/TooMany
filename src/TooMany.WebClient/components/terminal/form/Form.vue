<template>
	<form @submit.prevent="onSave">
		<dl>
			<dt><label for="name">name</label></dt>
			<dd>
				<input id="name" v-model="name" type="text" />
			</dd>
		</dl>
		<table>
			<thead>
				<tr>
					<th>Task</th>
					<th v-for="stream in streamNames" :key="stream">{{ stream }}</th>
					<th>Filter</th>
					<th>Include</th>
				</tr>
				<tr v-for="task of tasks" :key="task.name">
					<td>{{ task.name }}</td>
					<td v-for="stream of streamNames" :key="stream">
						<input v-model="task[stream]" type="checkbox" />
					</td>
					<td><input v-model="task.filter" type="text" /></td>
					<td><input v-model="task.include" type="checkbox" /></td>
				</tr>
			</thead>
			<tbody></tbody>
		</table>
		<input id="save" type="submit" value="Save" />
	</form>
</template>

<script lang="ts">
import { defineComponent, ref, computed } from '@nuxtjs/composition-api';
import { StdSteams } from '~/types';
import { useTaskMeta } from '~/hooks';

export default defineComponent({
	setup() {
		const tasks = useTaskMeta(null);
		const name = ref('');
		const streamNames = Object.keys(StdSteams);
		const streams: Record<string, boolean> = {};
		for (const s of streamNames) {
			streams[s] = true;
		}

		const state = computed(() =>
			tasks.value.map((t) => ({
				name: t.name,
				...streams,
				filter: '',
				include: false,
			})),
		);

		function onSave() {
			console.log(state.value);
		}

		return { name, onSave, streamNames, tasks: state };
	},
});
</script>

<style lang="postcss" scoped>
fieldset {
	margin: 0;
	padding: 0;
	border: none;
}
</style>
