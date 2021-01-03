import { useFetch, ref } from '@nuxtjs/composition-api';
import { useRealtime, useApi } from '~/hooks';
import { Task, Realtime } from '~/types';

export default function (id: Realtime.ChannelId) {
	const InitialMeta = ref<Task.Meta>([]);
	const api = useApi();
	useFetch(async () => {
		const result = await api.tasks();
		InitialMeta.value = result;
	});

	const RealtimeMeta = useRealtime.TaskMeta(id, InitialMeta);

	return RealtimeMeta;
}
