type Graph = Map<string, string[]>;

function createOrbitGraph(lines: string[]) {
    const graph = new Map<string, string[]>();
    for (const line of lines) {
        const objects = line.split(")");
        graph.set(objects[0], [...(graph.get(objects[0]) || []), objects[1]]);
    }
    return graph;
}

function countOrbits(g: Graph, node: string, d: number): number {
    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    return d + (g.get(node) || []).map(orbit => countOrbits(g, orbit, d + 1))
        .reduce((r, x) => r + x, 0);
}

function findParent(g: Graph, node: string) {
    for (const [k, v] of g.entries()) {
        if (v.includes(node)) {
            return k;
        }
    }
}

const visited = new Set<string>();

function countSteps(g: Graph, source: string, target: string): number | undefined {
    visited.add(source);
    if (source === target) {
        return visited.size - 2;
    } else {
        const neighbours = [...(g.get(source) || [])];
        const parent = findParent(g, source);
        if (parent) {
            neighbours.push(parent);
        }
        const result = neighbours.filter(n => !visited.has(n)).map(n => () => countSteps(g, n, target))
            .reduce<number | undefined>((finalResult, f) => finalResult || f(), undefined);
        visited.delete(source);
        return result;
    }
}

export function solve(lines: string[]) {
    const g = createOrbitGraph(lines);
    return countSteps(g, "YOU", findParent(g, "SAN")!);
}
