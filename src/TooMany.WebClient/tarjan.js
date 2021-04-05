console.clear();
const edges = [
	['messageService', 'req'],
	['level1', 'level2'],
	['slushi', 'Bowie'],
	['all', 'req'],
	['level1', 'all'],
	['something2', 'messageService'],
	['something0', 'something1'],
	['something1', 'level2'],
	['barry', 'onion bargie'],
	['something1', 'messageService'],
	['something11', 'all'],
	['Bowie', 'barry'],
	['barry', 'pizza'],
	['messageService', 'all'],
	['something11', 'something1'],
	['oil', 'req'],
	['space', 'req'],
	['level1', 'space'],
	['space', 'level1991'],
	['terry', 'barry'],
	['space', 'level19'],
	['something2', 'level19'],
	['space', 'level7'],
	['messageService', 'oil'],
	['level2', 'oil'],
	['1', '2'],
	['2', '3'],
	['3', '4'],
	['barry', 'slushi'],
];

const uniqueNodes = {};
for (const [a, b] of edges) {
	uniqueNodes[a] = null;
	uniqueNodes[b] = null;
}
const nodes = Object.keys(uniqueNodes);
const numberOfNodesInGraph = nodes.length;
const UNVISITED = -1;
let adjacencyList = [];

for (let index = 0; index < numberOfNodesInGraph; index++) {
	const node = nodes[index];
	if (typeof adjacencyList[index] === 'undefined') {
		adjacencyList[index] = [];
	}
	for (const [source, target] of edges) {
		if (node !== source) continue;
		const targetIndex = nodes.findIndex((n) => n === target);
		adjacencyList[index].push(targetIndex);
	}
}

function findScc() {
	const n = nodes.length;

	let id = 0;

	const ids = new Array(n).fill(UNVISITED);
	const low = new Array(n);
	const visited = new Array(n).fill(false);
	const stack = [];

	function dfs(at) {
		ids[at] = low[at] = id++;
		stack.push(at);
		visited[at] = true;

		for (const to of adjacencyList[at]) {
			if (ids[to] === UNVISITED) {
				dfs(to);
				low[at] = Math.min(low[at], low[to]);
			}
			if (visited[to]) {
				low[at] = Math.min(low[at], ids[to]);
			}
		}

		if (ids[at] === low[at]) {
			for (let node = stack.pop(); ; node = stack.pop()) {
				visited[node] = false;
				if (at !== node) break;
			}
		}
	}

	for (let i = 0; i < n; i++) {
		if (ids[i] === UNVISITED) {
			dfs(i);
		}
	}
	const occurrences = {};
	for (const l of low) {
		occurrences[l] = (occurrences[l] || 0) + 1;
	}
	const acyclicalNodes = nodes.filter((_, index) => {
		const l = low[index];
		return occurrences[l] > 1;
	});

	console.log(acyclicalNodes);
}

findScc();
