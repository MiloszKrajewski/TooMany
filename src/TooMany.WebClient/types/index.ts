export enum StdSteams {
	Stdout = 'StdOut',
	StdErr = 'StdErr',
}

export interface Ref<T> {
	value: T;
}

export namespace Task {
	export interface IMeta {
		name: string;
		executable: string;
		arguments: string;
		directory: string;
		expected_state: string;
		actual_state: string;
		environment: Record<string, string>;
		tags: string[];
	}

	export type Meta = IMeta[];

	export interface ILog {
		id: string;
		channel: StdSteams;
		text: string;
		timestamp: string;
		time: number;
	}

	export type Log = ILog[];

	export interface ILogs extends ILog {
		task: string;
	}
	export type Logs = ILogs[];
}

export namespace Realtime {
	export type onLogFn = (task: string, data: Task.ILog) => void;
	export type onLogFnCallback = (data: Task.ILog) => void;

	export type onMetaFn = (task: string, data: Task.IMeta) => void;
	export type onMetaFnCallback = (task: string, data: Task.IMeta) => void;

	export type ChannelId = string | null;
}
