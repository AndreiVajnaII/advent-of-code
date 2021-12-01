export type ValuesOf<T extends readonly any[]> = T extends ReadonlyArray<infer R> ? R : never;

export const toNum = (s: string) => +s;

export const sum = (a: number, b: number) => a + b;
export const product = (a: number, b: number) => a * b;
export const max = (a: number, b: number) => a > b ? a : b;
export const min = (a: number, b: number) => a < b ? a : b;
export const ascending = (a: number, b: number) => a - b;

export const withOriginal = <T, R>(f: (x: T, index: number) => R) =>
    (x: T, index: number) => [x, f(x, index)] as const;
export const mapped = <T>([_o, m]: readonly [any, T]) => m;
export const original = <T>([o, _m]: readonly [T, any]) => o;
export const byOriginal = <T, R>(f: (x: T) => R) =>
    ([o, _m]: readonly [T, any]) => f(o);
export const ofOriginal = <T, M, R>(f: (x: T) => R) =>
    ([o, m]: readonly [T, M]) => [f(o), m] as const;

export const minOf = <T>(f: (x: T) => number) => (a: T, b: T) => f(a) < f(b) ? a : b;
export const maxOf = <T>(f: (x: T) => number) => (a: T, b: T) => f(a) > f(b) ? a : b;

export const concat = <T>(a: T[], b: T[]) => a.concat(b);

export function createRange(start: number, end: number) {
    const range: number[] = [];
    for (let i = 0; i < end - start; i++) {
        range[i] = start + i;
    }
    return range;
}

export const replace = <T>(array: T[], index: number, value: T) =>
    [...array.slice(0, index), value, ...array.slice(index + 1)];

export const arraysEqual = <T>(a: T[], b: T[]) =>
    a.length === b.length && a.every((v, i) => v === b[i]);

export function rotateArray<T>(arr: T[][]) {
    const result = new Array<T[]>(arr.length);
    for (let row = 0; row < arr.length; row++) {
        result[row] = new Array<T>(arr[row].length);
    }
    for (let row = 0; row < arr.length; row++) {
        for (let col = 0; col < arr[row].length; col++) {
            result[row][col] = arr[arr[row].length - col - 1][row];
        }
    }
    return result;
}

export function lcm(a: number, b: number) {
    return a * b / gcd(a, b);
}

export function gcd(a: number, b: number): number {
    while (b !== 0) {
        [a, b] = [b, a % b];
    }
    return a;
}

export const manhattan = ({ x, y }: { x: number; y: number }) => Math.abs(x) + Math.abs(y);

export function isUpperCase(s: string) {
    return s.charCodeAt(0) >= "A".charCodeAt(0) && s.charCodeAt(0) <= "Z".charCodeAt(0);
}

export function isLowerCase(s: string) {
    return s.charCodeAt(0) >= "a".charCodeAt(0) && s.charCodeAt(0) <= "z".charCodeAt(0);
}

export function reverseString(s: string) {
    return s.split("").reverse().join("");
}

export function objFromEntries<K extends keyof any, V>(entries: Iterable<readonly [K, V]>) {
    const result: Partial<Record<K, V>> = {};
    for (const [key, value] of entries) {
        result[key] = value;
    }
    return result;
}

export class Box<T> {
    constructor(public value: T) { }

    public map<R>(f: (value: T) => R) {
        return new Box(f(this.value));
    }

    public reduce<R>(f: (value: T) => R) {
        return f(this.value);
    }
}

export const boxed = <T>(value: T) => new Box(value);

export type CollectFunction<T> = (a: Set<T>, b: Set<T>) => Set<T>;

export function union<T>(a: Set<T>, b: Set<T>) {
    const result = new Set<T>();
    a.forEach(x => {
        result.add(x);
    });
    b.forEach(x => {
        result.add(x);
    });
    return result;
}

export function intersect<T>(a: Set<T>, b: Set<T>) {
    const result = new Set<T>();
    a.forEach(x => {
        if (b.has(x)) {
            result.add(x);
        }
    });
    return result;
}

export function single<T>(set: Set<T>) {
    return set.values().next().value as T;
}

// groups lines separated by blank lines
export function groupLines(lines: string[]) {
    const result: string[][] = [];
    let newGroup = true;
    for (const line of lines) {
        if (line === "") {
            newGroup = true;
        } else {
            if (newGroup) {
                result.push([line]);
                newGroup = false;
            } else {
                result[result.length - 1].push(line);
            }
        }
    }
    return result;
}
export class RecursiveArray<T> {
    public dimensions: number;

    public array: Array<RecursiveArray<T> | OneDimensionalArray<T>>;

    constructor([length1, length2, ...rest]: number[]) {
        this.array = new Array<RecursiveArray<T> | OneDimensionalArray<T>>(length1);
        for (let i = 0; i < this.array.length; i++) {
            this.array[i] = rest.length === 0
                ? new OneDimensionalArray<T>(length2)
                : new RecursiveArray<T>([length2, ...rest]);
        }
        this.dimensions = this.array[0].dimensions + 1;
    }

    public get lengths(): number[] {
        return [this.array.length].concat(this.array[0].lengths);
    }

    public getValue([index, ...rest]: number[]): T {
        return this.array[index].getValue(rest);
    }

    public setValue([index, ...rest]: number[], value: T) {
        this.array[index].setValue(rest, value);
    }

    public push(value: RecursiveArray<T> | OneDimensionalArray<T>) {
        this.array.push(value);
    }

    public unshift(value: RecursiveArray<T> | OneDimensionalArray<T>) {
        this.array.unshift(value);
    }

    public fill(value: T) {
        this.array.forEach(subArray => subArray.fill(value));
        return this;
    }

    public isInBounds([index, ...rest]: number[]): boolean {
        return 0 <= index && index < this.array.length && this.array[0].isInBounds(rest);
    }

    public forEach(f: (element: T, indices: number[]) => void, currentIndices: number[] = []) {
        this.array.forEach((subArray, index) => subArray.forEach(f, currentIndices.concat(index)));
    }

}

export class OneDimensionalArray<T> {
    public dimensions = 1;

    private array: T[];

    constructor(length: number) {
        this.array = new Array<T>(length);
    }

    public get lengths(): number[] {
        return [this.array.length];
    }

    public getValue([index]: number[]) {
        return this.array[index];
    }

    public setValue([index]: number[], value: T) {
        this.array[index] = value;
    }

    public push(value: T) {
        this.array.push(value);
    }

    public unshift(value: T) {
        this.array.unshift(value);
    }

    public fill(value: T) {
        this.array.fill(value);
        return this;
    }

    public isInBounds([index]: number[]) {
        return 0 <= index && index < this.array.length;
    }

    public forEach(f: (element: T, indices: number[]) => void, currentIndices: number[] = []) {
        this.array.forEach((el, index) => f(el, currentIndices.concat(index)));
    }
}

const neighbours1 = [[0], [1], [-1]];

export function neighbours(dimensions: number, top = true): number[][] {
    if (dimensions === 1) {
        return neighbours1;
    } else {
        const result = neighbours(dimensions - 1, false).flatMap(r => neighbours1.map(n => n.concat(r)));
        if (top) {
            return result.slice(1);
        } else {
            return result;
        }
    }
}

export class TupleSet<T extends readonly number[] | readonly string[]> implements Set<T> {
    private readonly set = new Set<string>();

    constructor(private readonly isNumber = true) { }

    public add(value: T): this {
        this.set.add(this.tupleToKey(value));
        return this;
    }

    public has(value: T): boolean {
        return this.set.has(this.tupleToKey(value));
    }

    public delete(value: T): boolean {
        return this.set.delete(this.tupleToKey(value));
    }

    public forEach(callbackfn: (value: T, value2: T, set: Set<T>) => void, thisArg?: any): void {
        this.set.forEach(key => callbackfn(this.keyToTuple(key), this.keyToTuple(key), this), thisArg);
    }

    public clear(): void {
        this.set.clear();
    }

    public get size(): number {
        return this.set.size;
    }

    public * keys(): IterableIterator<T> {
        for (const key of this.set) {
            yield this.keyToTuple(key);
        }
    }

    public values(): IterableIterator<T> {
        return this.keys();
    }

    public * entries(): IterableIterator<[T, T]> {
        for (const tuple of this.keys()) {
            yield [tuple, tuple];
        }
    }

    public [Symbol.iterator](): IterableIterator<T> {
        return this.keys();
    }

    public get [Symbol.toStringTag](): string {
        const values: T[] = [];
        const it = this.values();
        for (let v = it.next(), n = 0; n < 5 && !v.done; v = it.next(), n++) {
            values.push(v.value);
        }
        let valuesEnumeration = values.join("; ");
        if (values.length < this.size) {
            valuesEnumeration += "; ...";
        }
        return `TupleSet(${this.size}) {${valuesEnumeration}}`;
    }

    private tupleToKey(tuple: T) {
        return tuple.join(",");
    }

    private keyToTuple(key: string): T {
        const tuple = key.split(",") as readonly string[];
        return (this.isNumber ? tuple.map(toNum) as readonly number[] : tuple) as T;
    }
}
