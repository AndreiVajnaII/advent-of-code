class Point {
    constructor(
        public x: number,
        public y: number) { }
}

class PointWithSteps extends Point {
    constructor(
        x: number, y: number,
        public steps: number) {
        super(x, y);
    }
}

class Line {
    public p1: PointWithSteps;
    public p2: PointWithSteps;

    constructor(p1: PointWithSteps, p2: PointWithSteps, public direction: string) {
        if (p1.x < p2.x || p1.y < p2.y) {
            this.p1 = p1;
            this.p2 = p2;
        } else {
            this.p1 = p2;
            this.p2 = p1;
        }
    }
}

function calcWireLines(wire: string[]) {
    let pos = new PointWithSteps(0, 0, 0);
    let steps = 0;
    return wire.map(v => {
        const delta = +v.substr(1);
        const oldPos = pos;
        steps += delta;
        switch (v[0]) {
        case "R":
            pos = new PointWithSteps(pos.x + delta, pos.y, steps);
            break;
        case "L":
            pos = new PointWithSteps(pos.x - delta, pos.y, steps);
            break;
        case "U":
            pos = new PointWithSteps(pos.x, pos.y + delta, steps);
            break;
        case "D":
            pos = new PointWithSteps(pos.x, pos.y - delta, steps);
            break;
        }
        return new Line(oldPos, pos, v[0]);
    });
}

function calcAxisIntersection(line1: Line, line2: Line, axis: "x" | "y") {
    if (line1.p1[axis] === line1.p2[axis]) {
        if (line2.p1[axis] <= line1.p1[axis] && line1.p1[axis] <= line2.p2[axis]) {
            return line1.p1[axis];
        }
    } else if (line2.p1[axis] === line2.p2[axis]) {
        if (line1.p1[axis] <= line2.p1[axis] && line2.p1[axis] <= line1.p2[axis]) {
            return line2.p1[axis];
        }
    }
}

function calcIntersection(line1: Line, line2: Line) {
    const x = calcAxisIntersection(line1, line2, "x");
    const y = calcAxisIntersection(line1, line2, "y");
    if (x && y) {
        return new Point(x, y);
    }
}

function calcIntersectionPoints(wire1: Line[], wire2: Line[]) {
    const intersectionPoints: Array<[Point, Line, Line]> = [];
    for (const line1 of wire1) {
        for (const line2 of wire2) {
            const intersection = calcIntersection(line1, line2);
            if (intersection) {
                intersectionPoints.push([intersection, line1, line2]);
            }
        }
    }
    return intersectionPoints;
}

function manhattanDistance(p: Point) {
    return Math.abs(p.x) + Math.abs(p.y);
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function closestIntersection(wire1: string[], wire2: string[]) {
    return calcIntersectionPoints(calcWireLines(wire1), calcWireLines(wire2))
        .map(([p, _line1, _line2]) => p)
        .map(p => [p, manhattanDistance(p)])
        .reduce(([minp, mind], [p, d]) => d < mind ? [p, d] : [minp, mind]);
}

function shortestIntersection(wire1: string[], wire2: string[]) {
    // just aproximate the sum of steps, then check how much left till instersection
    return calcIntersectionPoints(calcWireLines(wire1), calcWireLines(wire2))
        .map(([p, line1, line2]) => [p, line1, line2, line1.p1.steps + line2.p1.steps] as const)
        .reduce(([minp, minLine1, minLine2, mind], [p, line1, line2, d]) =>
            d < mind ? [p, line1, line2, d] : [minp, minLine1, minLine2, mind]);
}

export function solve(lines: string[]) {
    const [wire1, wire2] = lines.map(line => line.split(","));
    const [p, line1, line2, d] = shortestIntersection(wire1, wire2);
    console.log(p, d);
    console.log(line1.p1, line1.p2);
    console.log(line2.p1, line2.p2);
}
