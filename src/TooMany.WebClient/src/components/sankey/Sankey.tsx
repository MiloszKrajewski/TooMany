import type { INode, ISankeyProps } from './types';
import Edge from './edge/Edge';
import Node from './node/Node';
import useBreadthFirstSearch from '../../hooks/useBreadthFirstSearch';

export const OldComp = (nodes: INode[]) => {
	const orphans = [];
	const columns: INode[][] = [];
	for (const node of nodes) {
		if (!node.column) {
			orphans.push(node);
			continue;
		}
		if (!columns[node.column]) columns[node.column] = [];
		columns[node.column].push(node);
	}
	return (
		<div>
			<div id="orphans">
				{orphans.map((o) => (
					<div key={o.id}>{o.label}</div>
				))}
			</div>
			<div id="columns">
				{columns.map((nodes, i) => (
					<>
						<div key={i}>
							{nodes.map((node) => (
								<Node {...node} />
							))}
						</div>
						{i !== columns.length - 1 && (
							<div>
								<svg>
									<Edge
										id="basic"
										left={{ top: { x: 10, y: 10 }, bottom: { x: 0, y: 40 } }}
										right={{
											top: { x: 100, y: 100 },
											bottom: { x: 0, y: -40 },
										}}
										isHighlighted={false}
									/>
								</svg>
							</div>
						)}
					</>
				))}
			</div>
		</div>
	);
};

export default ({ edges = [] }: ISankeyProps) => {
	const Nodes = useBreadthFirstSearch(edges);
	return <pre>{JSON.stringify(Nodes, null, 2)}</pre>;
};
