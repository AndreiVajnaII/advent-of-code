import { neighbours, OneDimensionalArray, RecursiveArray } from "../utils";

export function solve(lines: string[]) {
    const initial = lines.map(line => line.split("") as CubeState[]);
    return [3, 4].map(dimensions => new CubeSpace(initial, dimensions))
        .map(cubeSpace => {
            for (let i = 0; i < 6; i++) {
                cubeSpace.evolve();
            }
            return cubeSpace.count;
        });
}

class CubeSpace {
    private cubes: RecursiveArray<CubeState>;
    private neighbours;

    constructor(initial: CubeState[][], dimensions: number) {
        const lengths = [initial.length, initial[0].length];
        const coordPrefix = [];
        while (lengths.length < dimensions) {
            lengths.unshift(1);
            coordPrefix.push(0);
        }
        this.cubes = new RecursiveArray<CubeState>(lengths);
        for (let y = 0; y < initial.length; y++) {
            for (let x = 0; x < initial[y].length; x++) {
                this.cubes.setValue([...coordPrefix, y, x], initial[y][x]);
            }
        }
        this.neighbours = neighbours(this.cubes.dimensions);
    }

    public evolve() {
        pad(this.cubes, ".");
        const newCubes = new RecursiveArray<CubeState>(this.cubes.lengths);
        this.cubes.forEach((cube, coords) => {
            const activeNeighbours = this.neighbours
                .map(delta => coords.map((coord, i) => coord + delta[i]))
                .filter(newCoords => this.cubes.isInBounds(newCoords))
                .map(newCoords => this.cubes.getValue(newCoords))
                .filter(c => c === "#").length;
            if (cube === "#") {
                newCubes.setValue(coords, activeNeighbours === 2 || activeNeighbours === 3 ? "#" : ".");
            } else {
                newCubes.setValue(coords, activeNeighbours === 3 ? "#" : ".");
            }
        });
        this.cubes = newCubes;
    }

    public get count() {
        const result: CubeState[] = [];
        this.cubes.forEach(cube => {
            if (cube === "#") {
                result.push(cube);
            }
        });
        return result.length;
    }
}

function pad<T>(space: RecursiveArray<T> | OneDimensionalArray<T>, value: T) {
    if (space instanceof OneDimensionalArray) {
        space.push(value);
        space.unshift(value);
    } else {
        for (const layer of space.array) {
            pad(layer, value);
        }
        space.push(newLayer<T>(space.lengths.slice(1)).fill(value));
        space.unshift(newLayer<T>(space.lengths.slice(1)).fill(value));
    }
}

function newLayer<T>(lengths: number[]) {
    if (lengths.length === 1) {
        return new OneDimensionalArray<T>(lengths[0]);
    } else {
        return new RecursiveArray<T>(lengths);
    }
}

type CubeState = "#" | ".";
