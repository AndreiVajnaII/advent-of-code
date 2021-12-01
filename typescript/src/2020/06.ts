import { boxed, CollectFunction, groupLines, intersect, sum, union } from "../utils";

export function solve(lines: string[]) {
    return boxed(groupLines(lines).map(splitAnswers))
        .reduce(groupedAnswers =>
            [union, intersect].map(collect =>
                groupedAnswers.map(collectAnswers(collect))
                    .map(answers => answers.size)
                    .reduce(sum)));
}

function splitAnswers(group: string[]) {
    return group.map(s => new Set(s.split("")));
}

function collectAnswers(collect: CollectFunction<string>) {
    return (answers: Array<Set<string>>) => answers.reduce(collect);
}
