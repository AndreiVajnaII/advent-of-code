import { AsciiIO, IntcodeProcessor } from "./intcode";

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    const droid = new SpringDroid(program);
    const spring1 = [
        "NOT A J",
        "NOT B T",
        "OR T J",
        "NOT C T",
        "OR T J",
        "AND D J",
        "WALK"];
    const spring2 = [
        "NOT A J",
        "NOT B T",
        "OR T J",
        "NOT C T",
        "OR T J",
        "AND D J",
        "NOT E T",
        "NOT T T",
        "OR H T",
        "AND T J",
        "RUN"];
    return [droid.run(spring1), droid.run(spring2)];
}

class SpringDroid {
    constructor(private program: number[]) { }

    public run(instructions: string[]) {
        const io = new AsciiIO();
        const droid = new IntcodeProcessor([...this.program], io);
        io.writeInstruction(instructions.join("\n"));
        droid.run();
        console.log(io.readString());
        try {
            return io.shift();
        } catch (e) {
            return;
        }
    }
}
