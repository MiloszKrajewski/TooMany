<template>
	<Fragment>
		<header>
			<h3>Terminals</h3>
		</header>
		<Select v-model="name" :options="names" />
		<Form
			:terminal="terminal"
			:names="terminalNames"
			:is-new="isNew"
			@onCreate="onCreate"
			@onUpdate="onUpdate"
		/>
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import {
	defineComponent,
	computed,
	ref,
	inject,
	useContext,
} from '@nuxtjs/composition-api';
import Form from './form/Form.vue';
import {
	StateSymbol as TerminalStateSymbol,
	NamesSymbol as TerminalNamesSymbol,
	CreateSymbol as CreateTerminalSymbol,
	UpdateSymbol as UpdateTerminalSymbol,
} from './TerminalProvider.vue';
import Select from '~/components/Select.vue';
import { Ref, Terminal } from '~/types';

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

		const terminals = inject<Ref<Terminal.Manifests>>(TerminalStateSymbol) || {
			value: {},
		};
		const { params, route } = useContext();
		const name = ref<string>(
			(() => {
				if (params.value.id && route.value.name === 'terminal-id') {
					return terminals.value[params.value.id].name;
				}
				return newName;
			})(),
		);
		const names = computed<string[]>(() => [newName, ...terminalNames.value]);

		const terminal = computed(() => {
			const terminalSet = Object.values(terminals.value);
			const terminal = terminalSet.find((t) => t.name === name.value);
			if (!terminal) {
				return {
					id: '',
					name: '',
					tasks: [],
				};
			}
			return terminal;
		});
		function onCreate(manifest: Terminal.Manifest) {
			onCreateTerminal(manifest);
			root.$nextTick(() => {
				name.value = manifest.name;
			});
		}
		function onUpdate(manifest: Terminal.Manifest) {
			onUpdateTerminal(manifest, terminal.value.id);
			root.$nextTick(() => {
				name.value = manifest.name;
			});
		}

		const isNew = computed(() => name.value === newName);

		return {
			name,
			names,
			onCreate,
			onUpdate,
			terminal,
			isNew,
			terminalNames,
		};
	},
});
</script>
