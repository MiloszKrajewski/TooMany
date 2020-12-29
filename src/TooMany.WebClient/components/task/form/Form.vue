<template>
	<form @submit.prevent>
		<dl>
			<dt><label for="name">name</label></dt>
			<dd>
				<input id="name" v-model="name" type="text" />
			</dd>

			<dt><label for="executable">executable</label></dt>
			<dd>
				<input id="executable" v-model="executable" type="text" />
			</dd>

			<dt><label for="arguments">arguments</label></dt>
			<dd>
				<input id="arguments" v-model="args" type="text" />
			</dd>

			<dt><label for="directory">directory</label></dt>
			<dd>
				<input id="directory" v-model="directory" type="text" />
			</dd>

			<Wrapper
				:is-add-visible="environmentVariables.length <= 0"
				@onAdd="addEnvironmentVariable"
			>
				<template slot:title>environment key/value</template>
				<Entry
					v-for="(environmentVariable, index) in environmentVariables"
					:key="environmentVariable.id"
					:index="index"
					@onAdd="addEnvironmentVariable"
					@onRemove="removeEnvironmentVariable"
				>
					<input
						v-model="environmentVariable.key"
						type="text"
						placeholder="key"
					/>
					<input
						v-model="environmentVariable.value"
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
					<input v-model="tag.value" type="text" placeholder="tag" />
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
	useContext,
} from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import { Entry, Wrapper } from './addative';

type TPropEnvironmentVariables = Record<string, string>;
interface IEnvironmentVariables {
	key: string;
	value: string;
	id: string;
}
type TEnvironmentVariables = IEnvironmentVariables[];
interface IEnvironmentVariablesValue {
	key: string;
	value: string;
}
type TEnvironmentVariableValues = IEnvironmentVariablesValue[];

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
	arguments: string;
	directory: string;
	environment: Record<string, string>;
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

function environmentFromProps(
	environmentVariables: TPropEnvironmentVariables,
): TEnvironmentVariables {
	return Object.entries(environmentVariables).map(([key, value]) => ({
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
	arguments: args = '',
	directory = '',
	environment = {},
	tags = [],
}: TTask) {
	return {
		name,
		executable,
		args,
		directory,
		environmentVariables: environmentFromProps(environment),
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
		const { $UserConfig } = useContext();
		const state = reactive(
			TaskToState({
				name: props.task.name,
				executable: props.task.executable || $UserConfig.task.executable,
				arguments: props.task.arguments,
				directory: props.task.directory || $UserConfig.task.directory,
				environment: props.task.environment,
				tags: props.task.tags,
			}),
		);
		watch(
			() => props.task.name,
			() => {
				const incomingTask = props.task;
				if (props.isNewTask) {
					incomingTask.executable = $UserConfig.task.executable;
					incomingTask.directory = $UserConfig.task.directory;
				}
				const task = TaskToState(incomingTask);
				state.name = task.name;
				state.executable = task.executable;
				state.args = task.args;
				state.directory = task.directory;
				state.environmentVariables = task.environmentVariables;
				state.tags = task.tags;
			},
		);
		const isNameInvaild = computed(() => !state.name.trim().toLowerCase());

		function addEnvironmentVariable(index?: number) {
			state.environmentVariables = addEntry<IEnvironmentVariables>({
				index,
				entry: { key: '', value: '', id: uuidv4() },
				entries: state.environmentVariables,
			});
		}

		function removeEnvironmentVariable(index: number) {
			state.environmentVariables = removeEntry<IEnvironmentVariables>({
				index,
				entries: state.environmentVariables,
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
			const environmentVariables: Record<string, string> = {};
			for (const { key, value } of state.environmentVariables) {
				if (Boolean(key) && Boolean(value)) {
					environmentVariables[key] = value;
				}
			}

			const tags: TTagValues = state.tags
				.map(({ value }) => value)
				.filter(Boolean);

			const payload: TTask = {
				name: state.name,
				executable: state.executable,
				arguments: state.args,
				directory: state.directory,
				environment: environmentVariables,
				tags,
			};
			emit('onSave', payload);
		}

		return {
			...toRefs(state),
			isNameInvaild,
			onSave,
			onDelete,
			addEnvironmentVariable,
			removeEnvironmentVariable,
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
