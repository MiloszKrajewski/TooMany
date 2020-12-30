export namespace Terminal {
	export interface Process {
		name: string;
		stdout: boolean;
		stderr: boolean;
		filter?: string;
		include: boolean;
	}

	export interface Manifest {
		name: string;
		processes: Process[];
	}

	export type Manifests = Manifest[];
}
