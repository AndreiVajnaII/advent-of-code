import { isUpperCase } from "../utils";

export function solve(lines: string[]) {
    const maze = new Maze(lines);
    let start: Gate;
    for (const g of maze.paths.keys()) {
        if (g.label === "AA-") {
            start = g;
        }
    }
    return [
        shortestPath(maze.paths, start!, [start!]),
        shortestPathRec(maze.paths, start!, [[start!, 0]])];
}

function shortestPath(
    gates: Map<Gate, Map<Gate, number>>,
    current: Gate,
    path: Gate[] = [],
    steps = 0) {
    if (current.label === "ZZ-") {
        return steps - 1;
    } else {
        const result: number[] = [];
        gates.get(current)!.forEach((s, g) => {
            if (!path.includes(g)) {
                const pairGate = findPair(gates, g)!;
                result.push(shortestPath(gates, pairGate, [...path, g, pairGate], steps + s + 1));
            }
        });
        return result.reduce((min, x) => x < min ? x : min, Infinity);
    }
}

function shortestPathRec(
    gates: Map<Gate, Map<Gate, number>>,
    current: Gate,
    path: Array<[Gate, number]>,
    level = 0,
    steps = 0) {
    if (current.label === "ZZ-") {
        return steps - 1;
    } else if (level < 0 || level >= 30) {
        return Infinity;
    } else {
        const result: number[] = [];
        gates.get(current)!.forEach((s, gate) => {
            if (gate.label !== "AA-"
                && !path.some(([g, l]) => g === gate && l === level)
                && !(gate.label === "ZZ-" && level > 0)) {
                const pairGate = findPair(gates, gate)!;
                const nextLevel = level + gate.levelInc;
                result.push(shortestPathRec(gates, pairGate,
                    [...path, [gate, level], [pairGate, nextLevel]],
                    nextLevel, steps + s + 1));
            }
        });
        return result.reduce((min, x) => x < min ? x : min, Infinity);
    }
}

function findPair(gates: Map<Gate, Map<Gate, number>>, gate: Gate) {
    if (gate.label === "ZZ-") {
        return gate;
    }
    let label = gate.label;
    if (label[2] === "+") {
        label = label.replace("+", "-");
    } else {
        label = label.replace("-", "+");
    }
    for (const g of gates.keys()) {
        if (g.label === label) {
            return g;
        }
    }
}

class Maze {
    public paths = new Map<Gate, Map<Gate, number>>();
    private width: number;
    private height: number;

    constructor(private map: string[]) {
        this.width = map[0].length;
        this.height = map.length;
        this.findGates();
        this.paths.forEach((paths, gate) => {
            this.findPaths(gate.exit, [gate.label1, gate.label2], paths);
        });
    }

    private findGates() {
        for (let row = 2; row < this.height - 2; row++) {
            for (let col = 2; col < this.width - 2; col++) {
                if (this.map[row][col] === ".") {
                    const gate = [[[-2, 0], [-1, 0]], [[1, 0], [2, 0]], [[0, -2], [0, -1]], [[0, 1], [0, 2]]]
                        .map(([[dr1, dc1], [dr2, dc2]]) => [[row + dr1, col + dc1], [row + dr2, col + dc2]] as const)
                        .find(([[row1, col1], [row2, col2]]) =>
                            isUpperCase(this.map[row1][col1])
                            && isUpperCase(this.map[row2][col2]));
                    if (gate) {
                        const [[r1, c1], [r2, c2]] = gate;
                        let label = this.map[r1][c1] + this.map[r2][c2];
                        label += (r1 === 0 || c1 === 0 || r2 === this.height - 1 || c2 === this.width - 1)
                            ? "-" : "+";
                        this.paths.set(new Gate(label, [row, col], [r1, c1], [r2, c2]), new Map());
                    }
                }
            }
        }
    }

    private getGate(row: number, col: number) {
        for (const [g, _] of this.paths) {
            if ((g.label1[0] === row && g.label1[1] === col)
                || (g.label2[0] === row && g.label2[1] === col)) {
                return g;
            }
        }
    }

    private findPaths(
        [row, col]: [number, number],
        path: Array<[number, number]>,
        result: Map<Gate, number> = new Map<Gate, number>()) {
        if (isUpperCase(this.map[row][col])) {
            const gate = this.getGate(row, col)!;
            result.set(gate,
                Math.min(path.length - 3, result.get(gate) || Infinity));
        } else {
            this.neighbours(row, col)
                .filter(([newRow, newCol]) => !path.some(([r, c]) => r === newRow && c === newCol))
                .forEach(neighbour => this.findPaths(neighbour, [...path, [row, col]], result));
        }
        return result;
    }

    private neighbours(row: number, col: number) {
        return [[-1, 0], [1, 0], [0, -1], [0, 1]].map(([dr, dc]) => [row + dr, col + dc] as [number, number])
            .filter(([newRow, newCol]) => this.map[newRow][newCol] === "." || isUpperCase(this.map[newRow][newCol]));
    }
}

class Gate {
    public levelInc: number;

    constructor(
        public label: string,
        public exit: [number, number],
        public label1: [number, number],
        public label2: [number, number]) {
        this.levelInc = label[2] === "+" ? 1 : -1;
    }
}
