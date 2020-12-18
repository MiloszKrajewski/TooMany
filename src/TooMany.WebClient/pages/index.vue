<template>
	<div class="container">
		<div>
			<Logo />
			<List />
			<h1 class="title">toomany.webclient</h1>
			<div class="links">
				<a
					href="https://nuxtjs.org/"
					target="_blank"
					rel="noopener noreferrer"
					class="button--green"
				>
					Documentation
				</a>
				<a
					href="https://github.com/nuxt/nuxt.js"
					target="_blank"
					rel="noopener noreferrer"
					class="button--grey"
				>
					GitHub
				</a>
			</div>
		</div>
	</div>
</template>

<script lang="ts">
interface Context {
	env: {
		apiUrl: string;
		baseUrl: string;
	};
}

export default {
	async asyncData(ctx: Context) {
		let tasks;
		try {
			const res = await fetch(`${ctx.env.apiUrl}/api/v1/task`);
			const data = await res.json();
			tasks = data?.result || [];
		} catch (e) {
			const res = await fetch(`${ctx.env.baseUrl}/tasks.dummy.json`);
			const data = await res.json();
			tasks = data?.result || [];
		}
		return { tasks };
	},
	data() {
		return {
			tasks: [],
		};
	},
};
</script>

<style>
@import './index.css';
</style>
