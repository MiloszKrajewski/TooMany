<template>
	<select :value="value" @input="onInput">
		<option v-for="option in options" :id="option" :key="option">
			{{ option }}
		</option>
	</select>
</template>

<script lang="ts">
import { defineComponent, onUpdated } from '@nuxtjs/composition-api';

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
		onUpdated(() => {
			console.log('update');
		});
		function onInput(event: { target: { value: string } }) {
			emit('input', event.target.value);
		}

		return { onInput };
	},
	head: {},
});
</script>
