import {
	HubConnection,
	HubConnectionState,
	HubConnectionBuilder,
	LogLevel,
} from '@microsoft/signalr';

import type * as Realtime from '@tm/types/realtime';

export enum Channel {
	Log = 'Log',
	Task = 'Task',
}

class SignalR {
	private static connection: HubConnection;
	static instance = new SignalR('http://localhost:31337/monitor');

	constructor(realtimeUrl: string) {
		SignalR.connection = new HubConnectionBuilder()
			.configureLogging(LogLevel.Debug)
			.withUrl(realtimeUrl)
			.build();
	}

	async start() {
		try {
			if (SignalR.connection.state !== HubConnectionState.Disconnected) {
				throw new Error('Still connected, try again once disconnected');
			}
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
			await SignalR.connection.stop();
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
			if (id === task || id === null) fn(data);
		};
		SignalR.connection.on(Channel.Log, handler);
		return handler;
	}

	onTaskMeta(
		id: Realtime.ChannelId,
		fn: Realtime.onMetaFnCallback,
	): Realtime.onMetaFn {
		const handler: Realtime.onMetaFn = (task, data) => {
			if (id === task || id === null) fn(task, data);
		};
		SignalR.connection.on(Channel.Task, handler);
		return handler;
	}

	offTaskLog(fn: Realtime.onLogFn | null) {
		if (fn === null) return;
		console.log('done');
		SignalR.connection.off(Channel.Log, fn);
	}

	offTaskMeta(fn: Realtime.onMetaFn | null) {
		if (fn === null) return;
		SignalR.connection.off(Channel.Task, fn);
	}
}

export default SignalR.instance;
