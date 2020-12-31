<template>
	<ul>
		<li v-for="log in logs" :key="log.id">>{{ log.task }} {{ log.text }}</li>
	</ul>
</template>
<script lang="ts">
import {
	defineComponent,
	useFetch,
	computed,
	inject,
	ref,
	watchEffect,
} from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import { useRealtime, useApi } from '~/hooks';
import { Terminal } from '~/components/terminal/types';
import { Ref } from '~/types';
import { StateSymbol as TerminalState } from '~/components/Terminal/TerminalProvider.vue';

const logFiler = (tasks: Ref<Terminal.Task[]>, name: string, data) => {
	const task = tasks.value.find((t) => t.name === name);
	console.log(name, 'stdout', task?.stdOut);
	if (task?.stdOut === false && data.channel === 'StdOut') {
		return false;
	}
	if (task?.stdErr === false && data.channel === 'StdErr') {
		return false;
	}
	// if (task?.filter) {
	// 	const filter = new RegExp(task.filter);
	// 	if (filter.exec(data.text)) {
	// 		return false;
	// 	}
	// }
	return true;
};

export default defineComponent({
	props: {
		name: {
			type: String,
			required: true,
		},
	},
	setup({ name }) {
		const terminals = inject<Ref<Terminal.Manifest>>(TerminalState) || {
			value: {},
		};
		const tasks = ref(terminals.value[name]);

		watchEffect(() => {
			if (terminals.value[name]) {
				tasks.value = terminals.value[name];
			}
		});

		const names = computed(() => tasks.value.map((t) => t.name));
		const logs = useRealtime.TaskLogs<Ref<Terminal.Task[]>>(
			names.value,
			undefined,
			logFiler,
			tasks,
		);

		console.log(names);
		useFetch(() => {
			for (const name of names.value) {
				const api = useApi().task(name);
				api.refresh().then((result = []) => {
					const transformedResults = result
						.map((r) => ({
							...r,
							id: uuidv4(),
							task: name,
							time: new Date(r.timestamp).getTime(),
						}))
						.filter((data) => logFiler(tasks.value, name, data));
					logs.value = [...logs.value, ...transformedResults].sort(
						(a, b) => a.time - b.time,
					);
				});
			}
		});

		return { logs };
	},
});
</script>

<style lang="postcss" scoped>
ul {
	padding: 0;
	margin: 0;
	list-style: none;
}
</style>
