<template>
	<main>
		<nav>
			<aside id="controls">
				<button
					class="control"
					:class="{ active: isThemesVisible }"
					@click="onToggleThemes"
				>
					Themes
				</button>
				<button
					class="control"
					:class="{ active: isTaskEditorVisible }"
					@click="onToggleTaskEditor"
				>
					Tasks
				</button>
			</aside>
			<aside v-if="isAnyOpen" id="content">
				<Theme v-if="isThemesVisible" />
				<Editor v-if="isTaskEditorVisible" />
			</aside>
		</nav>
		<section>
			<Nuxt />
		</section>
	</main>
</template>

<script lang="ts">
import { defineComponent, computed } from '@nuxtjs/composition-api';
import { useToggle } from '~/hooks';
import Theme from '~/components/theme/Theme.vue';
import Editor from '~/components/task/Task.vue';

enum controls {
	theme = 'theme',
	taskEditor = 'taskEditor',
}

export default defineComponent({
	components: { Theme, Editor },
	setup() {
		const [isThemesVisible, onToggleThemes] = useToggle(false);
		const [isTaskEditorVisible, onToggleTaskEditor] = useToggle();

		function onToggleControl(target: string) {
			switch (target) {
				case controls.theme: {
					onToggleThemes();
					onToggleTaskEditor(false);
					break;
				}
				case controls.taskEditor: {
					onToggleThemes(false);
					onToggleTaskEditor();
					break;
				}
			}
		}

		const isAnyOpen = computed(
			() => isThemesVisible.value || isTaskEditorVisible.value,
		);

		return {
			isThemesVisible,
			onToggleThemes: () => onToggleControl(controls.theme),
			isTaskEditorVisible,
			onToggleTaskEditor: () => onToggleControl(controls.taskEditor),
			isAnyOpen,
		};
	},
});
</script>

<style lang="postcss" scoped>
main {
	width: 100vw;
	display: flex;
	nav {
		background-color: var(--background-color);
		min-height: 100vh;
		max-width: 100%;
		z-index: 0;
		display: flex;
		aside {
			&#controls {
				background-color: var(--background-color);
				display: flex;
				flex-direction: column;
				.control {
					outline: none;
					border: none;
					border-left: 0.25rem solid transparent;
					padding: 0.5rem;
					padding-right: 0.75rem;
					height: 3rem;
					margin-bottom: 0.5rem;
					&.active {
						border-left-color: var(--dark-background-color);
					}
				}
			}
			&#content {
				background-color: var(--dark-background-color);
				min-width: 20rem;
				padding: 0.5rem 0.75rem;
			}
		}
	}
	section {
		z-index: 1;
		min-height: 100vh;
		flex: 1;
	}
}
</style>
