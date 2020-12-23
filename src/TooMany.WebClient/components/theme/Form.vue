<template>
	<form @submit.prevent="onSave">
		form
		<dl>
			<label v-for="proptery in properties" :key="proptery">
				<dt>{{ proptery }}</dt>
				<dd>
					<input
						:id="proptery"
						:value="values[proptery]"
						:placeholder="proptery"
						:type="SupportedCssPropertyTypes[proptery]"
					/></dd
			></label>
		</dl>
		<input id="save" type="submit" value="save" />
	</form>
</template>

<script lang="ts">
import { defineComponent } from '@nuxtjs/composition-api';
import {
	SupportedCssPropertyKeys,
	SupportedCssPropertyTypes,
} from '~/hooks/useTheme';

export default defineComponent({
	props: {
		values: {
			type: Object,
			default: () => ({}),
		},
	},
	setup(_, { emit }) {
		function onSave(event: {
			target: { elements: Record<string, HTMLInputElement> };
		}) {
			const properties = SupportedCssPropertyKeys;
			const values: Record<string, string> = {};
			for (const [proptery] of Object.entries(properties)) {
				values[proptery] = event.target.elements[proptery].value;
			}
			emit('onSave', values);
		}

		return {
			onSave,
			properties: SupportedCssPropertyKeys,
			SupportedCssPropertyTypes,
		};
	},
});
</script>
