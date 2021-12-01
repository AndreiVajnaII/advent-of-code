export function solve(lines: string[]) {
    const input = lines[0].split("").map(s => +s);
    const result1 = fft(input, 100).slice(0, 8).join("");
    const result2 = part2(input, 100).join("");
    return [result1, result2];
}

function part2(input: number[], phases: number) {
    /*  The offset is close enough to the end that it can be calculated just by adding up
        all values of the phase below, starting with the offset up to the end. */
    const offset = +input.slice(0, 7).join("");
    const count = 10000 * input.length - offset;
    const values: number[][] = new Array<number[]>(count);
    for (let i = 0; i < count; i++) {
        values[i] = [input[input.length - i % input.length - 1]];
    }
    for (let p = 1; p <= phases; p++) {
        values[0][p] = values[0][0];
    }
    for (let i = 1; i < count; i++) {
        for (let p = 1; p <= phases; p++) {
            values[i][p] = (values[i][p - 1] + values[i - 1][p]) % 10;
        }
    }
    return values.map(arr => arr[100]).slice(count - 8).reverse();
}

function fft(input: number[], phases: number) {
    while (phases > 0) {
        phases--;
        input = input.map(calcOutput);
    }
    return input;
}

function calcOutput(_: number, index: number, input: number[]) {
    let v = 0;
    for (let i = index; i < input.length;) {
        for (let j = 0; j < index + 1 && i < input.length; j++ , i++) {
            v += input[i];
        }
        i += index + 1;
        for (let j = 0; j < index + 1 && i < input.length; j++ , i++) {
            v -= input[i];
        }
        i += index + 1;
    }
    return Math.abs(v) % 10;
}
