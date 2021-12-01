import { sum } from "../utils";

export function solve(lines: string[]) {
    const expressions = lines.map(line => `(${line})`).map(parseExpr);
    return [evaluate1, evaluate2].map(evaluate => solveHomework(expressions, evaluate));
}

function parseExpr(line: string) {
    return line.split(" ").map(s => {
        const result: string[] = [];
        if (s.endsWith(")")) {
            while (s.endsWith(")")) {
                result.push(")");
                s = s.slice(0, s.length - 1);
            }
            result.unshift(s);
        } else {
            while (s.startsWith("(")) {
                result.push("(");
                s = s.slice(1);
            }
            result.push(s);
        }
        return result;
    }).flat();
}

function solveHomework(expressions: string[][], evaluate: (expr: string[]) => number) {
    return expressions.map(evaluate).reduce(sum);
}

function evaluate1(expr: string[]): number {
    const numberStack: number[] = [];
    const opStack: string[] = [];
    for (const term of expr) {
        if (term === "+" || term === "*" || term === "(") {
            opStack.push(term);
        } else {
            if (term === ")") {
                opStack.pop(); // remove (
            } else {
                numberStack.push(+term);
            }
            while (opStack.length > 0 && opStack[opStack.length - 1] !== "(") {
                const a = numberStack.pop()!;
                const b = numberStack.pop()!;
                numberStack.push(opStack.pop() === "+" ? a + b : a * b);
            }
        }
    }
    return numberStack[0];
}

function evaluate2(expr: string[]): number {
    const numberStack: number[] = [];
    const opStack: string[] = [];
    for (const term of expr) {
        if (term === "+" || term === "*" || term === "(") {
            opStack.push(term);
        } else {
            if (term === ")") {
                while (opStack.length > 0 && opStack[opStack.length - 1] !== "(") {
                    opStack.pop();
                    const a = numberStack.pop()!;
                    const b = numberStack.pop()!;
                    numberStack.push(a * b);
                }
                opStack.pop(); // remove (
            } else {
                numberStack.push(+term);
            }
            while (opStack.length > 0 && opStack[opStack.length - 1] === "+") {
                opStack.pop();
                const a = numberStack.pop()!;
                const b = numberStack.pop()!;
                numberStack.push(a + b);
            }
        }
    }
    return numberStack[0];
}
