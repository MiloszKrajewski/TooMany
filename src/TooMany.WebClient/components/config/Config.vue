<template>
	<Fragment>
		<header>
			<h3>Tasks</h3>
		</header>
		<TaskForm />
	</Fragment>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';
import TaskForm from './form/Form.vue';

export default defineComponent({
	components: { TaskForm },
});
</script>
