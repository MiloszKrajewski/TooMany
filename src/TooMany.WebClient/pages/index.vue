<template>
	<Fragment>
		<div v-for="task in tasks" :key="task.name">
			<NuxtLink :to="`/task/${task.name}`">{{ task.name }}</NuxtLink>
		</div>
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import { defineComponent, inject } from '@nuxtjs/composition-api';
import { Ref, Task } from '~/types';
import { StateSymbol as TaskMetadataState } from '~/components/TaskMetadataProvider.vue';

export default defineComponent({
	components: { Fragment },
	setup() {
		const tasks = inject<Ref<Task.Meta>>(TaskMetadataState) || { value: [] };
		return { tasks };
	},
});
</script>
