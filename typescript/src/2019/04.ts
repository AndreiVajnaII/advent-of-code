function hasDouble(p: string): boolean {
    let count = 0;
    for (let i = 1; i < p.length; i++) {
        if (p[i] === p[i - 1]) {
            count++;
        } else {
            if (count === 1) {
                return true;
            }
            count = 0;
        }
    }
    return count === 1;
}

function isAscending(p: string): boolean {
    for (let i = 1; i < p.length; i++) {
        if (p[i] < p[i - 1]) {
            return false;
        }
    }
    return true;
}

function isValid(p: string) {
    return hasDouble(p) && isAscending(p);
}

export function solve(lines: string[]) {
    const [a, b] = lines[0].split("-").map(s => +s);
    let count = 0;
    for (let p = a; p <= b; p++) {
        if (isValid(p.toString())) {
            count++;
        }
    }
    return count;
}
