<template>
	<main>
		<nav>
			<aside id="controls">
				<button
					class="control"
					:class="{ active: isThemeEditorVisible }"
					@click="onToggleThemeEditor"
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
				<button
					class="control"
					:class="{ active: isConfigEditorVisible }"
					@click="onToggleConfigEditor"
				>
					Config
				</button>
			</aside>
			<aside v-if="isAnyOpen" id="content">
				<ThemeEditor v-if="isThemeEditorVisible" />
				<TaskEditor v-if="isTaskEditorVisible" />
				<ConfigEditor v-if="isConfigEditorVisible" />
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
import ThemeEditor from '~/components/theme/Theme.vue';
import TaskEditor from '~/components/task/Task.vue';
import ConfigEditor from '~/components/config/Config.vue';

enum controls {
	themeEditor = 'themeEditor',
	taskEditor = 'taskEditor',
	configEditor = 'configEditor',
}

export default defineComponent({
	components: { ThemeEditor, TaskEditor, ConfigEditor },
	setup() {
		const [isThemeEditorVisible, onToggleThemeEditor] = useToggle(false);
		const [isTaskEditorVisible, onToggleTaskEditor] = useToggle();
		const [isConfigEditorVisible, onToggleConfigEditor] = useToggle();

		function onToggleControl(target: string) {
			switch (target) {
				case controls.themeEditor: {
					onToggleThemeEditor();
					onToggleTaskEditor(false);
					onToggleConfigEditor(false);
					break;
				}
				case controls.taskEditor: {
					onToggleThemeEditor(false);
					onToggleTaskEditor();
					onToggleConfigEditor(false);
					break;
				}
				case controls.configEditor: {
					onToggleThemeEditor(false);
					onToggleTaskEditor(false);
					onToggleConfigEditor();
					break;
				}
			}
		}

		const isAnyOpen = computed(
			() =>
				isThemeEditorVisible.value ||
				isTaskEditorVisible.value ||
				isConfigEditorVisible.value,
		);

		return {
			isThemeEditorVisible,
			onToggleThemeEditor: () => onToggleControl(controls.themeEditor),
			isTaskEditorVisible,
			onToggleTaskEditor: () => onToggleControl(controls.taskEditor),
			isConfigEditorVisible,
			onToggleConfigEditor: () => onToggleControl(controls.configEditor),
			isAnyOpen,
		};
	},
});
</script>

<style lang="postcss" scoped>
main {
	display: flex;
	nav {
		background-color: var(--background-color);
		min-height: 100vh;
		max-width: 100%;
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
		flex: 1;
		height: 100vh;
		width: 100vw;
		overflow-y: scroll;
	}
}
</style>
