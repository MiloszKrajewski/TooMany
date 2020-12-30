<template>
	<div :class="classes" class="root">
		<slot></slot>
	</div>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';

export default defineComponent({
	props: {
		classes: {
			type: Object,
			default: () => ({}),
		},
	},
});
</script>

<style lang="postcss" scoped>
.root {
	overflow: auto;
	display: flex;
	flex-direction: column-reverse;
}
</style>
