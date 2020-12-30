<template>
	<Fragment>
		<header>
			<h3>Terminals</h3>
		</header>
		<Form />
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import { defineComponent } from '@nuxtjs/composition-api';
import Form from './form/Form.vue';

export default defineComponent({
	components: { Fragment, Form },
	setup() {
		return {};
	},
});
</script>
