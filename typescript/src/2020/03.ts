import { product } from "../utils";

export function solve(map: string[]) {
    return [[1, 1], [1, 3], [1, 5], [1, 7], [2, 1]]
        .map(([slopeY, slopeX]) => countTrees(map, slopeY, slopeX))
        .reduce(product);
}

function countTrees(map: string[], slopeY: number, slopeX: number) {
    let count = 0;
    for (let y = 0, x = 0; y < map.length; y += slopeY, x = (x + slopeX) % map[0].length) {
        if (map[y][x] === "#") {
            count++;
        }
    }
    return count;
}
