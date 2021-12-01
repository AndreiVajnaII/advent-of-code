import { boxed, sum, union } from "../utils";

export function solve(lines: string[]) {
    return boxed(lines.map(parseLine))
        .map(constructGraph)
        .reduce(bagGraph => [
            bagGraph.findParents("shiny gold").size,
            bagGraph.countBags("shiny gold"),
        ]);
}

function parseLine(lines: string) {
    const [parentString, childrenString] = lines.split(" contain ");
    const parent = /^(.*) bags$/.exec(parentString)![1];
    const children = childrenString === "no other bags." ? []
        : childrenString.split(", ").map(childString => {
            const m = /(\d+) (.*) bag/.exec(childString)!;
            return [+m[1], m[2]] as const;
        });
    return [parent, children] as const;
}

function constructGraph(rules: BagRule[]) {
    const childrenMap = new Map<string, Array<readonly [number, string]>>();
    const parentsMap = new Map<string, string[]>();
    rules.forEach(([parent, children]) => {
        childrenMap.set(parent, children);
        if (!parentsMap.has(parent)) {
            parentsMap.set(parent, []);
        }
        children.forEach(([_, child]) => {
            if (!parentsMap.has(child)) {
                parentsMap.set(child, []);
            }
            parentsMap.get(child)!.push(parent);
        });
    });
    return new BagGraph(parentsMap, childrenMap);
}

type BagRule = readonly [string, Array<readonly [number, string]>];

class BagGraph {
    constructor(
        private parentsMap: Map<string, string[]>,
        private childrenMap: Map<string, Array<readonly [number, string]>>) { }

    public findParents(node: string): Set<string> {
        return boxed(this.parentsMap.get(node)!)
            .reduce(parents => parents.map(this.findParents.bind(this))
                .reduce(union, new Set(parents)));
    }

    public countBags(node: string): number {
        return this.childrenMap.get(node)!
            .map(([count, color]) => count + count * this.countBags(color))
            .reduce(sum, 0);
    }
}
