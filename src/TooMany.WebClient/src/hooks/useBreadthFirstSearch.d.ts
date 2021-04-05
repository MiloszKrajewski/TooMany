export type Edge = [string, string] | [string];
export type AdjacencyList = Record<string, string[]>;
export interface EdgeWithDepth {
	id: string;
	depth: number;
}
