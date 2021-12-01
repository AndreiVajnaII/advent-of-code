import { arraysEqual, groupLines, sum, toNum } from "../utils";

export function solve(lines: string[]) {
    const [deck1, deck2] = groupLines(lines).map(group => group.slice(1).map(toNum));
    return [
        combat(deck1.slice(), deck2.slice()),
        recursiveCombat(deck1.slice(), deck2.slice())[1],
    ];
}

function combat(deck1: number[], deck2: number[]) {
    while (deck1.length !== 0 && deck2.length !== 0) {
        const card1 = deck1.shift()!;
        const card2 = deck2.shift()!;
        const [winner, high, low] = card1 > card2 ? [deck1, card1, card2] : [deck2, card2, card1];
        winner.push(high, low);
    }
    return deck1.length > 0 ? score(deck1) : score(deck2);
}

function recursiveCombat(deck1: number[], deck2: number[]): [winner: number, score: number] {
    const prev = new DeckSet();
    while (deck1.length !== 0 && deck2.length !== 0) {
        if (prev.has(deck1, deck2)) {
            return [1, score(deck1)];
        }
        prev.add(deck1, deck2);
        const card1 = deck1.shift()!;
        const card2 = deck2.shift()!;
        if (deck1.length < card1 || deck2.length < card2) {
            const [winner, high, low] = card1 > card2 ? [deck1, card1, card2] : [deck2, card2, card1];
            winner.push(high, low);
        } else {
            const [winner, high, low] =
                recursiveCombat(deck1.slice(0, card1), deck2.slice(0, card2))[0] === 1
                    ? [deck1, card1, card2] as const
                    : [deck2, card2, card1] as const;
            winner.push(high, low);
        }
    }
    return (deck1.length > 0 ? [1, score(deck1)] : [2, score(deck2)]) as [number, number];
}

class DeckSet {
    private map = new Map<number, Array<readonly [number[], number[]]>>();

    public add(deck1: number[], deck2: number[]) {
        const key = score(deck1) + score(deck2);
        if (!this.map.has(key)) {
            this.map.set(key, []);
        }
        this.map.get(key)!.push([deck1.slice(), deck2.slice()]);
    }

    public has(deck1: number[], deck2: number[]) {
        const key = score(deck1) + score(deck2);
        for (const [d1, d2] of this.map.get(key) || []) {
            if (arraysEqual(d1, deck1) && arraysEqual(d2, deck2)) {
                return true;
            }
        }
        return false;
    }
}

function score(deck: number[]) {
    return deck.slice().reverse().map((v, i) => v * (i + 1)).reduce(sum);
}
