<template>
	<Fragment>
		<div v-for="terminal in terminals" :key="terminal">
			<NuxtLink :to="`/terminal/${terminal}`">{{ terminal }}</NuxtLink>
		</div>
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import { defineComponent, inject } from '@nuxtjs/composition-api';

import { Ref } from '~/types';
import { Terminal } from '~/components/terminal/types';
import { NamesSymbol as TerminalNames } from '~/components/Terminal/TerminalProvider.vue';

export default defineComponent({
	components: { Fragment },
	setup() {
		const terminals = inject<Ref<Terminal.Names>>(TerminalNames);
		return { terminals };
	},
});
</script>
