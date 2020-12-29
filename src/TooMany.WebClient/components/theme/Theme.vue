<template>
	<div class="root">
		<header>
			<h3>Themes</h3>
		</header>
		<Select :options="avalibleThemes" :value="selection" @onChange="onSelect" />
		<button @click="onAdd">{{ isAdd ? 'Cancel' : 'Create' }}</button>
		<Form
			v-if="!isFirstPartySelection && !isAdd"
			:themes="avalibleThemes"
			:values="selectedProperties"
			:name="selection"
			@onSave="onSave"
		/>
		<Form v-if="isAdd" is-new @onSave="onSave" />
		<button v-if="!isFirstPartySelection && !isAdd" @click="onDelete">
			Delete
		</button>
	</div>
</template>

<script lang="ts">
import { defineComponent, useContext } from '@nuxtjs/composition-api';
import Select from './../Select.vue';
import Form from './Form.vue';
import { useToggle } from '~/hooks';
import { SupportedCssProperty } from '~/plugins/Theme.client';

export default defineComponent({
	components: { Select, Form },
	setup() {
		const [isAdd, onAdd] = useToggle();
		const ctx = useContext();
		const Theme = ctx.$Theme;

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

<style lang="postcss" scoped>
.root {
	min-height: 100%;
}
</style>