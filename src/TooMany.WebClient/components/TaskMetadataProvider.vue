<script lang="ts">
import { defineComponent, provide, readonly } from '@nuxtjs/composition-api';
import { useTaskMeta } from '~/hooks';

export const StateSymbol = Symbol('All task metadata');

export default defineComponent({
	setup() {
		const taskMeta = useTaskMeta(null);
		provide(StateSymbol, readonly(taskMeta));
	},
	render() {
		return typeof this.$slots.default === 'function'
			? this.$slots.default()
			: this.$slots.default;
	},
});
</script>
