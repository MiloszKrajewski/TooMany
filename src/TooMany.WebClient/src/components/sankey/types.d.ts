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
type IValidEdge = [string, string];
type ISoloEdge = [string];
export type IEdge = IValidEdge | ISoloEdge;

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
	className?: string;
	id: EdgeId;
	left: IPosition;
	right: IPosition;
	isHighlighted?: boolean;
	color?: string;
	onMouseEnter?: (id: EdgeId) => void;
	onMouseLeave?: (id: EdgeId) => void;
}

interface INodeButtonProps {
	id: NodeId;
	onClick(id: NodeId): void;
}

interface INodeProps {
	className?: string;
	id: NodeId;
	inputs?: NodeId[];
	outputs?: NodeId[];
	children?: React.ReactNode;
	label?: string;
}
