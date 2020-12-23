<template>
	<div>
		<label v-for="Theme in themes" :key="Theme" :for="Theme">
			<input
				:id="Theme"
				v-model="value"
				type="radio"
				name="theme"
				:value="Theme"
				@click="handleToggle"
			/>{{ Theme }}
		</label>
	</div>
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

		function handleToggle(event: { target: { value: string } }) {
			emit('onChange', event.target.value);
		}

		return {
			handleToggle,
		};
	},
	head: {},
});
</script>
