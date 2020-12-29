<template>
	<select :value="value" @change="onChange">
		<option v-for="option in options" :id="option" :key="option">
			{{ option }}
		</option>
	</select>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';

export default defineComponent({
	props: {
		options: {
			type: Array,
			default: () => [],
		},
		value: {
			type: String,
			required: true,
		},
	},
	setup(_, { emit }) {
		function onChange(event: { target: { value: string } }) {
			emit('onChange', event.target.value);
		}

		return { onChange };
	},
	head: {},
});
</script>
