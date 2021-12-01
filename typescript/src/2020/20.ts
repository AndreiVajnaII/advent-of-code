import { concat, groupLines, reverseString, rotateArray } from "../utils";

export function solve(lines: string[], fun = false) {
    const tiles = groupLines(lines).map(parseTile);
    const borders = new Map<string, Tile[]>();
    for (const tile of tiles) {
        for (const border of tile.borders) {
            if (!borders.has(border)) {
                borders.set(border, []);
            }
            borders.get(border)!.push(tile);
        }
    }

    const tilesForBorder = (border: string) =>
        (borders.get(border) || []).concat(borders.get(reverseString(border)) || []);

    const neighbours = (tile: Tile) =>
        tile.borders.map(border => tilesForBorder(border).find(t => t !== tile) || null);

    const tileGrid: Tile[][] = [];
    let current = tiles.find(t => neighbours(t).filter(n => n === null).length === 2)!;
    let row = 0;
    while (current != null) {
        tileGrid.push([]);
        let col = 0;
        while (current !== null) {
            current.flipUntil(tile =>
                neighbours(tile)[LEFT] === (tileGrid[row][col - 1] || null)
                && neighbours(tile)[TOP] === ((tileGrid[row - 1] || [])[col] || null));
            tileGrid[row].push(current);
            current = neighbours(current)[RIGHT]!;
            col++;
        }
        current = neighbours(tileGrid[row][0])[BOTTOM]!;
        row++;
    }

    const image = new Tile(0, tileGrid.map(gridRow =>
        gridRow.map(tile => tile.data.slice(1, tile.data.length - 1)
            .map(tileRow => tileRow.slice(1, tileRow.length - 1)))
            .reduce((a, b) => a.map((tileRow, i) => tileRow.concat(b[i]))))
        .reduce(concat));
    image.flipUntil(containsSeaMonster);
    if (fun) {
        image.print();
    }

    return [
        tileGrid[0][0].id
        * tileGrid[0][tileGrid[0].length - 1].id
        * tileGrid[tileGrid.length - 1][0].id
        * tileGrid[tileGrid.length - 1][tileGrid[0].length - 1].id,
        image.data.reduce(concat).filter(p => p === "#").length,
    ];
}

function parseTile([header, ...data]: string[]) {
    return new Tile(+/Tile (\d+)/.exec(header)![1], data.map(s => s.split("")));
}

function containsSeaMonster(tile: Tile) {
    const seaMonster = [
        "                  # ",
        "#    ##    ##    ###",
        " #  #  #  #  #  #   ",
    ].map(s => s.split(""));
    let found = false;
    for (let row = 0; row < tile.data.length - seaMonster.length; row++) {
        for (let col = 0; col < tile.data[row].length - seaMonster[0].length; col++) {
            if (matchesImage(tile.data, seaMonster, row, col)) {
                found = true;
                for (let i = 0; i < seaMonster.length; i++) {
                    for (let j = 0; j < seaMonster[i].length; j++) {
                        if (seaMonster[i][j] === "#") {
                            tile.data[row + i][col + j] = "0";
                        }
                    }
                }
            }
        }
    }
    return found;
}

function matchesImage(data: string[][], image: string[][], row: number, col: number) {
    for (let i = 0; i < image.length; i++) {
        for (let j = 0; j < image[i].length; j++) {
            if (image[i][j] === "#" && data[row + i][col + j] !== "#") {
                return false;
            }
        }
    }
    return true;
}

class Tile {
    constructor(public id: number, public data: string[][]) { }

    public get borders() {
        return [
            this.data[0].join(""),
            this.data.map(row => row[row.length - 1]).join(""),
            this.data[this.data.length - 1].join(""),
            this.data.map(row => row[0]).join(""),
        ] as const;
    }

    public print() {
        console.log(this.data.map(row => row.join("")).join("\n"));
    }

    public flip() {
        this.data = this.data.reverse();
    }

    public rotate() {
        this.data = rotateArray(this.data);
    }

    public flipUntil(condition: (tile: Tile) => boolean) {
        if (this.rotateUntil(condition)) {
            return true;
        } else {
            this.flip();
            return this.rotateUntil(condition);
        }

    }

    public rotateUntil(condition: (tile: Tile) => boolean) {
        for (let i = 0; i < 4; i++) {
            if (condition(this)) {
                return true;
            }
            this.rotate();
        }
        return false;
    }
}

const TOP = 0;
const RIGHT = 1;
const BOTTOM = 2;
const LEFT = 3;
