/* eslint-disable no-console */
import { Plugin } from '@nuxt/types';
import { v4 as uuidv4 } from 'uuid';
import {
	HubConnection,
	HubConnectionBuilder,
	LogLevel,
} from '@microsoft/signalr';
import { Realtime } from '~/@types';

export enum Channel {
	Log = 'Log',
	Task = 'Task',
}

class SignalR {
	private static connection: HubConnection;

	constructor(realtimeUrl: string) {
		SignalR.connection = new HubConnectionBuilder()
			.configureLogging(LogLevel.Debug)
			.withUrl(realtimeUrl)
			.build();
	}

	async start() {
		try {
			console.log('Connecting.');
			await SignalR.connection.start();
			console.log('Connected.');
		} catch (ex) {
			console.error('Connection exception:', ex);
		}
	}

	async stop() {
		try {
			console.log('Stopping.');
			await SignalR.connection.start();
			console.log('Stopped.');
		} catch (ex) {
			console.error('Stopping exception:', ex);
		}
	}

	onClose(fn: (error?: Error) => void) {
		SignalR.connection.onclose(fn);
	}

	onTaskLog(
		id: Realtime.ChannelId,
		fn: Realtime.onLogFnCallback,
	): Realtime.onLogFn {
		const handler: Realtime.onLogFn = (task, data) => {
			if (id === task || id === null) fn({ ...data, id: uuidv4() });
		};
		SignalR.connection.on(Channel.Log, handler);
		return handler;
	}

	onTaskMeta(
		id: Realtime.ChannelId,
		fn: Realtime.onMetaFnCallback,
	): Realtime.onMetaFn {
		const handler: Realtime.onMetaFn = (task, data) => {
			if (id === task || id === null) fn(data);
		};
		SignalR.connection.on(Channel.Task, handler);
		return handler;
	}

	offTaskLog(fn: Realtime.onLogFn | null) {
		if (fn === null) return;
		SignalR.connection.off(Channel.Log, fn);
	}

	offTaskMeta(fn: Realtime.onMetaFn | null) {
		if (fn === null) return;
		SignalR.connection.off(Channel.Task, fn);
	}
}

declare module 'vue/types/vue' {
	interface Vue {
		$SignalR: SignalR;
	}
}

declare module '@nuxt/types' {
	interface NuxtAppOptions {
		$SignalR: SignalR;
	}
	interface Context {
		$SignalR: SignalR;
	}
}

const $SignalR: Plugin = ({ env: { realtimeUrl } }, inject) => {
	inject(
		'SignalR',
		(() => {
			const SR = new SignalR(realtimeUrl);
			SR.start();
			return SR;
		})(),
	);
};

export default $SignalR;
