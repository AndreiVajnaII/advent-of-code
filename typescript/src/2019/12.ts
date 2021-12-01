const axes = ["x", "y", "z"] as const;

class Vector {
    constructor(
        public x: number,
        public y: number,
        public z: number,
    ) { }

    public get energy() {
        return axes.map(axis => Math.abs(this[axis])).reduce((r, x) => r + x);
    }

}

class Moon {
    constructor(
        public position: Vector,
        public velocity = new Vector(0, 0, 0)) { }

    public get potentialEnergy() {
        return this.position.energy;
    }

    public get kineticEnergy() {
        return this.velocity.energy;
    }

    public get totalEnergy() {
        return this.potentialEnergy * this.kineticEnergy;
    }
}

function applyGravity(m1: Moon, m2: Moon) {
    axes.forEach(axis => {
        m1.velocity[axis] += Math.sign(m2.position[axis] - m1.position[axis]);
        m2.velocity[axis] += Math.sign(m1.position[axis] - m2.position[axis]);
    });
}

function applyVelocity(m: Moon) {
    axes.forEach(axis => {
        m.position[axis] += m.velocity[axis];
    });
}

class Pattern {
    public found = false;
    private history: number[] = [];

    constructor(private getValue: (moons: Moon[]) => number) { }

    public get length() {
        return this.history.length / 2;
    }

    public check(moons: Moon[]) {
        this.history.push(this.getValue(moons));
        return this.checkValues();
    }

    private checkValues() {
        if (this.history.length % 2 === 0) {
            for (let i = 0; i < this.history.length / 2; i++) {
                if (this.history[i] !== this.history[this.history.length / 2 + i]) {
                    return;
                }
            }
            this.found = true;
        }
    }

}

export function solve(lines: string[]) {
    const moons = lines.map(parseVector).map(v => new Moon(v));
    const patterns: Pattern[] = [];
    for (let i = 0; i < moons.length; i++) {
        patterns.push(...axes.map(axis => new Pattern(m => m[i].position[axis])));
        patterns.push(...axes.map(axis => new Pattern(m => m[i].velocity[axis])));
    }
    patterns.forEach(p => {
        p.check(moons);
    });
    let step = 0;
    do {
        step++;
        for (let i = 0; i < moons.length - 1; i++) {
            for (let j = i + 1; j < moons.length; j++) {
                applyGravity(moons[i], moons[j]);
            }
        }
        moons.forEach(applyVelocity);
        patterns.filter(p => !p.found).forEach(p => {
            p.check(moons);
        });
        if (step % 100 === 0) {
            console.log(step, `Found  ${patterns.filter(p => p.found).length}/${patterns.length} cycles`);
        }
    } while (!patterns.every(p => p.found));
    console.log(step);
    return mlcm(patterns.map(p => p.length));
}

function parseVector(line: string) {
    const m = /<x=([^,]+), y=([^,]+), z=([^>]+)>/.exec(line);
    if (!m) {
        throw new Error("error parsing input");
    }
    return new Vector(+m[1], +m[2], + m[3]);
}

function mlcm(values: number[]) {
    return values.reduce((r, x) => lcm(r, x));
}

function lcm(a: number, b: number) {
    return a * b / gcd(a, b);
}

function gcd(a: number, b: number): number {
    while (b !== 0) {
        [a, b] = [b, a % b];
    }
    return a;
}
