import { StdStreams } from '../enums/cli';

export interface IMeta {
	name: string;
	executable: string;
	arguments: string;
	directory: string;
	expected_state: string;
	actual_state: string;
	environment: Record<string, string>;
	tags: string[];
	use_shell: boolean;
}

export interface ILog {
	id: string;
	channel: StdStreams;
	text: string;
	timestamp: string;
	time: number;
	task: string;
	formattedTimestamp: string;
}
