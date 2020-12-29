<template>
	<div class="root">
		C O N T E N T
		<div v-for="task in tasks" id="content" :key="task.name">
			<h3>{{ task.name }}</h3>
			<ul>
				<li>
					executable:
					<span>{{ task.executable }}</span>
				</li>
				<li>
					directory:
					<span>{{ task.directory }}</span>
				</li>
				<li>
					arguments:
					<span>{{ task.arguments }}</span>
				</li>
				<li>
					environment:
					<span>{{ task.environment }}</span>
				</li>
				<li>
					expected_state:
					<span>{{ task.expected_state }}</span>
				</li>
				<li>
					actual_state:
					<span>{{ task.actual_state }}</span>
				</li>
			</ul>
			<Terminal :task="task.name" :status="task.actual_state" />
		</div>
	</div>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';
import { useTaskMeta } from '~/hooks';

import Terminal from '~/components/Terminal.vue';

export default defineComponent({
	components: { Terminal },
	setup() {
		const tasks = useTaskMeta(null);
		return { tasks };
	},
});
</script>

<style lang="postcss" scoped>
.root {
	padding: 0.5rem;
	transition: color 300ms, background-color 300ms;
	background: var(--background-color);
	color: var(--text-color);
	#content {
		margin-bottom: 2rem;
		padding: 1rem 0;
		border-bottom: 2px solid var(--text-color);
		ul {
			list-style: square;
			li {
				font-size: 1rem;
				span {
					font-weight: bold;
				}
			}
		}
		&:last-of-type {
			border-bottom: none;
		}
	}
}
</style>
