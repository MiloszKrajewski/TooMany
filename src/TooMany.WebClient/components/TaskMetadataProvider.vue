<script lang="ts">
import {
	defineComponent,
	provide,
	readonly,
	computed,
} from '@nuxtjs/composition-api';
import { useTaskMeta } from '~/hooks';

export const StateSymbol = 'All task metadata';
export const NamesSymbol = 'All task names';

export default defineComponent({
	setup() {
		const taskMeta = useTaskMeta(null);
		provide(StateSymbol, readonly(taskMeta));
		const names = computed(() => {
			const taskNames = taskMeta?.value.map((t) => t.name);
			if (!taskNames || taskNames.length <= 0) {
				return [];
			}
			return taskNames;
		});
		provide(NamesSymbol, names);
	},
	render() {
		return typeof this.$slots.default === 'function'
			? this.$slots.default()
			: this.$slots.default;
	},
});
</script>
