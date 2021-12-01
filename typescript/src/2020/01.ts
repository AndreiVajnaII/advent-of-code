export function solve(lines: string[]) {
    const numbers = lines.map(s => +s);
    const set = new Set(numbers);
    return findTriplet(numbers, set);
}

function findPair(numbers: number[], sum: number, set: Set<number>) {
    for (const num of numbers) {
        const p = sum - num;
        if (set.has(p)) {
            return p * num;
        } else {
            set.add(num);
        }
    }
}

function findTriplet(numbers: number[], set: Set<number>) {
    for (let i = 0; i < numbers.length; i++) {
        const pairProduct = findPair(removeFromArray(i, numbers), 2020 - numbers[i], set);
        if (pairProduct) {
            return numbers[i] * pairProduct;
        }
    }
}

function removeFromArray(index: number, numbers: number[]) {
    return numbers.slice(0, index).concat(numbers.slice(index + 1));
}
