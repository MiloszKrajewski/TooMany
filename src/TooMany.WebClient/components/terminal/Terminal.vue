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
import { defineComponent, computed, ref } from '@nuxtjs/composition-api';
import Form from './form/Form.vue';
import { Terminal } from './types';
import Select from '~/components/Select.vue';

enum localStorageKeys {
	terminals = 'terminals',
}

const newName = 'New';

export default defineComponent({
	components: { Fragment, Select, Form },
	setup(_, { root }) {
		const serializedTerminals = localStorage.getItem(
			localStorageKeys.terminals,
		);
		let deserializedTerminals: Terminal.DBManifest = {};
		if (serializedTerminals) {
			deserializedTerminals = JSON.parse(serializedTerminals);
		}
		const terminals = ref(
			Object.entries(deserializedTerminals).map(([name, tasks]) => ({
				name,
				tasks,
			})),
		);

		const name = ref<string>(newName);
		const names = computed<string[]>(() => {
			const tNames = terminals.value.map((t) => t.name);
			return [newName, ...tNames];
		});

		const terminal = computed<Terminal.Manifest>(() => {
			const terminal = terminals.value.find((t) => t.name === name.value);
			if (!terminal)
				return {
					name: '',
					tasks: [],
				};
			return terminal;
		});

		function updateLocalStorage(updatedTerminals: Terminal.Manifests) {
			const dbValue: Terminal.DBManifest = {};
			for (const terminal of updatedTerminals) {
				dbValue[terminal.name] = terminal.tasks;
			}

			localStorage.setItem(localStorageKeys.terminals, JSON.stringify(dbValue));
		}

		function transformTerminal(value: Terminal.Manifest) {
			return {
				name: value.name,
				tasks: value.tasks
					.filter((t) => t.include)
					.map((t) => {
						const result: Terminal.Task = {
							name: t.name,
							stdOut: t.stdOut,
							stdErr: t.stdErr,
						};
						if (t.filter) {
							result.filter = t.filter;
						}
						return result;
					}),
			};
		}

		function onCreate(newTerminal: Terminal.Manifest) {
			if (terminals.value.some((t) => t.name === newTerminal.name)) {
				return;
			}
			const transformedTerminal = transformTerminal(newTerminal);
			if (transformedTerminal.tasks.length <= 0) return;
			const updatedTerminals = [...terminals.value, transformedTerminal];
			updateLocalStorage(updatedTerminals);

			terminals.value = updatedTerminals;
			root.$nextTick(() => {
				name.value = transformedTerminal.name;
			});
		}
		function onUpdate(updatedTerminal: Terminal.Manifest) {
			if (!terminals.value.some((t) => t.name === updatedTerminal.name)) {
				onCreate(updatedTerminal);
				return;
			}
			const transformedTerminal = transformTerminal(updatedTerminal);
			if (transformedTerminal.tasks.length <= 0) return;
			const updatedTerminals = terminals.value.map((t) => {
				if (t.name === name.value) return transformedTerminal;
				return t;
			});
			updateLocalStorage(updatedTerminals);
			terminals.value = updatedTerminals;

			root.$nextTick(() => {
				name.value = transformedTerminal.name;
			});
		}

		return {
			name,
			names,
			terminal,
			onCreate,
			onUpdate,
		};
	},
});
</script>
