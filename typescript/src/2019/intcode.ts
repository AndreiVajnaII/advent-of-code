type ParameterMode = 0 | 1 | 2;

class Instruction {
    public opcode: number;
    public modes: [ParameterMode, ParameterMode, ParameterMode];
    public getParam: (index: number) => number;
    public setParam: (index: number, value: number) => void;

    private resolveParamIndex: (index: number) => number;

    constructor(program: number[], position: number, relativeBase: number) {
        const code = program[position];
        this.opcode = code % 100;
        this.modes = [
            Math.floor((code % 1000) / 100) as ParameterMode,
            Math.floor((code % 10000) / 1000) as ParameterMode,
            Math.floor(code / 10000) as ParameterMode,
        ];
        this.resolveParamIndex = index => {
            switch (this.modes[index]) {
            case 0:
                return program[position + index + 1];
            case 1:
                return position + index + 1;
            case 2:
                return relativeBase + program[position + index + 1];
            default:
                throw new Error(`Invalid mode ${this.modes[index]} at index ${index}`);
            }
        };
        this.getParam = index => program[this.resolveParamIndex(index)] || 0;
        this.setParam = (index, value) => {
            program[this.resolveParamIndex(index)] = value;
        };
    }

    public executeBinary(f: (a: number, b: number) => number) {
        return f(this.getParam(0), this.getParam(1));
    }
}

export interface IO {
    read(): number | undefined;
    write(v: number): void;
}

export class IntcodeProcessor {

    public halted = false;

    private position = 0;
    private relativeBase = 0;

    constructor(private program: number[], public io: IO) { }

    public run() {
        while (this.program[this.position] !== 99) {
            const instruction = new Instruction(this.program, this.position, this.relativeBase);
            if (instruction.opcode === 1) {
                instruction.setParam(2, instruction.executeBinary((a, b) => a + b));
                this.position += 4;
            } else if (instruction.opcode === 2) {
                instruction.setParam(2, instruction.executeBinary((a, b) => a * b));
                this.position += 4;
            } else if (instruction.opcode === 3) {
                const v = this.io.read();
                if (v === undefined) {
                    return;
                }
                instruction.setParam(0, v);
                this.position += 2;
            } else if (instruction.opcode === 4) {
                this.io.write(instruction.getParam(0));
                this.position += 2;
            } else if (instruction.opcode === 5) {
                this.position = instruction.executeBinary((a, b) => a !== 0 ? b : this.position + 3);
            } else if (instruction.opcode === 6) {
                this.position = instruction.executeBinary((a, b) => a === 0 ? b : this.position + 3);
            } else if (instruction.opcode === 7) {
                instruction.setParam(2, instruction.executeBinary((a, b) => a < b ? 1 : 0));
                this.position += 4;
            } else if (instruction.opcode === 8) {
                instruction.setParam(2, instruction.executeBinary((a, b) => a === b ? 1 : 0));
                this.position += 4;
            } else if (instruction.opcode === 9) {
                this.relativeBase += instruction.getParam(0);
                this.position += 2;
            } else {
                throw new Error(`Wrong opcode at ${this.position}: ${this.program[this.position]}`);
            }
        }
        this.halted = true;
    }

}

export class InMemoryIO implements IO {
    public input: number[] = [];
    public output: number[] = [];

    public read() {
        return this.input.shift();
    }

    public write(v: number) {
        this.output.push(v);
    }

    public shift() {
        const v = this.output.shift();
        if (v === undefined) {
            throw new Error("No output!");
        } else {
            return v;
        }
    }

}

export class AsciiIO extends InMemoryIO {

    public writeInstruction(instruction: string) {
        for (let i = 0; i < instruction.length; i++) {
            this.input.push(instruction.charCodeAt(i));
        }
        this.input.push(10); // newline
    }

    public readString() {
        let s = "";
        while (this.output.length > 0 && this.output[0] < 128) {
            s += String.fromCharCode(this.output.shift()!);
        }
        return s;
    }
}
