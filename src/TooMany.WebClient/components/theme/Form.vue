<template>
	<form @submit.prevent="onSave">
		<dl>
			<dt>
				<label for="name">name</label>
			</dt>
			<dd>
				<input id="name" type="text" :value="name" />
			</dd>
			<Fragment v-for="proptery in properties" :key="proptery">
				<dt>
					<label :for="proptery">{{ proptery }}</label>
				</dt>
				<dd>
					<input
						:id="proptery"
						:value="values[proptery]"
						:placeholder="proptery"
						:type="types[proptery]"
					/>
				</dd>
			</Fragment>
		</dl>
		<input id="save" type="submit" value="Update" />
	</form>
</template>

<script lang="ts">
import { Fragment } from 'vue-fragment';
import { defineComponent, computed } from '@nuxtjs/composition-api';
import { SupportedCssProperty } from '~/plugins/Theme.client';

export default defineComponent({
	components: { Fragment },
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
