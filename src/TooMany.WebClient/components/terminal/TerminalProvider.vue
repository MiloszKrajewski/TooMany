<script lang="ts">
import {
	defineComponent,
	provide,
	readonly,
	ref,
	computed,
} from '@nuxtjs/composition-api';
import { Terminal } from './types';
import { useDeserializedLocalStorage } from '~/hooks';

export const StateSymbol = 'All terminals';
export const NamesSymbol = 'All terminal names';
export const CreateSymbol = 'Create a terminal, updating state and store';
export const UpdateSymbol = 'Update a terminal, updating state and store';

enum localStorageKeys {
	terminals = 'terminals',
}

export default defineComponent({
	setup() {
		const storedState = useDeserializedLocalStorage<Terminal.Manifest>(
			localStorageKeys.terminals,
		);
		const state = ref<Terminal.Manifest>(storedState || {});
		const names = computed<string[]>(() => Object.keys(state.value));

		function storeTerminal(terminals: Terminal.Manifest) {
			localStorage.setItem(
				localStorageKeys.terminals,
				JSON.stringify(terminals),
			);
		}

		function filterIncluded(tasks: Terminal.Task[]) {
			return tasks
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
				});
		}

		const onCreate: Terminal.onCreate = function (terminal) {
			const [[name, tasks]] = Object.entries(terminal);
			const filterTasks = filterIncluded(tasks);
			if (filterTasks.length <= 0) return;
			const updatedManifests = { ...state.value, [name]: filterTasks };
			storeTerminal(updatedManifests);
			state.value = updatedManifests;
		};

		const onUpdate: Terminal.onUpdate = function (terminal, initialName) {
			const [name] = Object.keys(terminal);
			if (name !== initialName && state.value[initialName]) {
				delete state.value[initialName];
			}
			onCreate(terminal);
		};

		provide(StateSymbol, readonly(state));
		provide(NamesSymbol, names);
		provide(CreateSymbol, onCreate);
		provide(UpdateSymbol, onUpdate);
	},
	render() {
		return typeof this.$slots.default === 'function'
			? this.$slots.default()
			: this.$slots.default;
	},
});
</script>
