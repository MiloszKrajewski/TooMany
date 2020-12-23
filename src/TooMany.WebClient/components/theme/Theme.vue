<template>
	<div>
		<Select
			:inline-properties="inlineProperties"
			:themes="avalibleThemes"
			:value="selection"
			@onChange="onSelect"
		/>
		<button @click="onAdd">New Theme</button>
		<button v-if="!isFirstPartySelection && !isAdd" @click="onDelete">
			Delete Theme
		</button>
		<Form
			v-if="!isFirstPartySelection && !isAdd"
			:themes="avalibleThemes"
			:values="selectedProperties"
			:name="selection"
			@onSave="onSave"
		/>
		<h1 v-if="isAdd">adding</h1>
		<Form v-if="isAdd" is-new @onSave="onSave" />
	</div>
</template>

<script lang="ts">
import { defineComponent, ref } from '@nuxtjs/composition-api';
import Select from './Select.vue';
import Form from './Form.vue';
import { useTheme } from '~/hooks';
import { SupportedCssProperty } from '~/hooks/useTheme';
import { Ref } from '~/@types';

function useToggle(
	initialValue = false,
): [Ref<boolean>, (value: boolean) => void] {
	const isToggled: Ref<boolean> = ref(initialValue);

	const onToggle = (value: boolean): void => {
		if (typeof value === 'boolean') {
			isToggled.value = value;
		} else {
			isToggled.value = !isToggled.value;
		}
	};

	return [isToggled, onToggle];
}

export default defineComponent({
	components: { Select, Form },
	setup() {
		const [isAdd, onAdd] = useToggle();
		const Theme = useTheme();

		function onSaveExtended(
			name: string,
			values: SupportedCssProperty.Keys,
			isNew: boolean,
		) {
			if (isAdd) onAdd(false);
			Theme.onSave(name, values, isNew);
		}

		function onSelectExtended(theme: string) {
			if (isAdd) onAdd(false);
			Theme.onSelect(theme);
		}
		function onDeleteExtended() {
			const isTheUserSure = confirm(
				`Are you sure you want to delete: ${Theme.selection.value}`,
			);
			if (isTheUserSure) {
				Theme.onDelete();
			}
		}

		return {
			avalibleThemes: Theme.all,
			selection: Theme.selection,
			onSelect: onSelectExtended,
			onSave: onSaveExtended,
			inlineProperties: Theme.inlineProperties,
			selectedProperties: Theme.selectedProperties,
			isFirstPartySelection: Theme.isFirstPartySelection,
			isAdd,
			onAdd,
			onDelete: onDeleteExtended,
		};
	},
});
</script>
