<script lang="ts">
import {
	defineComponent,
	provide,
	readonly,
	ref,
	computed,
} from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import { Terminal } from '~/types';
import { useDeserializedLocalStorage } from '~/hooks';

export const StateSymbol = 'All terminals';
export const IdsSymbol = 'All terminal ids';
export const NamesSymbol = 'All terminal names';
export const CreateSymbol = 'Create a terminal, updating state and store';
export const UpdateSymbol = 'Update a terminal, updating state and store';

enum localStorageKeys {
	terminals = 'terminals',
}

export default defineComponent({
	setup() {
		const storedState = useDeserializedLocalStorage<Terminal.Manifests>(
			localStorageKeys.terminals,
		);
		const state = ref(storedState || {});
		const ids = computed(() => Object.keys(state.value));
		const names = computed(() => {
			const internalState = Object.values(state.value);
			if (internalState.length <= 0) return [];
			return internalState.map((t) => t.name);
		});

		function storeTerminals(terminals: Terminal.Manifests) {
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

		const onCreate: Terminal.onCreate = function (terminal, id = uuidv4()) {
			const filterTasks = filterIncluded(terminal.tasks);
			if (filterTasks.length <= 0) return;
			const updatedManifests = {
				...state.value,
				[id]: {
					id,
					name: terminal.name,
					tasks: filterTasks,
				},
			};
			storeTerminals(updatedManifests);
			state.value = updatedManifests;
		};

		provide(StateSymbol, readonly(state));
		provide(NamesSymbol, names);
		provide(IdsSymbol, ids);
		provide(CreateSymbol, onCreate);
		provide(UpdateSymbol, onCreate);
	},
	render() {
		return typeof this.$slots.default === 'function'
			? this.$slots.default()
			: this.$slots.default;
	},
});
</script>
