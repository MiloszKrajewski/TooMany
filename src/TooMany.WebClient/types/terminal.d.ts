export interface Task {
	name: string;
	stdOut: boolean;
	stdErr: boolean;
	filter?: string;
	include?: boolean;
}

export interface Manifest {
	id: string;
	name: string;
	tasks: Task[];
}

type manifestId = string;
export type Manifests = Record<manifestId, Manifest>;
export type Names = string[];

export type onCreate = (terminal: Manifest, id?: string) => void;

export type onUpdate = (terminal: Manifest, initialId: string) => void;
