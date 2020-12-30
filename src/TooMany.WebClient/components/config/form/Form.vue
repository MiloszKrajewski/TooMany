<template>
	<form @submit.prevent="onSave">
		<dl>
			<dt><label for="executable">executable</label></dt>
			<dd>
				<input id="executable" v-model="executable" type="text" />
			</dd>
			<dt><label for="directory">directory</label></dt>
			<dd>
				<input id="directory" v-model="directory" type="text" />
			</dd>
		</dl>
		<input id="save" type="submit" value="Save" />
	</form>
</template>

<script lang="ts">
import {
	defineComponent,
	reactive,
	toRefs,
	useContext,
} from '@nuxtjs/composition-api';

export default defineComponent({
	setup() {
		const { $UserConfig } = useContext();
		const task = reactive({
			executable: $UserConfig.task.executable,
			directory: $UserConfig.task.directory,
		});

		function onSave() {
			$UserConfig.setTaskConfig(task);
		}

		return { ...toRefs(task), onSave };
	},
});
</script>

<style lang="postcss" scoped>
fieldset {
	margin: 0;
	padding: 0;
	border: none;
}
</style>
