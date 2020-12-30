<template>
	<ul>
		<li v-for="log in logs" :key="log.id">> {{ log.text }}</li>
	</ul>
</template>
<script lang="ts">
import { defineComponent, useFetch } from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import ScrollToBottom from '~/components/ScrollToBottom.vue';
import { useRealtime, useApi } from '~/hooks';

export default defineComponent({
	components: { ScrollToBottom },
	props: {
		name: {
			type: String,
			required: true,
		},
	},
	setup({ name }) {
		const RealtimeLogs = useRealtime.TaskLog(name);
		const api = useApi().task(name);

		useFetch(async () => {
			const result = (await api.refresh()) || [];
			RealtimeLogs.value = result.map((r) => ({ ...r, id: uuidv4() }));
		});

		return {
			logs: RealtimeLogs,
		};
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
