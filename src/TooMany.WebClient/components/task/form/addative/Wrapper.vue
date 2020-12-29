<template>
	<dl>
		<dt>
			<slot name="title"></slot>
		</dt>
		<dd>
			<slot></slot>
		</dd>
		<dd v-if="isAddVisible">
			<input value="+" type="button" @click="onAdd" />
		</dd>
	</dl>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';

export default defineComponent({
	props: {
		isAddVisible: {
			type: Boolean,
			default: false,
		},
	},
	setup(_, { emit }) {
		function onAdd() {
			emit('onAdd');
		}
		return { onAdd };
	},
});
</script>
