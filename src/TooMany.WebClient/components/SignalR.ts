/* eslint-disable no-console */
import {
	HubConnection,
	HubConnectionBuilder,
	LogLevel,
} from '@microsoft/signalr';

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

	onLog(
		fn: (
			task: string,
			data: {
				channel: string;
				text: string;
				timestamp: string;
			},
		) => void,
	) {
		SignalR.connection.on('Log', fn);
	}

	offLog(fn: typeof SignalR.connection.on) {
		SignalR.connection.off('Log', fn);
	}
}

export default SignalR;
