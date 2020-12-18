<template>
	<ul>
		<li v-for="task in tasks" :key="task.name">
			{{ task.name }}
		</li>
	</ul>
</template>
<script lang="ts">
export default {
	async fetch() {
		let tasks;
		try {
			const res = await fetch(`${this.$nuxt.context.env.apiUrl}/api/v1/task`);
			const data = await res.json();
			tasks = data?.result || [];
		} catch (e) {
			const res = await fetch(
				`${this.$nuxt.context.env.baseUrl}/tasks.dummy.json`,
			);
			const data = await res.json();
			tasks = data?.result || [];
		}
		this.tasks = tasks;
	},
	data() {
		return {
			tasks: [],
		};
	},
};
</script>
