import { groupLines } from "../utils";

export function solve(lines: string[]) {
    const [rulesGroup, messages] = groupLines(lines);
    const part1Rules = new Map(rulesGroup.map(parseRuleEntry));
    rulesGroup.push("8: 42 | 42 8", "11: 42 31 | 42 11 31");
    const part2Rules = new Map(rulesGroup.map(parseRuleEntry));
    return [
        messages.filter(isValid(part1Rules)).length,
        messages.filter(isValid(part2Rules)).length,
    ];
}

function parseRuleEntry(line: string) {
    const [id, rule] = line.split(": ");
    return [id, parseRule(rule)] as const;
}

function parseRule(s: string): Rule {
    if (s.startsWith("\"")) {
        return new Char(s.charAt(1));
    } else if (s.includes("|")) {
        return new Pipe(s.split(" | ").map(parseRule));
    } else {
        return new SequenceRule(s.split(" "));
    }
}

function isValid(rules: Map<string, Rule>) {
    const rule = rules.get("0")!;
    return (s: string) => {
        for (const r of rule.exec(s, 0, rules)) {
            if (r === s.length) {
                return true;
            }
        }
        return false;
    };
}

interface Rule {
    exec(s: string, index: number, rules: Map<string, Rule>, next?: Rule): Generator<number>;
}

class SequenceRule implements Rule {
    constructor(private readonly ruleIds: string[]) { }

    public exec(s: string, index: number, rules: Map<string, Rule>) {
        return this.execRule(this.ruleIds, s, index, rules);
    }

    private * execRule(ruleIds: string[], s: string, index: number, rules: Map<string, Rule>): Generator<number> {
        if (ruleIds.length === 0) {
            yield index;
        } else {
            const rule = rules.get(ruleIds[0])!;
            const newIndices = rule.exec(s, index, rules);
            for (const i of newIndices) {
                if (i >= 0) {
                    for (const r of this.execRule(ruleIds.slice(1), s, i, rules)) {
                        yield r;
                    }
                } else {
                    yield -1;
                }
            }
        }
    }
}

class Char implements Rule {
    constructor(private char: string) { }

    public * exec(s: string, index: number) {
        yield s.charAt(index) === this.char ? index + 1 : -1;
    }
}

class Pipe implements Rule {
    constructor(private rules: Rule[]) { }

    public * exec(s: string, index: number, rules: Map<string, Rule>) {
        for (const rule of this.rules) {
            for (const r of rule.exec(s, index, rules)) {
                yield r;
            }
        }
    }
}
