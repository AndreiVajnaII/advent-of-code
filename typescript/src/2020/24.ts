import { TupleSet } from "../utils";

export function solve(lines: string[]) {
    let blackTiles = new TupleSet<Coord>();
    for (const line of lines) {
        let [row, col] = [0, 0];
        for (const direction of parseLine(line)) {
            [row, col] = applyDirection(direction, row, col);
        }
        if (blackTiles.has([row, col])) {
            blackTiles.delete([row, col]);
        } else {
            blackTiles.add([row, col]);
        }
    }
    const numberOfInitalBlackTiles = blackTiles.size;
    for (let day = 0; day < 100; day++) {
        const newBlackTiles = new TupleSet<Coord>();
        const whiteTiles = new TupleSet<Coord>();
        for (const [row, col] of blackTiles) {
            const neighbours = adjacents.map(([dr, dc]) => [row + dr, col + dc] as const);
            const blackNeighbours = neighbours.filter(tile => blackTiles.has(tile)).length;
            if (blackNeighbours > 0 && blackNeighbours <= 2) {
                newBlackTiles.add([row, col]);
            }
            for (const [nRow, nCol] of neighbours.filter(tile => !blackTiles.has(tile) && !whiteTiles.has(tile))) {
                whiteTiles.add([nRow, nCol]);
                const nNeighbours = adjacents.map(([dr, dc]) => [nRow + dr, nCol + dc] as const);
                const nBlackNeighbours = nNeighbours.filter(tile => blackTiles.has(tile)).length;
                if (nBlackNeighbours === 2) {
                    newBlackTiles.add([nRow, nCol]);
                }
            }
        }
        blackTiles = newBlackTiles;
    }
    return [numberOfInitalBlackTiles, blackTiles.size];
}

function* parseLine(line: string) {
    const regexp = /e|se|ne|w|sw|nw/g;
    let match: ReturnType<typeof regexp.exec>;
    while ((match = regexp.exec(line)) !== null) {
        yield match[0] as Direction;
    }
}

function applyDirection(direction: Direction, row: number, col: number) {
    const [dr, dc] = directions[direction];
    return [row + dr, col + dc] as const;
}

type Coord = readonly [number, number];

const directions = {
    e: [0, 2],
    w: [0, -2],
    ne: [1, 1],
    nw: [1, -1],
    se: [-1, 1],
    sw: [-1, -1],
} as const;

const adjacents = Object.values(directions);

type Direction = keyof typeof directions;
