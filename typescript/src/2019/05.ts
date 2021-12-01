import { IntcodeProcessor } from "./intcode";

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    const input = [5];
    let result: number | undefined;
    const read = () => input.pop();
    const write = (v: number) => { result = v; };
    new IntcodeProcessor(program, { read, write }).run();
    return result;
}
