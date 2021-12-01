function countDigits(layer: number[], digit: number) {
    return layer.filter(x => x === digit).length;
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function checkImage(layers: number[][]) {
    const minLayer = layers[layers
        .map(layer => countDigits(layer, 0))
        .reduce((mini, x, i, arr) => x < arr[mini] ? i : mini, 0)];
    return countDigits(minLayer, 1) * countDigits(minLayer, 2);
}

function decodePixel(layers: number[][], pos: number) {
    for (const layer of layers) {
        if (layer[pos] === 0) {
            return " ";
        } else if (layer[pos] === 1) {
            return "*";
        }
    }
    return "?";
}

function decodeImage(layers: number[][], w: number, h: number) {
    for (let y = 0; y < h; y++) {
        const line: string[] = [];
        for (let x = 0; x < w; x++) {
            line.push(decodePixel(layers, y * w + x));
        }
        console.log(line.join(""));
    }
}

export function solve(lines: string[]) {
    const w = 25;
    const h = 6;
    const layers: number[][] = [];
    const data = lines[0].split("").map(s => +s);
    for (let i = 0; i < data.length; i += w * h) {
        layers.push(data.slice(i, i + w * h));
    }
    decodeImage(layers, w, h);
}
