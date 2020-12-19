import { useContext } from '@nuxtjs/composition-api';
import useFetcher from './useFetcher';
import { Task } from '~/@types';

export default function () {
	const { env } = useContext();
	const fetcher = useFetcher();

	return {
		tasks: () => fetcher.getRequest<Task.Meta>(`${env.apiV1Url}/task`),
		task: (task: string) => ({
			start: () =>
				fetcher.putRequest<Task.Log>(`${env.apiV1Url}/task/${task}/start`),
			restart: () =>
				fetcher.putRequest<Task.Log>(
					`${env.apiV1Url}/task/${task}/start?force=true`,
				),
			stop: () => fetcher.putRequest<void>(`${env.apiV1Url}/task/${task}/stop`),
			delete: () => fetcher.deleteRequest<void>(`${env.apiV1Url}/task/${task}`),
			meta: () =>
				fetcher.getRequest<Task.IMeta>(`${env.apiV1Url}/task/${task}`),
			refresh: () =>
				fetcher.getRequest<Task.Log>(`${env.apiV1Url}/task/${task}/logs`),
		}),
	};
}
