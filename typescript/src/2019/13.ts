import { InMemoryIO, IntcodeProcessor } from "./intcode";

export async function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    program[0] = 2;
    const io = new InMemoryIO();
    const arcade = new IntcodeProcessor(program, io);
    const screen: string[][] = [];
    let ballX = 0;
    let paddleX = 0;
    let score = 0;
    while (!arcade.halted) {
        console.log("\x1B[2J");
        arcade.run();
        while (io.output.length > 0) {
            const x = io.shift();
            const y = io.shift();
            const tile = io.shift();
            if (x === -1 && y === 0) {
                score = tile;
                console.log(`Score: ${score}`);
            } else {
                if (!screen[y]) {
                    screen[y] = [];
                }
                screen[y][x] = symbol(tile);
                if (tile === 4) {
                    ballX = x;
                }
                if (tile === 3) {
                    paddleX = x;
                }
            }
        }
        for (const row of screen) {
            console.log((row || []).join(""));
        }
        io.input.push(Math.sign(ballX - paddleX));
        await sleep(50);
    }
    return score;
}

function symbol(tile: number) {
    return " #+-*"[tile];
}

function sleep(ms: number) {
    return new Promise(resolve => {
        setTimeout(resolve, ms);
    });
}
