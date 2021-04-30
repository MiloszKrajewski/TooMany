import useFetcher from './useFetcher';
import type * as Task from '@tm/types/task';
import endpoints from './endpoints';

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
			start<Payload>(task: string, payload: Payload) {
				console.log('start:', task);
				return fetcher.putRequest<Task.ILog[], Payload>(
					endpoints.task.start(task),
					payload,
				);
			},
			restart<Payload>(task: string, payload: Payload) {
				console.log('restart:', task);
				return fetcher.putRequest<Task.ILog[], Payload>(
					endpoints.task.restart(task),
					payload,
				);
			},
			stop<Payload>(task: string, payload: Payload) {
				console.log('stop:', task);
				return fetcher.putRequest<void, Payload>(
					endpoints.task.stop(task),
					payload,
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
