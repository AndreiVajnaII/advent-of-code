import { boxed, max, min, toNum } from "../utils";

export function solve(lines: string[]) {
    const numbers = lines.map(toNum);
    let i = 25;
    while(findPair(numbers[i], numbers.slice(i-25, i))) {
        i++;
    }
    const target = numbers[i];
    let sum = 0;
    let start = 0;
    let end = 0;
    while (sum !== target) {
        sum += numbers[end];
        while (sum > target) {
            sum -= numbers[start];
            start++;
        }
        end++;
    }
    return boxed(numbers.slice(start, end)).reduce(subArray =>
        subArray.reduce(min) + subArray.reduce(max));
}

function findPair(sum: number, numbers: number[]) {
    const set = new Set(numbers);
    for (const num of numbers) {
        if (set.has(sum - num)) {
            return true;
        }
    }
    return false;
}
