import { byOriginal, lcm, mapped, minOf, ofOriginal, original, product, toNum, withOriginal } from "../utils";

export function solve(lines: string[]) {
    return [ part1(lines), part2(lines) ];
}

function part1(lines: string[]) {
    const now = +lines[0];
    return lines[1].split(",").filter(s => s !== "x").map(toNum)
        .map(withOriginal(bus => bus - (now % bus)))
        .reduce(minOf(mapped)).reduce(product);
}

function part2(lines: string[]) {
    const buses = lines[1].split(",").map(withOriginal((bus, i) => i))
        .filter(byOriginal(bus => bus !== "x"))
        .map(ofOriginal(toNum));
    /*
    The problem basically is to solve the system:
    X = offset (mod busId)
    Found the exact solution:
        https://en.wikipedia.org/wiki/Modular_multiplicative_inverse#Applications
    */
    let result = BigInt(0);
    const mod = BigInt(buses.map(original).reduce(lcm));
    for (let i = 1; i < buses.length; i++) {
        const [bus, offset] = buses[i];
        const others = [...buses.slice(0, i), ...buses.slice(i + 1)];
        const prod = BigInt(others.map(original).reduce(product));
        result = (result + BigInt(bus - (offset % bus)) * prod * invMod(prod, BigInt(bus))) % mod;
    }
    return result;
}

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
