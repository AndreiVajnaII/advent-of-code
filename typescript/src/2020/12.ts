import { boxed, manhattan } from "../utils";

export function solve(lines: string[]) {
    return boxed(lines.map(parseInstruction)).reduce(instructions => [
        instructions.reduce(navigate1, new Bearing(0, 0, 0)),
        boxed(instructions.reduce(navigate2, [new Bearing(0, 0, 0), new Bearing(10, 1, 0)] as const))
            .reduce(([{ x, y }, _]) => ({ x, y })),
    ]).map(manhattan);
}

function parseInstruction(line: string): [Direction, number] {
    return [line.charAt(0) as Direction, +line.slice(1)];
}

function navigate1(bearing: Bearing, [direction, units]: [Direction, number]) {
    return instructions1[direction](bearing, units);
}

function navigate2(location: readonly [Bearing, Bearing], [direction, units]: [Direction, number]) {
    return instructions2[direction](location, units);
}

class Bearing {
    constructor(
        public x: number,
        public y: number,
        public orientation: number) { }
}

const instructions1 = {
    N: ({ x, y, orientation }: Bearing, units: number) => new Bearing(x, y + units, orientation),
    S: ({ x, y, orientation }: Bearing, units: number) => new Bearing(x, y - units, orientation),
    E: ({ x, y, orientation }: Bearing, units: number) => new Bearing(x + units, y, orientation),
    W: ({ x, y, orientation }: Bearing, units: number) => new Bearing(x - units, y, orientation),
    L: ({ x, y, orientation }: Bearing, units: number) =>
        new Bearing(x, y, (orientation + units) % 360),
    R: ({ x, y, orientation }: Bearing, units: number) =>
        new Bearing(x, y, (orientation + 360 - units) % 360),
    F: ({ x, y, orientation }: Bearing, units: number) =>
        new Bearing(
            x + ((orientation % 180) === 0 ? (90 - orientation) / 90 * units : 0),
            y + ((orientation % 180) === 0 ? 0 : (180 - orientation) / 90 * units),
            orientation),
};

const instructions2 = {
    N: moveWaypoint("N"),
    S: moveWaypoint("S"),
    E: moveWaypoint("E"),
    W: moveWaypoint("W"),
    L: ([ship, waypoint]: readonly [Bearing, Bearing], units: number) => [ship,
        new Bearing(...callTimes(units / 90, turnLeft)([waypoint.x, waypoint.y]), waypoint.orientation)] as const,
    R: ([ship, waypoint]: readonly [Bearing, Bearing], units: number) => [ship,
        new Bearing(...callTimes(units / 90, turnRight)([waypoint.x, waypoint.y]), waypoint.orientation)] as const,
    F: ([ship, waypoint]: readonly [Bearing, Bearing], units: number) =>
        [new Bearing(ship.x + units * waypoint.x, ship.y + units * waypoint.y, ship.orientation), waypoint] as const,
};

function turnLeft([x, y]: readonly [number, number]) {
    return [-y, x] as const;
}

function turnRight([x, y]: readonly [number, number]) {
    return [y, -x] as const;
}

function callTimes<T>(times: number, f: (x: T) => T): (x: T) => T {
    return times === 1 ? f : x => callTimes(times - 1, f)(f(x));
}

function moveWaypoint(orientation: "N" | "S" | "W" | "E") {
    return ([ship, waypoint]: readonly [Bearing, Bearing], units: number) =>
        [ship, instructions1[orientation](waypoint, units)] as const;
}

type Direction = keyof typeof instructions1;
