import { InMemoryIO, IntcodeProcessor } from "./intcode";

type PhaseSetting = [number, number, number, number, number];

class Amplifier {

    public io: InMemoryIO;

    private proc: IntcodeProcessor;

    constructor(program: number[]) {
        this.io = new InMemoryIO();
        this.proc = new IntcodeProcessor(program, this.io);
    }

    public get halted() {
        return this.proc.halted;
    }

    public run() {
        this.proc.run();
    }
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function runAmps(program: number[], phase: PhaseSetting) {
    const amps: Amplifier[] = [];
    for (let i = 0; i < 5; i++) {
        amps.push(new Amplifier([...program]));
    }
    for (let i = 1; i < 5; i++) {
        amps[i].io.input = amps[i - 1].io.output;
        amps[i].io.input.push(phase[i]);
    }
    amps[0].io.input.push(phase[0]);
    amps[0].io.input.push(0);
    for (const amp of amps) {
        amp.run();
    }
    return amps[amps.length - 1].io.output[0];
}

function runFeedbackAmps(program: number[], phase: PhaseSetting) {
    const amps: Amplifier[] = [];
    for (let i = 0; i < 5; i++) {
        amps.push(new Amplifier([...program]));
    }
    let prevAmp = amps[amps.length - 1];
    for (let i = 0; i < amps.length; i ++) {
        amps[i].io.input = prevAmp.io.output;
        amps[i].io.input.push(phase[i]);
        prevAmp = amps[i];
    }
    amps[0].io.input.push(0);
    while (amps.some(amp => !amp.halted)) {
        for (const amp of amps) {
            amp.run();
        }
    }
    return amps[amps.length - 1].io.output[0];
}

function generatePhases(available: number[], phase: number[] = []): PhaseSetting[] {
    if (available.length === 0) {
        return [phase as PhaseSetting];
    } else {
        return available
            .map((x, i) => generatePhases(
                [...available.slice(0, i), ...available.slice(i + 1)],
                [...phase, x]))
            .reduce((acc, x) => acc.concat(x), []);
    }
}

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    return generatePhases([5, 6, 7, 8, 9])
        .map(phase => runFeedbackAmps(program, phase))
        .reduce((max, x) => x > max ? x : max);
}
