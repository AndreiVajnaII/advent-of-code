type Tile = "#" | ".";

type Row = [Tile, Tile, Tile, Tile, Tile];
type Midrow = [Tile, Tile, RecursiveLayout | undefined, Tile, Tile];

class RecursiveLayout {
    public layout: [Row, Row, Midrow, Row, Row];
    public bugCount = 0;

    private upper: RecursiveLayout | undefined;

    constructor() {
        this.layout = [this.emptyRow, this.emptyRow, this.midRow, this.emptyRow, this.emptyRow];
        this.bugCount = 0;
    }

    public assign(layout: Tile[][]) {
        this.bugCount = 0;
        for (let row = 0; row < layout.length; row++) {
            for (let col = 0; col < layout[row].length; col++) {
                if (row !== 2 || col !== 2) {
                    const t = layout[row][col];
                    this.layout[row][col] = t;
                    if (t === "#") {
                        this.bugCount++;
                    }
                }
            }
        }
    }

    public evolve() {
        this.upper = new RecursiveLayout();
        this.upper.layout[2][2] = this;
        this.upper.evolveTree();
        return this.upper;
    }

    private evolveTree() {
        const newLayout = this.evolveThis();
        if (!this.layout[2][2]) {
            this.layout[2][2] = new RecursiveLayout();
            this.layout[2][2].upper = this;
            const newLayoutBelow = this.layout[2][2].evolveThis();
            this.layout[2][2].assign(newLayoutBelow);
        } else {
            this.layout[2][2].evolveTree();
        }
        this.assign(newLayout);
        this.bugCount += this.layout[2][2].bugCount;
    }

    private evolveThis() {
        const newLayout: Tile[][] = [];
        for (let row = 0; row < this.layout.length; row++) {
            newLayout[row] = [];
            for (let col = 0; col < this.layout[row].length; col++) {
                if (row !== 2 || col !== 2) {
                    if (this.layout[row][col] === "#") {
                        newLayout[row][col] = this.adjacents(row, col)
                            .filter(t => t === "#").length === 1
                            ? "#" : ".";
                    } else {
                        const adjacentBugs = this.adjacents(row, col)
                            .filter(t => t === "#").length;
                        newLayout[row][col] = adjacentBugs === 1 || adjacentBugs === 2
                            ? "#" : ".";
                    }
                }
            }
        }
        return newLayout;
    }

    private adjacents(row: number, col: number) {
        const adj: Tile[] = [];
        if (col === 0) {
            adj.push(this.upper?.layout[2][1] || ".");
        } else if (col === 3 && row === 2) {
            for (let i = 0; i < 5; i++) {
                adj.push(this.layout[2][2]?.layout[i][4] || ".");
            }
        } else {
            adj.push(this.layout[row][col - 1] as Tile);
        }

        if (col === 4) {
            adj.push(this.upper?.layout[2][3] || ".");
        } else if (col === 1 && row === 2) {
            for (let i = 0; i < 5; i++) {
                adj.push(this.layout[2][2]?.layout[i][0] || ".");
            }
        } else {
            adj.push(this.layout[row][col + 1] as Tile);
        }

        if (row === 0) {
            adj.push(this.upper?.layout[1][2] || ".");
        } else if (row === 3 && col === 2) {
            for (let i = 0; i < 5; i++) {
                adj.push(this.layout[2][2]?.layout[4][i] || ".");
            }
        } else {
            adj.push(this.layout[row - 1][col] as Tile);
        }

        if (row === 4) {
            adj.push(this.upper?.layout[3][2] || ".");
        } else if (row === 1 && col === 2) {
            for (let i = 0; i < 5; i++) {
                adj.push(this.layout[2][2]?.layout[0][i] || ".");
            }
        } else {
            adj.push(this.layout[row + 1][col] as Tile);
        }

        return adj;
    }

    public get emptyRow(): Row {
        return [".", ".", ".", ".", "."];
    }

    public get midRow(): Midrow {
        return [".", ".", undefined, ".", "."];
    }
}

export function solve(lines: string[]) {
    let layout = lines.map(line => line.split("")) as Tile[][];
    const history: Tile[][][] = [];
    while (!exists(layout, history)) {
        history.push(layout);
        layout = evolve(layout);
    }
    const biodiversity = calcBiodiversity(layout);

    let layoutRec = new RecursiveLayout();
    layoutRec.assign(lines.map(line => line.split("")) as Tile[][]);

    for (let i = 0; i < 200; i++) {
        layoutRec = layoutRec.evolve();
    }

    return [biodiversity, layoutRec.bugCount];
}

function evolve(layout: Tile[][]) {
    const newLayout: Tile[][] = [];
    for (let row = 0; row < layout.length; row++) {
        newLayout[row] = [];
        for (let col = 0; col < layout[row].length; col++) {
            if (layout[row][col] === "#") {
                newLayout[row][col] = adjacents(layout, row, col)
                    .filter(t => t === "#").length === 1
                    ? "#" : ".";
            } else {
                const adjacentBugs = adjacents(layout, row, col)
                    .filter(t => t === "#").length;
                newLayout[row][col] = adjacentBugs === 1 || adjacentBugs === 2
                    ? "#" : ".";
            }
        }
    }
    return newLayout;
}

function adjacents(layout: Tile[][], row: number, col: number) {
    return [[-1, 0], [1, 0], [0, 1], [0, -1]]
        .map(([dr, dc]) => [row + dr, col + dc])
        .filter(([newRow, newCol]) => newRow >= 0 && newRow < layout.length
            && newCol >= 0 && newCol < layout[0].length)
        .map(([newRow, newCol]) => layout[newRow][newCol]);
}

function exists(layout: Tile[][], history: Tile[][][]) {
    return history.some(h => layoutsEqual(layout, h));
}

function layoutsEqual(layout1: Tile[][], layout2: Tile[][]) {
    for (let i = 0; i < layout1.length; i++) {
        for (let j = 0; j < layout1[i].length; j++) {
            if (layout1[i][j] !== layout2[i][j]) {
                return false;
            }
        }
    }
    return true;
}

function calcBiodiversity(layout: Tile[][]) {
    let r = 0;
    let p = 1;
    for (const row of layout) {
        for (const col of row) {
            if (col === "#") {
                r += p;
            }
            p *= 2;
        }
    }
    return r;
}
