import {
	onMounted,
	onUnmounted,
	ref,
	useContext,
} from '@nuxtjs/composition-api';
import { Ref, Task, Realtime } from '~/types';

function useTaskLogErrorNotifications(
	id: Realtime.ChannelId,
	config?: {
		bufferThreshold: number;
		timestampThreshold: number;
		cooldown: number;
	},
) {
	/**
	 * TODO:
	 * Add notification click
	 * focus on the app
	 * open the erroring terminal
	 * scroll to first error
	 */
	const { $Notification } = useContext();

	const defaultConfig = {
		bufferThreshold: 10,
		timestampThreshold: 1000, // 1s
		cooldown: 10000, // 10s
	};

	const _config = {
		...defaultConfig,
		...config,
	};

	let buffer: string[] = [];

	let hasBeenErroringForLongTime = false;
	let firstErrorTimestamp: null | number = null;

	let isCoolingDown = false;

	if (!id) return () => {};

	return function (data: Task.ILog) {
		if (!isCoolingDown) {
			const isError = data.channel === 'StdErr';
			const isMessage = typeof data.text !== 'undefined';
			if (isError && isMessage) {
				buffer.push(data.text);

				if (firstErrorTimestamp === null) {
					firstErrorTimestamp = new Date(data.timestamp).getTime();
				} else {
					const currentErrorTimestamp = new Date(data.timestamp).getTime();
					const diff = currentErrorTimestamp - firstErrorTimestamp;
					hasBeenErroringForLongTime = diff >= _config.timestampThreshold;
				}
			}

			const isRemainingMessages = buffer.length > 0;
			const isTooManyMessages = buffer.length >= _config.bufferThreshold;
			if (hasBeenErroringForLongTime || isTooManyMessages) {
				$Notification.create({
					title: id,
					body: 'TooMany Errors, please check terminal.',
				});
				isCoolingDown = true;
			} else if (!isError && isRemainingMessages) {
				$Notification.create({
					title: id,
					body: buffer.join('\n'),
				});
				isCoolingDown = true;
			}

			if (isCoolingDown) {
				buffer = [];
				firstErrorTimestamp = null;
				setTimeout(() => {
					isCoolingDown = false;
				}, _config.cooldown);
			}
		}
	};
}

export namespace useRealtime {
	export function TaskMeta(id: Realtime.ChannelId, Input?: Ref<Task.Meta>) {
		const { $SignalR } = useContext();

		const Output: Ref<Task.Meta> = Input || ref([]);
		const Listener = ref<Realtime.onMetaFn | null>(null);
		onMounted(() => {
			Listener.value = $SignalR.onTaskMeta(id, (task, data) => {
				if (data === null) {
					Output.value = Output.value.filter((o) => o.name !== task);
				} else if (Output.value.length <= 0) {
					Output.value = [data];
				} else if (!Output.value.some((o) => o.name === task)) {
					Output.value = [...Output.value, data];
				} else {
					Output.value = Output.value
						.map((o) => {
							if (o.name !== task) return o;
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
		const errorNotifications = useTaskLogErrorNotifications(id);

		const Output: Ref<Task.Log> = Input || ref([]);
		const Listener = ref<Realtime.onLogFn | null>(null);
		onMounted(() => {
			Listener.value = $SignalR.onTaskLog(id, (data) => {
				errorNotifications(data);
				Output.value = [
					...Output.value,
					{ ...data, time: new Date(data.timestamp).getTime() },
				];
			});
		});
		onUnmounted(() => $SignalR.offTaskLog(Listener.value));
		return Output;
	}
	export function TaskLogs<T>(
		ids: string[] | null,
		Input?: Ref<Task.Logs>,
		filter?: Function,
		config?: T,
	) {
		if (ids === null) {
			return TaskLog(ids, Input);
		}
		const { $SignalR } = useContext();

		const Output: Ref<Task.Logs> = Input || ref([]);
		const Listener = ref<Record<string, Realtime.onLogFn>>({});
		onMounted(() => {
			for (const id of ids) {
				const errorNotifications = useTaskLogErrorNotifications(id);
				Listener.value[id] = $SignalR.onTaskLog(id, (data) => {
					const payload = {
						...data,
						task: id,
						time: new Date(data.timestamp).getTime(),
					};
					errorNotifications(payload);
					if (typeof filter === 'function') {
						const result = filter(config, id, payload);
						if (result === false) return;
					}
					Output.value = [...Output.value, payload];
				});
			}
		});
		onUnmounted(() => {
			for (const id of ids) {
				$SignalR.offTaskLog(Listener.value[id]);
			}
		});
		return Output;
	}
}
