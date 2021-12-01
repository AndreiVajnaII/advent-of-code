
export function solve(lines: string[]) {
    const cpu = new Cpu(lines.map(parseInstruction));
    while (!cpu.isComplete) {
        const opType = cpu.currentInstruction[0];
        if (opType !== "acc") {
            cpu.currentInstruction[0] = ({ nop: "jmp", jmp: "nop" } as const)[opType];
            const snapshot = cpu.snapshot;
            cpu.run();
            if (!cpu.isComplete) {
                cpu.snapshot = snapshot;
                cpu.currentInstruction[0] = opType;
            } else {
                return cpu.state.acc;
            }
        }
        cpu.execute();
    }
}

function parseInstruction(line: string) {
    const [operation, arg] = line.split(" ");
    return [operation, +arg] as Instruction;
}

const operations = {
    nop: ({ index, acc }: CpuState) => new CpuState(index + 1, acc),
    jmp: ({ index, acc }: CpuState, arg: number) => new CpuState(index + arg, acc),
    acc: ({ index, acc }: CpuState, arg: number) => new CpuState(index + 1, acc + arg),
};

function executeInstrcution([operationType, arg]: Instruction, state: CpuState) {
    return operations[operationType](state, arg);
}

type OperationType = keyof typeof operations;
type Instruction = [OperationType, number];

class CpuState {
    constructor(public readonly index: number, public readonly acc: number) { }
}

class Cpu {
    public state: CpuState = { index: 0, acc: 0 };
    private executed: boolean[];

    constructor(private program: Instruction[]) {
        this.executed = new Array<boolean>(program.length);
    }

    public execute() {
        this.executed[this.state.index] = true;
        this.state = executeInstrcution(this.currentInstruction, this.state);
    }

    public run() {
        while (!this.isComplete && !this.executed[this.state.index]) {
            this.execute();
        }
    }

    public get currentInstruction() {
        return this.program[this.state.index];
    }

    public get isComplete() {
        return this.state.index >= this.program.length;
    }

    public get snapshot() {
        return [this.state, [...this.executed]] as const;
    }

    public set snapshot([state, executed]: readonly [CpuState, readonly boolean[]]) {
        this.state = state;
        this.executed = [...executed];
    }

}
