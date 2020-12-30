<template>
	<div class="page">
		<header>
			<div class="controls">
				<div class="tabs">Something goes here?</div>
			</div>
			<div class="subheader">
				<ul class="breadcrumbs">
					<li><NuxtLink to="/">Home</NuxtLink></li>
					<li>{{ name }}</li>
				</ul>
			</div>
		</header>
		<ScrollToBottom class="terminal">
			<TerminalLogs :name="name" />
		</ScrollToBottom>
	</div>
</template>

<script lang="ts">
import { defineComponent, useContext } from '@nuxtjs/composition-api';
import TerminalLogs from '~/components/terminal/Log.vue';
import ScrollToBottom from '~/components/ScrollToBottom.vue';

export default defineComponent({
	components: { TerminalLogs, ScrollToBottom },
	setup() {
		const { params } = useContext();
		return { name: params.value.name };
	},
});
</script>

<style lang="postcss" scoped>
.page {
	display: flex;
	flex-direction: column;
	height: 100%;
	header {
		background: rgba(0, 0, 0, 0.1);
		.controls {
			height: 2rem;
			display: flex;
			justify-content: flex-end;
		}
		.subheader {
			padding: 0 1rem 1rem;
			.breadcrumbs {
				margin: 0;
				padding: 0;
				display: flex;
				list-style: none;
				font-size: 1.25rem;
				font-weight: 300;
				li {
					a {
						color: rgba(255, 255, 255, 0.8);
						text-decoration: none;
					}
					&::after {
						color: rgba(255, 255, 255, 0.8);
						padding: 0 0.25rem;
						content: '/';
					}
					&:last-of-type,
					&:only-of-type {
						&::after {
							content: '';
						}
					}
				}
			}
		}
	}
	.terminal {
		background: rgba(255, 255, 255, 0.1);
		flex: 1 1 auto;
		padding: 1rem;
	}
}
</style>
