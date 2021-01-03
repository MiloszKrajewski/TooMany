<template>
	<TerminalProvider>
		<TaskMetadataProvider>
			<main>
				<nav>
					<aside id="controls">
						<button
							class="control"
							:class="{ active: isTaskEditorVisible }"
							@click="onToggleTaskEditor"
						>
							Tasks
						</button>
						<button
							class="control"
							:class="{ active: isTerminalEditorVisible }"
							@click="onToggleTerminalEditor"
						>
							Terminals
						</button>
						<button
							class="control"
							:class="{ active: isConfigEditorVisible }"
							@click="onToggleConfigEditor"
						>
							Config
						</button>
						<button
							class="control"
							:class="{ active: isThemeEditorVisible }"
							@click="onToggleThemeEditor"
						>
							Themes
						</button>
					</aside>
					<aside v-if="isAnyOpen" id="content">
						<TaskEditor v-if="isTaskEditorVisible" />
						<TerminalEditor v-if="isTerminalEditorVisible" />
						<ConfigEditor v-if="isConfigEditorVisible" />
						<ThemeEditor v-if="isThemeEditorVisible" />
					</aside>
				</nav>
				<section>
					<Nuxt />
				</section>
			</main>
		</TaskMetadataProvider>
	</TerminalProvider>
</template>

<script lang="ts">
import { defineComponent, computed } from '@nuxtjs/composition-api';
import { useToggle } from '~/hooks';
import TerminalEditor from '~/components/terminal/Terminal.vue';
import ThemeEditor from '~/components/theme/Theme.vue';
import TaskEditor from '~/components/task/Task.vue';
import ConfigEditor from '~/components/config/Config.vue';
import TaskMetadataProvider from '~/components/TaskMetadataProvider.vue';
import TerminalProvider from '~/components/Terminal/TerminalProvider.vue';

enum controls {
	themeEditor = 'themeEditor',
	taskEditor = 'taskEditor',
	configEditor = 'configEditor',
	terminalEditor = 'terminalEditor',
}

export default defineComponent({
	components: {
		TaskMetadataProvider,
		TerminalProvider,
		TerminalEditor,
		ThemeEditor,
		TaskEditor,
		ConfigEditor,
	},
	setup() {
		const [isThemeEditorVisible, onToggleThemeEditor] = useToggle();
		const [isTaskEditorVisible, onToggleTaskEditor] = useToggle();
		const [isConfigEditorVisible, onToggleConfigEditor] = useToggle();
		const [isTerminalEditorVisible, onToggleTerminalEditor] = useToggle();

		function onToggleControl(target: string) {
			switch (target) {
				case controls.themeEditor: {
					onToggleThemeEditor();
					onToggleTaskEditor(false);
					onToggleConfigEditor(false);
					onToggleTerminalEditor(false);
					break;
				}
				case controls.taskEditor: {
					onToggleThemeEditor(false);
					onToggleTaskEditor();
					onToggleConfigEditor(false);
					onToggleTerminalEditor(false);
					break;
				}
				case controls.configEditor: {
					onToggleThemeEditor(false);
					onToggleTaskEditor(false);
					onToggleConfigEditor();
					onToggleTerminalEditor(false);
					break;
				}
				case controls.terminalEditor: {
					onToggleThemeEditor(false);
					onToggleTaskEditor(false);
					onToggleConfigEditor(false);
					onToggleTerminalEditor();
					break;
				}
			}
		}

		const isAnyOpen = computed(
			() =>
				isThemeEditorVisible.value ||
				isTaskEditorVisible.value ||
				isConfigEditorVisible.value ||
				isTerminalEditorVisible.value,
		);

		return {
			isThemeEditorVisible,
			onToggleThemeEditor: () => onToggleControl(controls.themeEditor),
			isTaskEditorVisible,
			onToggleTaskEditor: () => onToggleControl(controls.taskEditor),
			isConfigEditorVisible,
			onToggleConfigEditor: () => onToggleControl(controls.configEditor),
			isTerminalEditorVisible,
			onToggleTerminalEditor: () => onToggleControl(controls.terminalEditor),
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
				padding: 0.5rem 0.75rem;
			}
		}
	}
	section {
		flex: 1;
		height: 100vh;
	}
}
</style>
