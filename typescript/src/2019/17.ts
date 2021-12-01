import { InMemoryIO, IntcodeProcessor } from "./intcode";

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    const io = new InMemoryIO();
    const proc = new IntcodeProcessor([...program], io);
    proc.run();
    let row = 0;
    const map: string[] = [];
    map[row] = "";
    while (io.output.length > 0) {
        const c = String.fromCharCode(io.output.shift()!);
        if (c === "\n") {
            console.log(map[row]);
            row++;
            map[row] = "";
        } else {
            map[row] += c;
        }
    }
    const intersections: number[] = [];
    for (let y = 1; y < map.length - 1; y++) {
        for (let x = 1; x < map[0].length - 1; x++) {
            if (isIntersection(x, y, map)) {
                intersections.push(x * y);
            }
        }
    }

    const movements = computeMovement(map);
    console.log(movements.join(","));
    const A = "L,4,L,4,L,6,R,10,L,6";
    const B = "L,12,L,6,R,10,L,6";
    const C = "R,8,R,10,L,6";
    const routine = "A,A,B,C,C,A,C,B,C,B";

    return runRobot([...program], A, B, C, routine);
}

function isIntersection(x: number, y: number, map: string[]) {
    return map[y][x] === "#"
        && map[y - 1][x] === "#"
        && map[y][x - 1] === "#"
        && map[y + 1][x] === "#"
        && map[y][x + 1] === "#";
}

let robotX = 0;
let robotY = 0;

function computeMovement(map: string[]) {
    let row = 0;
    let col = 0;
    do {
        col = map[row].indexOf("^");
        row++;
    } while (col === -1);
    robotX = col;
    robotY = row - 1;

    const movements: string[] = [];
    while (turn(movements, map)) {
        moveForward(movements, map);
    }

    return movements;
}

type Turn = "<" | ">" | "^" | "v";

const turns = {
    "<": [
        [0, +1, "L", "v"] as [number, number, string, Turn],
        [0, -1, "R", "^"] as [number, number, string, Turn],
    ],
    ">": [
        [0, -1, "L", "^"] as [number, number, string, Turn],
        [0, +1, "R", "v"] as [number, number, string, Turn],
    ],
    "^": [
        [-1, 0, "L", "<"] as [number, number, string, Turn],
        [+1, 0, "R", ">"] as [number, number, string, Turn],
    ],
    "v": [
        [+1, 0, "L", ">"] as [number, number, string, Turn],
        [-1, 0, "R", "<"] as [number, number, string, Turn],
    ],
};

const forward = {
    "<": [-1, 0],
    ">": [+1, 0],
    "^": [0, -1],
    "v": [0, +1],
};

let position: Turn = "^";

function turn(movements: string[], map: string[]) {
    const next = turns[position].find(([dx, dy, _, _p]) => isScaffolding(map, robotX + dx, robotY + dy));
    if (next) {
        movements.push(next[2]);
        position = next[3];
        return true;
    } else {
        return false;
    }
}

function moveForward(movements: string[], map: string[]) {
    let steps = 0;
    const [dx, dy] = forward[position];
    while(isScaffolding(map, robotX + dx, robotY + dy)) {
        steps++;
        robotX += dx;
        robotY += dy;
    }
    movements.push(steps.toString());
}

function isScaffolding(map: string [], x: number, y: number) {
    return x >= 0 && x < map[0].length && y >= 0 && y < map.length
        && map[y][x] === "#";
}

function runRobot(program: number[], A: string, B: string, C: string, routine: string) {
    program[0] = 2;
    const io = new InMemoryIO();
    writeInstruction(io.input, routine);
    writeInstruction(io.input, A);
    writeInstruction(io.input, B);
    writeInstruction(io.input, C);
    writeInstruction(io.input, "y");

    const robot = new IntcodeProcessor(program, io);
    robot.run();
    return io.output[io.output.length - 1];
}

function writeInstruction(arr: number[], instruction: string) {
    for (let i = 0; i < instruction.length; i++) {
        arr.push(instruction.charCodeAt(i));
    }
    arr.push(10); // newline
}
