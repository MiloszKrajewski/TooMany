<template>
	<Fragment>
		<div v-for="terminal in terminals" :key="terminal.id">
			<NuxtLink :to="`/terminal/${terminal.id}`">{{ terminal.name }}</NuxtLink>
		</div>
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import { defineComponent, inject } from '@nuxtjs/composition-api';

import { Ref, Terminal } from '~/types';
import { StateSymbol as TerminalState } from '~/components/Terminal/TerminalProvider.vue';

export default defineComponent({
	components: { Fragment },
	setup() {
		const terminals = inject<Ref<Terminal.Manifests>>(TerminalState);
		return { terminals };
	},
});
</script>
