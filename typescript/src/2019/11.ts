import { InMemoryIO, IntcodeProcessor } from "./intcode";

type Direction = 0 | 90 | 180 | 270;
class Panel {
    constructor(
        public x: number,
        public y: number,
        public color: number) {}
}

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    const io = new InMemoryIO();
    const robot = new IntcodeProcessor(program, io);
    let [x, y] = [0, 0];
    let direction: Direction = 0;
    const painted: Panel[] = [];
    painted.push(new Panel(0, 0, 1));
    io.input.push(1);
    while (!robot.halted) {
        robot.run();
        const color = io.output.shift();
        if (color === undefined) {
            throw new Error("Oops, no output for color!");
        }
        const panel = painted.find(panelAt(x, y));
        if (panel) {
            panel.color = color;
        } else {
            painted.push(new Panel(x, y, color));
        }
        const turnCommand = io.output.shift();
        if (turnCommand !== undefined) {
            direction = turn(direction, turnCommand as 0 | 1);
        } else {
            throw new Error("Oops, no output for turn!");
        }
        [x, y] = advance(x, y, direction);
        io.input.push(painted.find(panelAt(x, y))?.color || 0);
    }
    const minX = painted.map(p => p.x).reduce((min, px) => px < min ? px : min);
    const minY = painted.map(p => p.y).reduce((min, py) => py < min ? py : min);
    const maxX = painted.map(p => p.x).reduce((max, px) => px > max ? px : max);
    const maxY = painted.map(p => p.y).reduce((max, py) => py > max ? py : max);
    const w = maxX - minX;
    const h = maxY - minY;
    painted.forEach(p => {
        p.x -= minX;
        p.y -= minY;
    });
    for (let row = 0; row <= h; row++) {
        let line = "";
        for (let col = 0; col <= w; col++) {
            const p = painted.find(panelAt(col, row));
            if (p && p.color === 1) {
                line += "#";
            } else {
                line += " ";
            }
        }
        console.log(line);
    }
}

function panelAt(x: number, y: number) {
    return (p: Panel) => p.x === x && p.y === y;
}

function turn(direction: Direction, command: 0 | 1): Direction {
    if (command === 0) {
        return (direction + 270) % 360 as Direction;
    } else {
        return (direction + 90) % 360 as Direction;
    }
}

function advance(x: number, y: number, direction: Direction): [number, number] {
    switch (direction) {
    case 0:
        return [x, y - 1];
    case 90:
        return [x + 1, y];
    case 180:
        return [x, y + 1];
    case 270:
        return [x - 1, y];
    }
}
