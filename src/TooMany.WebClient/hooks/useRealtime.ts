import {
	onMounted,
	onUnmounted,
	ref,
	useContext,
} from '@nuxtjs/composition-api';
import { Ref, Task, Realtime } from '~/@types';

export namespace useRealtime {
	export function TaskMeta(id: Realtime.ChannelId, Input?: Ref<Task.Meta>) {
		const { $SignalR } = useContext();

		const Output: Ref<Task.Meta> = Input || ref([]);
		const Listener: Ref<Realtime.onMetaFn | null> = ref(null);
		onMounted(() => {
			Listener.value = $SignalR.onTaskMeta(id, (data) => {
				if (data === null) {
					return Output.value;
				} else if (Output.value.length <= 0) {
					Output.value = [data];
				} else {
					Output.value = Output.value
						.map((o) => {
							if (o.name !== data.name) return o;
							return data;
						})
						.filter(Boolean);
				}
			});
		});
		onUnmounted(() => $SignalR.offTaskMeta(Listener.value));
		return Output;
	}

	export function TaskLog(id: Realtime.ChannelId, Input?: Ref<Task.Log>) {
		const { $SignalR } = useContext();

		const Output: Ref<Task.Log> = Input || ref([]);
		const Listener: Ref<Realtime.onLogFn | null> = ref(null);
		onMounted(() => {
			Listener.value = $SignalR.onTaskLog(id, (data) => {
				Output.value = [...Output.value, data];
			});
		});
		onUnmounted(() => $SignalR.offTaskLog(Listener.value));
		return Output;
	}
}
