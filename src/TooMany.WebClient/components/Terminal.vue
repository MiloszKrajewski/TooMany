<template>
	<Fragment>
		<header>
			<button @click="handleStart">start</button>
			<button @click="handleRestart">restart</button>
			<button @click="handleRefresh">refresh</button>
			<button @click="handleStop">stop</button>
			<button @click="handleDelete">delete</button>
		</header>
		<div class="scrollWrapper">
			<ul>
				<li v-for="log in logs" :key="log.id">
					{{ log.date }}: {{ log.text }}
				</li>
			</ul>
		</div>
	</Fragment>
</template>
<script lang="ts">
import { defineComponent, useFetch } from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import { useRealtime, useApi } from '~/hooks';

enum Status {
	Stopped = 'Stopped',
	Started = 'Started',
	Failed = 'Failed',
}

export default defineComponent({
	props: {
		task: {
			type: String,
			required: true,
		},
		status: {
			type: String,
			required: true,
		},
	},
	setup({ task, status }, { emit }) {
		const RealtimeLogs = useRealtime.TaskLog(task);
		const api = useApi().task(task);

		const handleRefresh = async () => {
			const result = (await api.refresh()) || [];
			RealtimeLogs.value = result.map((r) => ({ ...r, id: uuidv4() }));
			emit('refresh', task);
		};

		const handleDelete = async () => {
			await api.delete();
			emit('delete', task);
		};

		const handleStop = async () => {
			if (status === Status.Stopped) return;
			await api.stop();
			emit('stop', task);
		};

		const handleStart = async () => {
			if (status === Status.Started) return;
			await api.start();
			emit('start', task);
		};

		const handleRestart = async () => {
			await api.restart();
			emit('restart', task);
		};

		useFetch(handleRefresh);

		return {
			handleStart,
			handleRestart,
			handleRefresh,
			handleStop,
			handleDelete,
			logs: RealtimeLogs,
		};
	},
});
</script>

<style lang="postcss" scoped>
.scrollWrapper {
	height: 500px;
	overflow: auto;
	display: flex;
	flex-direction: column-reverse;
}
</style>
