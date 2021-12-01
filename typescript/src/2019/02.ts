function run(program: number[]) {
    let position = 0;
    while (program[position] !== 99) {
        if (program[position] === 1) {
            program[program[position + 3]] = program[program[position + 1]] + program[program[position + 2]];
        } else if (program[position] === 2) {
            program[program[position + 3]] = program[program[position + 1]] * program[program[position + 2]];
        } else {
            throw new Error(`Wrong opcode at ${position}: ${program[position]}`);
        }
        position += 4;
    }
    return program;
}

function findInputs(program: number[], target: number): [number, number] {
    for (let noun = 0; noun <= 99; noun++) {
        for (let verb = 0; verb <= 99; verb++) {
            program[1] = noun;
            program[2] = verb;
            const result = run([...program]);
            if (result[0] === target) {
                return [noun, verb];
            }
        }
    }
    throw new Error("No inputs found!");
}

export function solve(lines: string[]) {
    const program = lines[0].split(",").map(s => +s);
    const [noun, verb] = findInputs(program, 19690720);
    return 100 * noun + verb;
}
