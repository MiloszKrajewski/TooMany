<template>
	<select :value="value" @input="onInput">
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
		function onInput(event: { target: { value: string } }) {
			emit('input', event.target.value);
		}

		return { onInput };
	},
	head: {},
});
</script>
