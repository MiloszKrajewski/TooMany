export type EdgeId = string;
export type NodeId = string;

interface IPosition {
	top: {
		x: number;
		y: number;
	};
	bottom: {
		x: number;
		y: number;
	};
}

// IEdge includes full and orphan relationships
type IEdge = [string, string] | [string];

interface INode {
	id: NodeId;
	inputs?: NodeId[];
	outputs?: NodeId[];
	label: string;
	column: number;
}

interface ISankeyProps {
	edges: IEdge[];
	nodes: INode[];
}

interface IEdgeProps {
	id: EdgeId;
	left: IPosition;
	right: IPosition;
	isHighlighted: boolean;
	color?: string;
}

interface INodeButtonProps {
	id: NodeId;
	onClick(id: NodeId): void;
}

interface INodeProps extends INode {
	children?: React.ReactNode;
}
