import { boxed, concat } from "../utils";

export function solve(lines: string[]) {
    return boxed(lines.map(s => s.split("") as Tile[]))
        .reduce(seatMap => [
            occupySeats(seatMap, newSeat1),
            occupySeats(seatMap, newSeat2),
        ].map(newSeatMap => newSeatMap.reduce(concat).filter(tile => tile === "#").length));
}

function occupySeats(seatMap: Tile[][], newSeat: (map: Tile[][], i: number, j: number) => Tile) {
    let changedSeats: number;
    do {
        changedSeats = 0;
        const newSeatMap = new Array<Tile[]>(seatMap.length);
        for (let i = 0; i < seatMap.length; i++) {
            newSeatMap[i] = new Array<Tile>(seatMap[0].length);
        }
        for (let i = 0; i < seatMap.length; i++) {
            for (let j = 0; j < seatMap[0].length; j++) {
                newSeatMap[i][j] = newSeat(seatMap, i, j);
                if (newSeatMap[i][j] !== seatMap[i][j]) {
                    changedSeats++;
                }
            }
        }
        seatMap = newSeatMap;
    } while(changedSeats > 0);
    return seatMap;
}


function newSeat1(seatMap: Tile[][], i: number, j: number) {
    if (seatMap[i][j] === "L" && adjacents(seatMap, i, j).filter(seat => seat === "#").length === 0) {
        return "#";
    } else if (seatMap[i][j] === "#" && adjacents(seatMap, i, j).filter(seat => seat === "#").length >= 4) {
        return "L";
    } else {
        return seatMap[i][j];
    }
}

function newSeat2(seatMap: Tile[][], i: number, j: number) {
    if (seatMap[i][j] === "L" && seenSeats(seatMap, i, j).filter(seat => seat === "#").length === 0) {
        return "#";
    } else if (seatMap[i][j] === "#" && seenSeats(seatMap, i, j).filter(seat => seat === "#").length >= 5) {
        return "L";
    } else {
        return seatMap[i][j];
    }
}

function adjacents(seatMap: Tile[][], i: number, j: number) {
    return neighbours
        .map(([di, dj]) => [i + di, j + dj])
        .filter(([newi, newj]) => inBounds(seatMap, newi, newj))
        .map(([newi, newj]) => seatMap[newi][newj]);
}

function seenSeats(seatMap: Tile[][], i: number, j: number) {
    return neighbours.map(direction => lineOfSight(seatMap, i, j, direction));
}

function lineOfSight(seatMap: Tile[][], i: number, j: number, [di, dj]: readonly [number, number]) {
    i += di;
    j += dj;
    while (inBounds(seatMap, i, j)) {
        if (seatMap[i][j] === "#") {
            return "#";
        } else if (seatMap[i][j] === "L") {
            return "L";
        }
        i += di;
        j += dj;
    }
    return ".";
}

function inBounds(seatMap: Tile[][], i: number, j: number) {
    return i >= 0 && i < seatMap.length
        && j >= 0 && j < seatMap[0].length;
}

type Tile = "L" | "." | "#";

const neighbours = [[1, 1], [1, 0], [1, -1], [0, 1], [0, -1], [-1, 1], [-1, 0], [-1, -1]] as const;
