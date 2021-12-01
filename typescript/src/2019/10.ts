class Asteroid {
    public seenAsteroids = 0;

    constructor(
        public x: number,
        public y: number) { }
}

class LineOfSight {
    constructor(
        private startX: number,
        private startY: number,
        private dx: number,
        private dy: number) { }

    public sees(a: Asteroid): boolean {
        if (a.x === this.startX && a.y === this.startY) {
            return false;
        }
        if (a.x === this.startX) {
            return this.dx === 0 && Math.sign(a.y - this.startY) === Math.sign(this.dy);
        }
        if (a.y === this.startY) {
            return this.dy === 0 && Math.sign(a.x - this.startX) === Math.sign(this.dx);
        }
        return (a.x - this.startX) / this.dx === (a.y - this.startY) / this.dy
            && Math.sign(a.x - this.startX) === Math.sign(this.dx)
            && Math.sign(a.y - this.startY) === Math.sign(this.dy);
    }

    public get quadrant() {
        if (this.dy < 0 && this.dx >= 0) {
            return 1;
        } else if (this.dx > 0 && this.dy >= 0) {
            return 2;
        } else if (this.dy > 0 && this.dx <= 0) {
            return 3;
        } else {
            return 4;
        }
    }

    public get slope() {
        return this.dy / this.dx;
    }

    public shoot(point?: [number, number]) {
        const [x, y] = point ?? [this.startX, this.startY];
        return [x + this.dx, y + this.dy] as const;
    }
}

function compLoS(los1: LineOfSight, los2: LineOfSight): number {
    if (los1.quadrant !== los2.quadrant) {
        return los1.quadrant - los2.quadrant;
    }
    return los1.slope - los2.slope;
}

export function solve(lines: string[]) {
    const stationFinder = new MonitoringStationFinder(lines);
    const [a, _] = stationFinder.findBest();
    stationFinder.buildStation(a);
    const linesOfSight = stationFinder.linesOfSight(a.x, a.y);
    const pulverized: Asteroid[] = [];
    while (stationFinder.hasAsteroids) {
        linesOfSight.forEach(los => {
            pulverized.push(...stationFinder.pulverize(los));
        });
    }
    return pulverized[199];
}

class MonitoringStationFinder {

    private w: number;
    private h: number;
    private asteroids: Asteroid[];

    constructor(lines: string[]) {
        this.w = lines[0].length;
        this.h = lines.length;
        this.asteroids = lines.join("").split("").map((v, i) => [v, i] as const).filter(([v, _]) => v === "#")
            .map(([_, i]) => new Asteroid(i % this.w, Math.floor(i / this.w)));
    }

    public get hasAsteroids() {
        return this.asteroids.length > 0;
    }

    public findBest() {
        return this.asteroids.map(a => [a, this.countSeenAsteroids(a)] as const)
            .reduce(([amax, max], [a, v]) => v > max ? [a, v] : [amax, max]);
    }

    public buildStation(a: Asteroid) {
        this.removeAsteroid(a.x, a.y);
    }

    public pulverize(los: LineOfSight) {
        let [x, y] = los.shoot();
        while (x >= 0 && y >= 0 && x < this.w && y < this.h) {
            const a = this.removeAsteroid(x, y);
            if (a) {
                return a;
            }
            [x, y] = los.shoot([x, y]);
        }
        return [];
    }

    public countSeenAsteroids(a: Asteroid) {
        return this.linesOfSight(a.x, a.y)
            .filter(lineOfSight => this.asteroids.some(asteroid => lineOfSight.sees(asteroid))).length;
    }

    public linesOfSight(startX: number, startY: number): LineOfSight[] {
        const linesOfSight: LineOfSight[] = [];
        for (let x = 0; x < this.w; x++) {
            for (let y = 0; y < this.h; y++) {
                if (x === startX && y === startY) {
                    continue;
                }
                const dx = x - startX;
                const dy = y - startY;
                if (gcd(Math.abs(dx), Math.abs(dy)) === 1) {
                    linesOfSight.push(new LineOfSight(startX, startY, dx, dy));
                }
            }
        }
        return linesOfSight.sort(compLoS);
    }

    private removeAsteroid(x: number, y: number) {
        const i = this.asteroids.findIndex(a => a.x === x && a.y === y);
        if (i >= 0) {
            return this.asteroids.splice(i, 1);
        }
    }
}

function gcd(a: number, b: number): number {
    if (a === 0) {
        return b;
    }
    if (b === 0) {
        return a;
    }
    if (a === b) {
        return a;
    }
    if (a > b) {
        return gcd(a - b, b);
    } else {
        return gcd(a, b - a);
    }
}
