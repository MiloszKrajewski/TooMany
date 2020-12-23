<template>
	<div>
		<Select
			:inline-properties="inlineProperties"
			:themes="avalibleThemes"
			:value="selection"
			@onChange="onSelect"
		/>
		<Form
			v-if="!isFirstPartySelection"
			:values="selectedProperties"
			@onSave="onSave"
		/>
	</div>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';
import Select from './Select.vue';
import Form from './Form.vue';
import { useTheme } from '~/hooks';

export default defineComponent({
	components: { Select, Form },
	setup() {
		const Theme = useTheme();

		return {
			avalibleThemes: Theme.all,
			selection: Theme.selection,
			onSelect: Theme.onSelect,
			onSave: Theme.onSave,
			inlineProperties: Theme.inlineProperties,
			selectedProperties: Theme.selectedProperties,
			isFirstPartySelection: Theme.isFirstPartySelection,
		};
	},
});
</script>
