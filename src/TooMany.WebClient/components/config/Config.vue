<template>
	<div class="root">
		<header>
			<h3>Task</h3>
		</header>
		<Form />
	</div>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';
import Form from './task/Form.vue';

export default defineComponent({
	components: { Form },
});
</script>
