<template>
	<form @submit.prevent="onSave">
		<label for="name">
			Name:
			<input id="name" type="text" :value="name" />
		</label>
		<dl>
			<label v-for="proptery in properties" :key="proptery">
				<dt>{{ proptery }}</dt>
				<dd>
					<input
						:id="proptery"
						:value="values[proptery]"
						:placeholder="proptery"
						:type="types[proptery]"
					/></dd
			></label>
		</dl>
		<input id="save" type="submit" value="Update" />
	</form>
</template>

<script lang="ts">
import { defineComponent, computed } from '@nuxtjs/composition-api';
import { SupportedCssProperty } from '~/plugins/Theme.client';

export default defineComponent({
	props: {
		name: {
			type: String,
		},
		themes: {
			type: Array,
			default: () => [],
		},
		values: {
			type: Object,
			default: () => ({}),
		},
		isNew: {
			type: Boolean,
			default: false,
		},
	},
	setup(props, { emit }) {
		function onSave(event: {
			target: { elements: Record<string, HTMLInputElement> };
		}) {
			const name = event.target.elements.name;
			const properties = SupportedCssProperty.Keys;
			const values: Record<string, string> = {};
			for (const [proptery] of Object.entries(properties)) {
				values[proptery] = event.target.elements[proptery].value;
			}
			emit('onSave', name.value, values, props.isNew);
		}

		const nameErrorPattern = computed(
			() => new RegExp(`(?!${props.themes.join('|')})`, 'gi'),
		);

		return {
			nameErrorPattern,
			onSave,
			properties: SupportedCssProperty.Keys,
			types: SupportedCssProperty.Types,
		};
	},
});
</script>
