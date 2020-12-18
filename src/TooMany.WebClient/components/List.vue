<template>
	<ul>
		tasks
		<pre>
					{{ JSON.stringify(tasks, null, 2) }}
				</pre>
		<li v-for="task in tasks" :key="task.name">
			{{ task.name }}
		</li>
	</ul>
</template>
<script lang="js">

export default {
	async fetch(ctx) {
		console.log(ctx);
		let tasks;
		try {
			const res = await fetch(`${ctx.env.apiUrl}/api/v1/task`);
			const data = await res.json();
			tasks = data?.result || [];
		} catch (e) {
			tasks = []
		}
		console.log(tasks);
	},
	data() {
		return {
			tasks: [],
		};
	},
};
</script>
