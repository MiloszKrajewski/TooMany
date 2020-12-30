<template>
	<div class="root">
		<header>
			<h3>Task</h3>
		</header>
		<TaskForm />
	</div>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';
import TaskForm from './task/Form.vue';

export default defineComponent({
	components: { TaskForm },
});
</script>
