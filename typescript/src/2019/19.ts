import { InMemoryIO, IntcodeProcessor } from "./intcode";

class Beam {
    constructor(private program: number[]) { }

    public shoot(x: number, y: number) {
        const io = new InMemoryIO();
        const beam = new IntcodeProcessor([...this.program], io);
        io.input.push(x);
        io.input.push(y);
        beam.run();
        return io.shift() === 1;
    }
}

export function solve(lines: string[]) {
    const beam = new Beam(lines[0].split(",").map(s => +s));
    return [coverage(beam, 50, 50), findRect(beam, 100, 100)];
}

function coverage(beam: Beam, width: number, height: number) {
    let count = 0;
    for (let x = 0; x < width; x++) {
        for (let y = 0; y < height; y++) {
            if (beam.shoot(x, y)) {
                count++;
            }
        }
    }
    return count;
}

function findRect(beam: Beam, width: number, height: number) {
    let y = height; // start from a distance, because some rows are not affected at all
    let prevX = 0;
    while (true) {
        let x = prevX;
        while (!beam.shoot(x, y)) {
            x++;
        }
        prevX = x;
        if (beam.shoot(x + width - 1, y - height + 1) && !beam.shoot(x + width, y - height + 1)) {
            return x * 10000 + (y - height + 1);
        }
        y++;
    }
}
