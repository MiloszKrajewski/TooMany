<template>
	<Fragment>
		<header>
			<h3>Terminals</h3>
		</header>
		<Select v-model="name" :options="names" />
		<Form :terminal="terminal" @onCreate="onCreate" @onUpdate="onUpdate" />
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import {
	defineComponent,
	computed,
	ref,
	inject,
} from '@nuxtjs/composition-api';
import Form from './form/Form.vue';
import {
	StateSymbol as TerminalStateSymbol,
	NamesSymbol as TerminalNamesSymbol,
	CreateSymbol as CreateTerminalSymbol,
	UpdateSymbol as UpdateTerminalSymbol,
} from './TerminalProvider.vue';
import Select from '~/components/Select.vue';
import { Ref } from '~/types';
import { Terminal } from '~/components/terminal/types';

const newName = 'New';

const noop = () => {};
export default defineComponent({
	components: { Fragment, Select, Form },
	setup(_, { root }) {
		const terminalNames = inject<Ref<Terminal.Names>>(TerminalNamesSymbol) || {
			value: [],
		};
		const onCreateTerminal =
			inject<Terminal.onCreate>(CreateTerminalSymbol) || noop;
		const onUpdateTerminal =
			inject<Terminal.onUpdate>(UpdateTerminalSymbol) || noop;

		const name = ref<string>(newName);
		const names = computed<string[]>(() => [newName, ...terminalNames.value]);

		const terminals = inject<Ref<Terminal.Manifest>>(TerminalStateSymbol) || {
			value: {},
		};
		const tasks = computed<Terminal.Task[]>(
			() => terminals.value[name.value] || [],
		);

		function onCreate(manifest: Terminal.Manifest) {
			onCreateTerminal(manifest);
			root.$nextTick(() => {
				const [newName] = Object.keys(manifest);
				name.value = newName;
			});
		}
		function onUpdate(manifest: Terminal.Manifest) {
			onUpdateTerminal(manifest, name.value);
			root.$nextTick(() => {
				const [newName] = Object.keys(manifest);
				name.value = newName;
			});
		}

		const terminal = computed(() => ({
			name: name.value === newName ? '' : name.value,
			tasks: tasks.value,
		}));

		return {
			name,
			names,
			onCreate,
			onUpdate,
			terminal,
		};
	},
});
</script>
