import { toNum } from "../utils";

export function solve(lines: string[]) {
    const numbers = lines[0].split(",").map(toNum);
    const lastSpoken = new Map<number, number>();
    numbers.slice(0, numbers.length - 1).forEach((num, i) => lastSpoken.set(num, i));
    const n2020 = play(numbers.length, 2020, lastSpoken, numbers[numbers.length - 1]);
    return [
        n2020,
        play(2020, 30000000, lastSpoken, n2020),
    ];
}

function play(start: number, end: number, lastSpoken: Map<number, number>, prev: number) {
    for (let i = start; i < end; i++) {
        const current = lastSpoken.has(prev) ? i - 1 - lastSpoken.get(prev)! : 0;
        lastSpoken.set(prev, i - 1);
        prev = current;
    }
    return prev;
}
