import { boxed, max, min, toNum } from "../utils";

export function solve(lines: string[]) {
    const initial = lines[0].split("").map(toNum);
    return [
        part1(new Cups(initial).playFor(100)),
        part2(new Cups(initial, 1_000_000).playFor(10_000_000)),
    ];
}

function part1(cups: Cups) {
    const one = cups.getNode(1);
    let result = "";
    for (let p = one.next; p !== one; p = p.next) {
        result += p.value;
    }
    return result;
}

function part2(cups: Cups) {
    return boxed(cups.getNode(1)).reduce(one => one.next.value * one.next.next.value);
}

class Cups {
    private current: LinkedNode;
    private min: number;
    private max: number;
    private nodeMap: LinkedNode[] = [];

    constructor(cups: readonly number[], maximum?: number) {
        this.min = cups.reduce(min);
        this.max = cups.reduce(max);
        this.current = new LinkedNode(cups[0]);
        let prev = this.current;
        this.nodeMap[this.current.value] = this.current;
        for (let i = 1; i < cups.length; i++) {
            prev = this.insertValueAfter(prev, cups[i]);
        }
        if (maximum) {
            do {
                prev = this.insertValueAfter(prev, ++this.max);
            } while (this.max < maximum);
        }
    }

    public playFor(rounds: number) {
        for (; rounds > 0; rounds--) {
            this.play();
        }
        return this;
    }

    public play() {
        const picked = [removeNext(this.current), removeNext(this.current), removeNext(this.current)];
        let destination = this.current.value - 1;
        if (destination < this.min) {
            destination = this.max;
        }
        while (picked.find(node => node.value === destination)) {
            destination--;
            if (destination < this.min) {
                destination = this.max;
            }
        }
        const destinationNode = this.nodeMap[destination];
        insertAfter(destinationNode, picked.pop()!);
        insertAfter(destinationNode, picked.pop()!);
        insertAfter(destinationNode, picked.pop()!);
        this.current = this.current.next;
    }

    public getNode(value: number) {
        return this.nodeMap[value];
    }

    private insertValueAfter(node: LinkedNode, value: number) {
        return this.nodeMap[value] = insertValueAfter(node, value);
    }
}

class LinkedNode {
    public next: LinkedNode;

    constructor(public value: number, next?: LinkedNode) {
        this.next = next || this;
    }
}

function insertValueAfter(node: LinkedNode, value: number) {
    return node.next = new LinkedNode(value, node.next);
}

function removeNext(node: LinkedNode) {
    const removed = node.next;
    node.next = removed.next;
    return removed;
}

function insertAfter(node: LinkedNode, toInsert: LinkedNode) {
    toInsert.next = node.next;
    node.next = toInsert;
}
