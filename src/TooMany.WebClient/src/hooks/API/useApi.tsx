import type * as Task from 'types/task';

import endpoints from './endpoints';
import useFetcher from './useFetcher';

export default function () {
	const fetcher = useFetcher();

	return {
		server: {
			version() {
				return fetcher.getRequest<string>(`${env.apiV1Url}/version`);
			},
		},
		app: {
			version() {
				return fetcher.getRequest<string>(`${env.apiV1Url}/version`);
			},
		},
		task: {
			list() {
				console.log('fetch all');
				return fetcher.getRequest<Task.IMeta[]>(endpoints.task.list());
			},
			create<Payload>(task: string, payload: Payload) {
				console.log('create:', task);
				return fetcher.postRequest<Task.IMeta, Payload>(
					endpoints.task.create(task),
					payload,
				);
			},
			start(task: string) {
				console.log('start:', task);
				return fetcher.putRequest<Task.IMeta, undefined>(
					endpoints.task.start(task),
				);
			},
			restart(task: string) {
				console.log('restart:', task);
				return fetcher.putRequest<Task.IMeta, undefined>(
					endpoints.task.restart(task),
				);
			},
			stop(task: string) {
				console.log('stop:', task);
				return fetcher.putRequest<Task.IMeta, undefined>(
					endpoints.task.stop(task),
				);
			},
			delete(task: string) {
				console.log('delete:', task);
				return fetcher.deleteRequest<void>(endpoints.task.delete(task));
			},
			meta(task: string) {
				console.log('meta:', task);
				return fetcher.getRequest<Task.IMeta>(endpoints.task.meta(task));
			},
			logs(task: string) {
				console.log('logs:', task);
				return fetcher.getRequest<Task.ILog[]>(endpoints.task.logs(task));
			},
		},
	};
}
