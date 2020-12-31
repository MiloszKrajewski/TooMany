export namespace Terminal {
	export interface Task {
		name: string;
		stdOut: boolean;
		stdErr: boolean;
		filter?: string;
		include: boolean;
	}

	export type DBManifest = Record<string, Task[]>;

	export interface Manifest {
		name: string;
		tasks: Task[];
	}

	export type Manifests = Manifest[];
}
