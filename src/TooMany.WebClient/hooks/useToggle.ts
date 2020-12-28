import { ref } from '@nuxtjs/composition-api';
import { Ref } from '~/@types';

export default function (
	initialValue = false,
): [Ref<boolean>, (value?: boolean) => void] {
	const isToggled: Ref<boolean> = ref(initialValue);

	const onToggle = (value?: boolean): void => {
		if (typeof value === 'boolean') {
			isToggled.value = value;
		} else {
			isToggled.value = !isToggled.value;
		}
	};

	return [isToggled, onToggle];
}
