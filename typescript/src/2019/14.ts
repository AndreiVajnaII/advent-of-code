type Formula = Map<string, number>;

class Reaction {
    constructor(
        public quantity: number,
        public formula: Formula) {}
}

type Reactions = Map<string, Reaction>;

const trillion = 1000000000000;

export function solve(lines: string[]) {
    const reactions = parseReactions(lines);
    const orePerFuel = transform(parseFormula("1 FUEL"), reactions).get("ORE")!;
    let guess = Math.ceil(trillion / orePerFuel);
    let ore = 0;
    do {
        ore = transform(parseFormula(`${guess} FUEL`), reactions).get("ORE")!;
        console.log(guess, ore);
        const error = 1 - ore / trillion;
        guess += Math.ceil(0.5 * guess * error);
    } while (ore < trillion);
}

function transform(formula: Formula, reactions: Reactions) {
    while (!completelyTransformed(formula)) {
        const oldFormula = formula;
        formula = new Map<string, number>();
        for (const [name, oldQuantity] of oldFormula) {
            if (name === "ORE") {
                addChemical(formula, "ORE", oldQuantity);
            } else if (oldQuantity > 0) {
                const reaction = reactions.get(name)!;
                const multiplier = Math.ceil(oldQuantity / reaction.quantity);
                reaction.formula.forEach((q, n) => {
                    addChemical(formula, n, q * multiplier);
                });
                if (oldQuantity % reaction.quantity !== 0) {
                    addChemical(formula, name, (oldQuantity % reaction.quantity) - reaction.quantity);
                }
            }
        }
    }
    return formula;
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function printFormula(formula: Formula) {
    let s = "";
    formula.forEach((q, n) => {
        s += `${q} ${n}, `;
    });
    console.log(s);
}

function addChemical(formula: Formula, name: string, q: number) {
    const existing = formula.get(name);
    if (existing !== undefined) {
        q += existing;
    }
    if (q !== 0) {
        formula.set(name, q);
    } else {
        formula.delete(name);
    }
}

function completelyTransformed(formula: Formula) {
    for (const [name, q] of formula.entries()) {
        if (name !== "ORE" && q > 0) {
            return false;
        }
    }
    return true;
}

function parseReactions(lines: string[]) {
    const reactions = new Map<string, Reaction>();
    lines.forEach(s => {
        const [left, right] = s.split(" => ");
        const [qunatity, name] = parseTerm(right);
        reactions.set(name, new Reaction(qunatity, parseFormula(left)));
    });
    return reactions;
}

function parseTerm(term: string) {
    const [q, n] = term.split(" ");
    return [+q, n] as const;
}

function parseFormula(s: string) {
    const formula = new Map<string, number>();
    s.split(", ").map(t => parseTerm(t)).forEach(([q, n]) => {
        formula.set(n, q);
    });
    return formula;
}
