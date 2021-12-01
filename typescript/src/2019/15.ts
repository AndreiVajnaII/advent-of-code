import { ValuesOf } from "../utils";
import { InMemoryIO, IntcodeProcessor } from "./intcode";

const directions = [1, 2, 3, 4] as const;

function nextCell(x: number, y: number, i: number) {
    return [
        x + [0, 0, -1, 1][i],
        y + [-1, 1, 0, 0][i]];
}

class Droid {
    private readonly proc: IntcodeProcessor;
    private readonly io: InMemoryIO;

    constructor(private program: number[]) {
        this.io = new InMemoryIO();
        this.proc = new IntcodeProcessor(program, this.io);
    }

    public move(direction: ValuesOf<typeof directions>) {
        this.io.input.push(direction);
        this.proc.run();
    }

    public readStatus() {
        return this.io.shift();
    }
}

class MapCell {
    constructor(
        public x: number,
        public y: number,
        public steps: number) { }
}

class MapScanner {
    private map: MapCell[] = [];
    private oxygenX = 0;
    private oxygenY = 0;
    private oxygen: MapCell[] = [];

    constructor(private droid: Droid) {
    }

    public get stepsToOxygen() {
        return this.getMapCell(this.oxygenX, this.oxygenY)?.steps;
    }

    public scan(x = 0, y = 0, step = 0) {
        this.setMapCell(x, y, step);
        for (const [i, direction] of directions.entries()) {
            const [newX, newY] = nextCell(x, y, i);
            if ((this.getMapCell(newX, newY)?.steps ?? Infinity) > step + 1) {
                this.droid.move(direction);
                const status = this.droid.readStatus();
                if (status === 1 || status === 2) {
                    if (status === 2) {
                        this.oxygenX = newX;
                        this.oxygenY = newY;
                    }
                    this.scan(newX, newY, step + 1);
                    this.droid.move(([2, 1, 4, 3] as const)[i]);
                    this.droid.readStatus();
                } else if (status !== 0) {
                    throw new Error(`Invalid status ${status}`);
                }
            }
        }
    }

    public spreadOxygen(toVisit = [new MapCell(this.oxygenX, this.oxygenY, 0)], step = 0): number {
        this.oxygen.push(...toVisit);
        const adjacents = toVisit.map(({x, y}) => this.findAdjacents(x, y))
            .reduce((r, x) => r.concat(x), []);
        return adjacents.length === 0 ? step : this.spreadOxygen(adjacents, step + 1);
    }

    private getMapCell(x: number, y: number) {
        return this.map.find(c => c.x === x && c.y === y);
    }

    private setMapCell(x: number, y: number, step: number) {
        const existing = this.getMapCell(x, y);
        if (existing) {
            existing.steps = step;
        } else {
            this.map.push(new MapCell(x, y, step));
        }
    }

    private findAdjacents(x: number, y: number) {
        return directions
            .map((d, i) => nextCell(x, y, i))
            .map(([newX, newY]) => this.getMapCell(newX, newY))
            .filter(cell => cell !== undefined)
            .map(cell => cell!)
            .filter(cell => !this.oxygen.find(c => c.x === cell.x && c.y === cell.y));
    }
}

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    const mapScanner = new MapScanner(new Droid(program));
    mapScanner.scan();
    return mapScanner.spreadOxygen();
}
