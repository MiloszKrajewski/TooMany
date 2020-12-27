<template>
	<form @submit.prevent>
		<dl>
			<label for="name">
				<dt>name</dt>
				<dd>
					<input v-model="name" name="name" type="text" />
				</dd>
			</label>
			<label for="executable">
				<dt>executable</dt>
				<dd>
					<input v-model="executable" name="executable" type="text" />
				</dd>
			</label>
			<label for="arguments">
				<dt>arguments</dt>
				<dd>
					<input v-model="args" name="arguments" type="text" />
				</dd>
			</label>
			<label for="directory">
				<dt>directory</dt>
				<dd>
					<input v-model="directory" name="directory" type="text" />
				</dd>
			</label>
			<Wrapper :is-add-visible="envVars.length <= 0" @onAdd="addEnvVar">
				<template slot:title>environment key/value</template>
				<Entry
					v-for="(envVar, index) in envVars"
					:key="envVar.id"
					:index="index"
					@onAdd="addEnvVar"
					@onRemove="removeEnvVar"
				>
					<input
						v-model="envVar.key"
						name="key"
						type="text"
						placeholder="key"
					/>
					<input
						v-model="envVar.value"
						name="value"
						type="text"
						placeholder="value"
					/>
				</Entry>
			</Wrapper>
			<Wrapper :is-add-visible="tags.length <= 0" @onAdd="addTag">
				<template slot:title>tags</template>
				<Entry
					v-for="(tag, index) in tags"
					:key="tag.id"
					:index="index"
					@onAdd="addTag"
					@onRemove="removeTag"
				>
					<input
						v-model="tag.value"
						name="value"
						type="text"
						placeholder="tag"
					/>
				</Entry>
			</Wrapper>
		</dl>
		<input
			id="delete"
			type="submit"
			value="Delete"
			:disabled="isNewTask"
			@click="onDelete"
		/>
		<input
			id="save"
			type="submit"
			value="Save"
			:disabled="isNameInvaild"
			@click="onSave"
		/>
	</form>
</template>

<script lang="ts">
import {
	defineComponent,
	watch,
	toRefs,
	reactive,
	computed,
} from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import { Entry, Wrapper } from './addative';

type TPropEnvVars = Record<string, string>;
interface IEnvVar {
	key: string;
	value: string;
	id: string;
}
type TEnvVars = IEnvVar[];
interface IEnvVarValue {
	key: string;
	value: string;
}
type TEnvVarValues = IEnvVarValue[];

type TPropTags = string[];
interface ITag {
	value: string;
	id: string;
}
type TTags = ITag[];
type TTagValues = string[];

interface TTask {
	name: string;
	executable: string;
	args: string;
	directory: string;
	envVars: Record<string, string>;
	tags: string[];
}

function addEntry<T>({
	index,
	entry,
	entries,
}: {
	index?: number;
	entry: T;
	entries: T[];
}): T[] {
	const output = [];
	if (typeof index === 'number') {
		for (let i = 0; i < entries.length; i++) {
			output.push(entries[i]);
			if (i === index) {
				output.push(entry);
			}
		}
	} else {
		output.push(entry);
	}
	return output;
}

function removeEntry<T>({ index, entries }: { index: number; entries: T[] }) {
	return entries.filter((_, i) => i !== index);
}

function envVarsFromProps(envVars: TPropEnvVars): TEnvVars {
	return Object.entries(envVars).map(([key, value]) => ({
		key,
		value,
		id: uuidv4(),
	}));
}
function tagsFromProps(tags: TPropTags): TTags {
	return tags.map((value) => ({
		value,
		id: uuidv4(),
	}));
}
function TaskToState({
	name = '',
	executable = '',
	args = '',
	directory = '',
	envVars = {},
	tags = [],
}: TTask) {
	return {
		name,
		executable,
		args,
		directory,
		envVars: envVarsFromProps(envVars),
		tags: tagsFromProps(tags),
	};
}

export default defineComponent({
	components: { Entry, Wrapper },
	props: {
		names: {
			type: Array as () => string[],
			default: () => [],
		},
		task: {
			type: Object as () => TTask,
			default: () => ({}),
		},
		isNewTask: {
			type: Boolean,
			default: false,
		},
	},
	setup(props, { emit }) {
		const state = reactive(TaskToState(props.task));
		watch(
			() => props.task.name,
			() => {
				const task = TaskToState(props.task);
				state.name = task.name;
				state.executable = task.executable;
				state.args = task.args;
				state.directory = task.directory;
				state.envVars = task.envVars;
				state.tags = task.tags;
			},
		);
		const isNameInvaild = computed(() => !state.name.trim().toLowerCase());

		function addEnvVar(index?: number) {
			state.envVars = addEntry<IEnvVar>({
				index,
				entry: { key: '', value: '', id: uuidv4() },
				entries: state.envVars,
			});
		}

		function removeEnvVar(index: number) {
			state.envVars = removeEntry<IEnvVar>({
				index,
				entries: state.envVars,
			});
		}

		function addTag(index?: number) {
			state.tags = addEntry<ITag>({
				index,
				entry: { value: '', id: uuidv4() },
				entries: state.tags,
			});
		}
		function removeTag(index: number) {
			state.tags = removeEntry<ITag>({
				index,
				entries: state.tags,
			});
		}

		function onDelete() {
			emit('onDelete', {
				name: props.task.name,
			});
		}

		function onSave() {
			const vallidEnvVars: TEnvVarValues = state.envVars
				.map(({ key, value }) => ({ key, value }))
				.filter(({ key, value }) => Boolean(key) && Boolean(value));

			const validTags: TTagValues = state.tags
				.map(({ value }) => value)
				.filter(Boolean);

			emit('onSave', {
				...state,
				tags: validTags,
				EnvVar: vallidEnvVars,
			});
		}

		return {
			...toRefs(state),
			isNameInvaild,
			onSave,
			onDelete,
			addEnvVar,
			removeEnvVar,
			addTag,
			removeTag,
		};
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
