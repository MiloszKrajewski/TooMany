<template>
	<form @submit.prevent="onSave">
		<dl>
			<label for="name">
				<dt>name</dt>
				<dd>
					<input name="name" type="text" :value="name" />
				</dd>
			</label>
			<label for="executable">
				<dt>executable</dt>
				<dd>
					<input name="executable" type="text" :value="executable" />
				</dd>
			</label>
			<label for="arguments">
				<dt>arguments</dt>
				<dd>
					<input name="arguments" type="text" :value="args" />
				</dd>
			</label>
			<label for="directory">
				<dt>directory</dt>
				<dd>
					<input name="directory" type="text" :value="directory" />
				</dd>
			</label>
			<dl>
				<dt>environment key/value</dt>
				<fieldset ref="envVars">
					<dd v-for="(envVar, index) in _envVars" :key="index">
						<input
							:name="`${index}.key`"
							type="text"
							:value="envVar.key"
							placeholder="key"
						/>
						<input
							:name="`${index}.value`"
							type="text"
							:value="envVar.value"
							placeholder="value"
						/>
						<button @click="() => removeEnvVar(index)">-</button>
						<button @click="() => addEnvVar(index)">+</button>
					</dd>
					<button v-if="_envVars.length <= 0" @click="() => addEnvVar()">
						+
					</button>
				</fieldset>
			</dl>
			<dl>
				<dt>tags</dt>
				<fieldset ref="tags">
					<dd v-for="(tag, index) in _tags" :key="tag.id">
						<input :name="`${index}.value`" type="text" :value="tag" />
						<button @click="() => removeTag(index)">-</button>
						<button @click="() => addTag(index)">+</button>
					</dd>
					<button v-if="_tags.length <= 0" @click="addTag">+</button>
				</fieldset>
			</dl>
		</dl>
		<input id="save" type="submit" value="Update" />
	</form>
</template>

<script lang="ts">
import { defineComponent, ref } from '@nuxtjs/composition-api';
import { Ref } from '~/@types';

interface envVar {
	key: string;
	value: string;
}
type envVars = envVar[];
type refEnvVars = Ref<envVars>;

export default defineComponent({
	props: {
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
			type: Object,
			default: () => ({}),
		},
		tags: {
			type: Array,
			default: () => [],
		},
	},
	setup(props) {
		const _tags = ref(props.tags);

		const _envVars: refEnvVars = ref(
			Object.entries(props.envVars).map(([key, value]) => ({
				key,
				value,
			})),
		);

		function cloneEnvVarValues(
			elements: Record<string, HTMLInputElement>,
			index: number,
		) {
			return {
				key: elements[`${index}.key`].value,
				value: elements[`${index}.value`].value,
			};
		}

		function addEnvVar(index?: number) {
			const newEntry = { key: '', value: '' };
			if (typeof index === 'number') {
				const elements = this.$refs.envVars.elements;
				const out = [];
				for (let i = 0; i < _envVars.value.length; i++) {
					const env = _envVars.value[i];
					out.push({
						...env,
						...cloneEnvVarValues(elements, i),
					});
					if (i === index) {
						out.push(newEntry);
					}
				}
				_envVars.value = out;
			} else {
				_envVars.value = [newEntry];
			}
		}

		function removeEnvVar(index: number) {
			const elements = this.$refs.envVars.elements;
			_envVars.value = _envVars.value
				.filter((_, i) => i !== index)
				.map((env, i) => ({
					...env,
					...cloneEnvVarValues(elements, i),
				}));
		}

		function cloneTagValue(
			elements: Record<string, HTMLInputElement>,
			index: number,
		) {
			return elements[`${index}.value`].value;
		}

		function addTag(index: number) {
			if (typeof index === 'number') {
				const elements = this.$refs.tags.elements;
				const out = [];
				for (let i = 0; i < _tags.value.length; i++) {
					out.push(cloneTagValue(elements, i));
					if (i === index) {
						out.push('');
					}
				}
				_tags.value = out;
			} else {
				_tags.value = [''];
			}
		}
		function removeTag(index: number) {
			const elements = this.$refs.tags.elements;
			_tags.value = _tags.value
				.filter((_, i) => i !== index)
				.map((_, i) => cloneTagValue(elements, i));
		}

		// eslint-disable-next-line @typescript-eslint/no-unused-vars
		function onSave(event: { target: { elements: Record<string, Element> } }) {
			const output = {};
			const envVarsElements = this.$refs.envVars.elements;
			output.envVars = _envVars.value.map((_, i) => {
				const { key, value } = cloneEnvVarValues(envVarsElements, i);
				return { [key]: value };
			});
			const tagElements = this.$refs.tags.elements;
			output.tags = _tags.value.map((_, i) => cloneTagValue(tagElements, i));
			console.log(output);
		}
		return {
			_envVars,
			_tags,
			onSave,
			addEnvVar,
			removeEnvVar,
			addTag,
			removeTag,
		};
	},
});
</script>
