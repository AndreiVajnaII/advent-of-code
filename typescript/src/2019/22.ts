export function solve(lines: string[]) {
    let stack: number[] = [];
    for (let i = 0; i < 10007; i++) {
        stack.push(i);
    }
    for (const line of lines) {
        stack = shuffle(line,
            () => dealNewStack(stack),
            step => stepwiseNewStack(stack, step),
            n => cutNewStack(stack, n));
    }
    const part1 = stack.indexOf(2019);

    const func = shuffleFunc(BigInt(119315717514047), BigInt(101741582076661), lines);
    const part2 = (func[0] * BigInt(2020) + func[1]) % BigInt(119315717514047);
    return [part1, part2];
}

function shuffleFunc(length: bigint, iterations: bigint, instructions: string[]): [bigint, bigint] {
    let func: [bigint, bigint] = [BigInt(1), BigInt(0)];
    for (let i = instructions.length - 1; i >= 0; i--) {
        func = shuffle(instructions[i],
            () => dealFunc(func, length),
            step => stepFunc(func, BigInt(step), length),
            n => cutFunc(func, BigInt(n), length));
    }
    iterations--;
    const powers = [BigInt(1)];
    const powerFuncs = [func];
    for (let p = BigInt(2); p <= iterations; p *= BigInt(2)) {
        powers.push(p);
        powerFuncs.push(composeFunc(powerFuncs[powerFuncs.length - 1],
            powerFuncs[powerFuncs.length - 1], length));
    }
    while (iterations > 0) {
        let i = 0;
        while (i < powers.length - 1 && powers[i + 1] <= iterations) {
            i++;
        }
        func = composeFunc(powerFuncs[i], func, length);
        iterations -= powers[i];
    }
    return func;
}

function composeFunc([a1, b1]: [bigint, bigint], [a2, b2]: [bigint, bigint], m: bigint): [bigint, bigint] {
    return [(a1 * a2) % m, (a1 * b2 + b1) % m];
}

function dealFunc([a, b]: [bigint, bigint], length: bigint): [bigint, bigint] {
    return [-a, length - BigInt(1) - b];
}

function stepFunc([a, b]: [bigint, bigint], n: bigint, length: bigint): [bigint, bigint] {
    const inv = invMod(n, length);
    return [(a * inv) % length, (b * inv) % length];
}

function cutFunc([a, b]: [bigint, bigint], n: bigint, length: bigint): [bigint, bigint] {
    return [a, (b + length + n) % length];
}

// Extended Euclidean algorithm to calculate inverse modulo
function invMod(x: bigint, m: bigint) {
    let ta = BigInt(0);
    let tb = BigInt(1);
    let a = m;
    let b = x;
    while (b !== BigInt(0)) {
        const q = a / b;
        [ta, tb] = [tb, ta - q * tb];
        [a, b] = [b, a - q * b];
    }
    return ta < 0 ? ta + m : ta;
}

function shuffle<T>(
    instruction: string,
    deal: () => T,
    stepwise: (step: number) => T,
    cut: (n: number) => T) {
    if (instruction === "deal into new stack") {
        return deal();
    }
    let m = /deal with increment (\d+)/.exec(instruction);
    if (m) {
        return stepwise(+m[1]);
    }
    m = /cut (-?\d+)/.exec(instruction);
    if (m) {
        return cut(+m[1]);
    }
    throw new Error("Unknown instruction: " + instruction);
}

function dealNewStack(stack: number[]) {
    const newStack: number[] = [];
    for (const c of stack) {
        newStack.unshift(c);
    }
    return newStack;
}

function stepwiseNewStack(stack: number[], step: number) {
    const newStack: number[] = new Array<number>(stack.length);
    let i = 0;
    while (stack.length > 0) {
        newStack[i] = stack.shift()!;
        i = (i + step) % newStack.length;
    }
    return newStack;
}

function cutNewStack(stack: number[], n: number) {
    if (n > 0) {
        const removed = stack.splice(0, n);
        return [...stack, ...removed];
    } else {
        const removed = stack.splice(stack.length - Math.abs(n), Math.abs(n));
        return [...removed, ...stack];
    }
}
