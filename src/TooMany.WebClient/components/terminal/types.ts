export namespace Terminal {
	export interface Task {
		name: string;
		stdOut: boolean;
		stdErr: boolean;
		filter?: string;
		include?: boolean;
	}

	export type Manifest = Record<string, Task[]>;
	export type Names = string[];

	export type onCreate = (terminal: Terminal.Manifest) => void;

	export type onUpdate = (
		terminal: Terminal.Manifest,
		initialName: string,
	) => void;
}
