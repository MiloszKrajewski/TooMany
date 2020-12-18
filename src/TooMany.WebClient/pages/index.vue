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
import { defineComponent, useContext, provide } from '@nuxtjs/composition-api';
export default defineComponent({
	setup() {
		provide('todoLength', ['Feed a cat', 'Buy tickets'].length);
		const ctx = useContext();
		console.log(ctx);
	},
	mounted() {
		console.log(this);
		// will console.log 'Hello mounted!'
	},
});
</script>

<style>
@import './index.css';
</style>
