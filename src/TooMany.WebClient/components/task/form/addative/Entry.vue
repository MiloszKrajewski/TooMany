<template>
	<fieldset>
		<slot></slot>
		<input value="-" type="button" @click="onRemove" />
		<input value="+" type="button" @click="onAdd" />
	</fieldset>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';

export default defineComponent({
	props: {
		index: {
			type: Number,
			default: 0,
		},
	},
	setup(props, { emit }) {
		function onAdd() {
			emit('onAdd', props.index);
		}
		function onRemove() {
			emit('onRemove', props.index);
		}

		return { onAdd, onRemove };
	},
});
</script>
