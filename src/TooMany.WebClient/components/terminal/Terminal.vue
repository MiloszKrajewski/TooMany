<template>
	<Fragment>
		<header>
			<h3>Terminals</h3>
		</header>
		<Select :options="names" :value="name" @onChange="onSelect" />
		<Form :initial-state="selection" :is-new="isNew" @onSave="onSave" />
	</Fragment>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import {
	defineComponent,
	computed,
	ComputedRef,
	ref,
} from '@nuxtjs/composition-api';
import Form from './form/Form.vue';
import { Terminal } from './types';
import Select from '~/components/Select.vue';

enum localStorageKeys {
	terminals = 'terminals',
}

const newName = 'New';

export default defineComponent({
	components: { Fragment, Select, Form },
	setup() {
		const name = ref(newName);
		const serializedTerminals = localStorage.getItem(
			localStorageKeys.terminals,
		);
		let terminals: Terminal.Manifests = [];
		if (serializedTerminals) {
			terminals = JSON.parse(serializedTerminals);
		}
		const names = computed(() => [newName, ...terminals.map((t) => t.name)]);
		const selection: ComputedRef<Terminal.Manifest> = computed(() => {
			const terminal = terminals.find((t) => t.name === name.value);
			if (!terminal)
				return {
					name: '',
					processes: [],
				};
			return terminal;
		});
		function onSelect(value: string) {
			name.value = value;
		}

		const isNew = computed(() => name.value === newName);

		return {
			name,
			names,
			selection,
			onSelect,
			onSave: console.log,
			isNew,
		};
	},
});
</script>
