<template>
	<form @submit.prevent="onSave">
		<dl>
			<label for="name">
				<dt>name</dt>
				<dd>
					<input
						id="name"
						ref="name"
						name="name"
						type="text"
						title="Three letter country code"
						@input="onNameInput"
					/>
				</dd>
			</label>
			<label for="executable">
				<dt>executable</dt>
				<dd>
					<input id="executable" type="text" :value="executable" />
				</dd>
			</label>
			<label for="arguments">
				<dt>arguments</dt>
				<dd>
					<input id="arguments" type="text" :value="args" />
				</dd>
			</label>
			<label for="directory">
				<dt>directory</dt>
				<dd>
					<input id="directory" type="text" :value="directory" />
				</dd>
			</label>
			<Wrapper :is-add-visible="_envVars.length <= 0" @onAdd="addEnvVar">
				<template slot:title>environment key/value</template>
				<Entry
					v-for="(envVar, index) in _envVars"
					:key="envVar.id"
					:index="index"
					@onAdd="addEnvVar"
					@onRemove="removeEnvVar"
				>
					<fieldset ref="envVars">
						<input name="key" type="text" placeholder="key" />
						<input name="value" type="text" placeholder="value" />
					</fieldset>
				</Entry>
			</Wrapper>
			<Wrapper :is-add-visible="_tags.length <= 0" @onAdd="addTag">
				<template slot:title>tags</template>
				<Entry
					v-for="(tag, index) in _tags"
					:key="tag.id"
					:index="index"
					@onAdd="addTag"
					@onRemove="removeTag"
				>
					<fieldset ref="tags">
						<input name="value" type="text" placeholder="tag" />
					</fieldset>
				</Entry>
			</Wrapper>
		</dl>
		<input id="delete" type="submit" value="Delete" />
		<input id="update" type="submit" value="Update" />
		<input id="create" type="submit" value="Create" :disabled="isNameInvaild" />
	</form>
</template>

<script lang="ts">
import {
	defineComponent,
	ref,
	watch,
	onMounted,
} from '@nuxtjs/composition-api';
import { v4 as uuidv4 } from 'uuid';
import { Entry, Wrapper } from './addative';
import { Ref } from '~/@types';

type TPropEnvVars = Record<string, string>;
interface IEnvVar {
	key: string;
	value: string;
	id: string;
}
type TEnvVars = IEnvVar[];
type TRefEnvVars = Ref<TEnvVars>;

interface EnvVarFieldSetElements extends HTMLCollection {
	key: HTMLInputElement;
	value: HTMLInputElement;
}
interface EnvVarFieldSetElement extends HTMLFieldSetElement {
	elements: EnvVarFieldSetElements;
}

type TPropTags = string[];
interface ITag {
	value: string;
	id: string;
}
type TTags = ITag[];
type TRefTags = Ref<TTags>;

interface TagFieldSetElements extends HTMLCollection {
	value: HTMLInputElement;
}
interface TagFieldSetElement extends HTMLFieldSetElement {
	elements: TagFieldSetElements;
}

interface ComponentRefs {
	name?: HTMLInputElement;
	envVars?: NodeListOf<EnvVarFieldSetElement>;
	tags?: NodeListOf<TagFieldSetElement>;
}

function setInitialEnvVarInputValues(
	envVars: TEnvVars,
	{ envVars: fieldsets }: ComponentRefs,
) {
	if (!fieldsets) {
		return;
	}
	for (let index = 0; index < envVars.length; index++) {
		const envVar = envVars[index];
		const fieldset = fieldsets[index];
		if (!fieldset.elements) {
			throw new Error(`The ${envVar.id} fieldset has no children.`);
		}
		const { key, value } = fieldset.elements;
		key.value = envVar.key;
		value.value = envVar.value;
	}
}

interface IEnvVarValue {
	key: string;
	value: string;
}
type TEnvVarValues = IEnvVarValue[];
function getEnvVarInputValues(
	envVars: TEnvVars,
	{ envVars: fieldsets }: ComponentRefs,
): TEnvVarValues | undefined {
	if (!fieldsets) {
		return;
	}
	const output = [];
	for (let index = 0; index < envVars.length; index++) {
		const envVar = envVars[index];
		const fieldset = fieldsets[index];
		if (!fieldset.elements) {
			throw new Error(`The ${envVar} fieldset has no children.`);
		}
		const { key, value } = fieldset.elements;
		output.push({ key: key.value, value: value.value });
	}
	return output;
}

function setInitialTagInputValues(
	tags: TTags,
	{ tags: fieldsets }: ComponentRefs,
) {
	if (!fieldsets) {
		return;
	}
	for (let index = 0; index < tags.length; index++) {
		const tag = tags[index];
		const fieldset = fieldsets[index];
		if (!fieldset.elements) {
			throw new Error(`The ${tag} fieldset has no children.`);
		}
		const { value } = fieldset.elements;
		value.value = tag.value;
	}
}

type TTagValues = string[];
function getTagInputValues(
	tags: TTags,
	{ tags: fieldsets }: ComponentRefs,
): TTagValues | undefined {
	if (!fieldsets) {
		return;
	}
	const output = [];
	for (let index = 0; index < tags.length; index++) {
		const tag = tags[index];
		const fieldset = fieldsets[index];
		if (!fieldset.elements) {
			throw new Error(`The ${tag} fieldset has no children.`);
		}
		const { value } = fieldset.elements;
		output.push(value.value);
	}
	return output;
}

export default defineComponent({
	components: { Entry, Wrapper },
	props: {
		names: {
			type: Array as () => string[],
			default: () => [],
		},
		name: {
			type: String,
			default: '',
		},
		executable: {
			type: String,
			default: '',
		},
		args: {
			type: String,
			default: '',
		},
		directory: {
			type: String,
			default: '',
		},
		envVars: {
			type: Object as () => TPropEnvVars,
			default: () => ({}),
		},
		tags: {
			type: Array as () => TPropTags,
			default: () => [],
		},
	},
	setup(props, { refs, root, emit }) {
		function tagsFromProps(tags: TPropTags): TTags {
			return tags.map((value) => ({
				value,
				id: uuidv4(),
			}));
		}
		const _tags: TRefTags = ref(tagsFromProps(props.tags));
		onMounted(() => setInitialTagInputValues(_tags.value, refs));
		watch(
			() => props.tags,
			(tags) => {
				_tags.value = tagsFromProps(tags);
				root.$nextTick(() => setInitialTagInputValues(_tags.value, refs));
			},
		);

		function envVarsFromProps(envVars: TPropEnvVars): TEnvVars {
			return Object.entries(envVars).map(([key, value]) => ({
				key,
				value,
				id: uuidv4(),
			}));
		}
		const _envVars: TRefEnvVars = ref(envVarsFromProps(props.envVars));
		onMounted(() => setInitialEnvVarInputValues(_envVars.value, refs));
		watch(
			() => props.envVars,
			(envVars) => {
				_envVars.value = envVarsFromProps(envVars);
				root.$nextTick(() => setInitialEnvVarInputValues(_envVars.value, refs));
			},
		);

		const isNameInvaild = ref(true);
		function validateName(value: string) {
			const name = value.trim().toLowerCase();
			if (!name) {
				isNameInvaild.value = true;
				return;
			}
			isNameInvaild.value = props.names
				.map((name) => name.toLowerCase())
				.includes(name);
		}
		function onNameInput(event: { target: { value: string } }) {
			validateName(event.target.value);
		}

		function setInitialNameInputValue(name: string, refs: ComponentRefs) {
			validateName(name);
			if (refs.name) {
				refs.name.value = name;
			}
		}
		onMounted(() => {
			setInitialNameInputValue(props.name, refs);
		});
		watch(
			() => props.name,
			(name) => {
				root.$nextTick(() => {
					setInitialNameInputValue(name, refs);
				});
			},
		);

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
		function removeEntry<T>({
			index,
			entries,
		}: {
			index: number;
			entries: T[];
		}) {
			return entries.filter((_, i) => i !== index);
		}

		function addEnvVar(index?: number) {
			_envVars.value = addEntry<IEnvVar>({
				index,
				entry: { key: '', value: '', id: uuidv4() },
				entries: _envVars.value,
			});
		}

		function removeEnvVar(index: number) {
			_envVars.value = removeEntry<IEnvVar>({
				index,
				entries: _envVars.value,
			});
		}

		function addTag(index?: number) {
			_tags.value = addEntry<ITag>({
				index,
				entry: { value: '', id: uuidv4() },
				entries: _tags.value,
			});
		}
		function removeTag(index: number) {
			_tags.value = removeEntry<ITag>({
				index,
				entries: _tags.value,
			});
		}

		const form = ref();
		function onSave(event: {
			submitter: HTMLInputElement;
			target: { elements: Record<string, HTMLInputElement> };
		}) {
			let eventType = '';
			switch (event.submitter.id) {
				case 'delete':
					emit('onDelete');
					return;
				case 'update':
					eventType = 'onUpdate';
					break;
				case 'create':
					eventType = 'onCreate';
					break;
			}
			const name = event.target.elements.name.value;
			const executable = event.target.elements.executable.value;
			const args = event.target.elements.arguments.value;
			const directory = event.target.elements.directory.value;
			const envVars = getEnvVarInputValues(_envVars.value, refs);
			let vallidEnvVars: TEnvVarValues = [];
			if (envVars) {
				vallidEnvVars = envVars.filter(
					({ key, value }) => Boolean(key) && Boolean(value),
				);
			}
			const tags = getTagInputValues(_tags.value, refs);
			let validTags: TTagValues = [];
			if (tags) {
				validTags = tags.filter(Boolean);
			}
			emit(eventType, {
				name,
				executable,
				arguments: args,
				directory,
				tags: validTags,
				EnvVar: vallidEnvVars,
			});
		}

		return {
			_envVars,
			_tags,
			onSave,
			addEnvVar,
			removeEnvVar,
			addTag,
			removeTag,
			isNameInvaild,
			onNameInput,
			form,
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
