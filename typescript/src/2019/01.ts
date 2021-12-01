function calcFuel(mass: number) {
    return Math.floor(mass / 3) - 2;
}

function calcRealFuel(mass: number): number {
    const fuel = calcFuel(mass);
    return fuel > 0 ? fuel + calcRealFuel(fuel) : 0;
}

export function solve(lines: string[]) {
    return lines.map(line => +line).map(calcRealFuel).reduce((sum, fuel) => sum + fuel);
}
