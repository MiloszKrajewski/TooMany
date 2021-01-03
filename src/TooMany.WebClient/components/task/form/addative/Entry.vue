<template>
	<Fragment>
		<slot></slot>
		<input value="-" type="button" @click="onRemove" />
		<input value="+" type="button" @click="onAdd" />
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import { defineComponent } from '@nuxtjs/composition-api';

export default defineComponent({
	components: { Fragment },
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
