import { ascending, sum, toNum } from "../utils";

export function solve(lines: string[]) {
    const joltages = [0, ...lines.map(toNum).sort(ascending)];

    return [
        calculateDifferences(joltages),
        calculateCombinations(joltages),
    ];
}

function calculateDifferences(joltages: number[]) {
    let diff1 = 0;
    let diff3 = 1;
    let prev = 0;
    for (const joltage of joltages) {
        if (joltage - prev === 1) {
            diff1++;
        } else if (joltage - prev === 3) {
            diff3++;
        }
        prev = joltage;
    }
    return diff1 * diff3;
}

function calculateCombinations(joltages: number[]) {
    const combinations = new Array<number>(joltages.length);
    combinations[combinations.length - 1] = 1;
    for (let i = joltages.length - 2; i >= 0; i--) {
        combinations[i] = [1, 2, 3].map(j => i + j)
            .filter(j => joltages[j] && joltages[j] - joltages[i] <= 3)
            .map(j => combinations[j]).reduce(sum);
    }
    return combinations[0];
}
