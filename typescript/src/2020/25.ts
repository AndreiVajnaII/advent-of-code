import { toNum } from "../utils";

export function solve(lines: string[]) {
    const [cardKey, doorKey] = lines.map(toNum);
    let value = 1;
    const subject = 7;
    const mod = 20201227;
    let loopSize = 0;
    while (value !== cardKey) {
        value = (value * subject) % mod;
        loopSize++;
    }
    value = 1;
    while (loopSize > 0) {
        value = (value * doorKey) % mod;
        loopSize--;
    }
    return value;
}
