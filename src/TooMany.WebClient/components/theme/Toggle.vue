<template>
	<div>
		<label v-for="Theme in avalibleThemes" :key="Theme" :for="Theme">
			<input
				:id="Theme"
				v-model="selection"
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
import useTheme, { FirstPartyThemes } from '~/hooks/useTheme';

export default defineComponent({
	setup() {
		const { bodyAttrs } = useMeta();
		const Themes = useTheme();

		watch(
			() => ({
				theme: Themes.selection.value,
				properties: Themes.inlineProperties.value,
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
				'data-theme': Themes.selection.value,
				style: Themes.inlineProperties.value,
			};
		});

		function handleToggle(event: { target: { value: string } }) {
			Themes.onSelect(event.target.value);
		}

		return {
			FirstPartyThemes,
			handleToggle,
			avalibleThemes: Themes.all,
			selection: Themes.selection,
		};
	},
	head: {},
});
</script>
