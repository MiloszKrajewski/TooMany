<template>
	<select v-model="value" @change="onChange">
		<option v-for="Theme in themes" :id="Theme" :key="Theme">
			{{ Theme }}
		</option>
	</select>
</template>

<script lang="ts">
import {
	defineComponent,
	useMeta,
	watch,
	onMounted,
} from '@nuxtjs/composition-api';

export default defineComponent({
	props: {
		inlineProperties: {
			type: String,
			default: '',
		},
		themes: {
			type: Array,
			default: () => [],
		},
		value: {
			type: String,
			required: true,
		},
	},
	setup(props, { emit }) {
		const { bodyAttrs } = useMeta();

		watch(
			() => ({
				theme: props.value,
				properties: props.inlineProperties,
			}),
			({ theme, properties }: { theme: string; properties: string }) => {
				bodyAttrs.value = {
					'data-theme': theme,
					style: properties,
				};
			},
		);
		onMounted(() => {
			bodyAttrs.value = {
				'data-theme': props.value,
				style: props.inlineProperties,
			};
		});

		function onChange(event: { target: { value: string } }) {
			emit('onChange', event.target.value);
		}

		return { onChange };
	},
	head: {},
});
</script>
