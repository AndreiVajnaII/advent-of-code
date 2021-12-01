import { InMemoryIO, IntcodeProcessor } from "./intcode";

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    const io = new InMemoryIO();
    io.input.push(2);
    new IntcodeProcessor(program, io).run();
    return io.output;
}
