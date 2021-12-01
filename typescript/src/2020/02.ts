export function solve(lines: string[]) {
    return [isValid1, isValid2]
        .map(isValid => lines.filter(isValid).length);
}

function isValid1(line: string) {
    const [min, max, letter, password] = parseLine(line);
    const count = password.split("").filter(c => c === letter).length;
    return min <= count && count <= max;
}

function isValid2(line: string) {
    const [first, second, letter, password] = parseLine(line);
    const isFirst = password.charAt(first - 1) === letter;
    const isSecond = password.charAt(second - 1) === letter;
    return (isFirst && !isSecond) || (!isFirst && isSecond);
}

function parseLine(line: string) {
    const [policy, password] = line.split(": ");
    const [range, letter] = policy.split(" ");
    const [first, second] = range.split("-").map(s => +s);
    return [first, second, letter, password] as const;
}
