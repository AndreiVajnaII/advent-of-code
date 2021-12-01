import { createRange, groupLines, intersect, single, toNum } from "../utils";

export function solve(lines: string[]) {
    const [ticketRulesGroup, myTicketGroup, nearbyTicketGroup] = groupLines(lines);
    const ticketRules = ticketRulesGroup.map(parseTicketRule);
    const nearbyTickets = nearbyTicketGroup.slice(1).map(parseTicket);
    const myTicket = parseTicket(myTicketGroup[1]);

    let errorRate = 0;
    const fieldMapping = new Array<Set<number>>(ticketRules.length);
    for (let i = 0; i < fieldMapping.length; i++) {
        fieldMapping[i] = new Set(createRange(0, ticketRules.length));
    }
    for (const ticket of nearbyTickets) {
        let isValidTicket = true;
        const potentialMapping = new Array<Set<number>>(ticketRules.length);
        for (let i = 0; i < fieldMapping.length; i++) {
            potentialMapping[i] = new Set();
        }
        for (let fieldIndex = 0; fieldIndex < ticket.length; fieldIndex++) {
            let isValidField = false;
            for (let ruleIndex = 0; ruleIndex < ticketRules.length; ruleIndex++) {
                if (ticketRules[ruleIndex].validate(ticket[fieldIndex])) {
                    potentialMapping[fieldIndex].add(ruleIndex);
                    isValidField = true;
                }
            }
            if (!isValidField) {
                isValidTicket = false;
                errorRate += ticket[fieldIndex];
            }
        }
        if (isValidTicket) {
            for (let i = 0; i < fieldMapping.length; i++) {
                fieldMapping[i] = intersect(fieldMapping[i], potentialMapping[i]);
            }
        }
    }
    const assignedFields = new Array<number>();
    for (const mapping of fieldMapping) {
        if (mapping.size === 1) {
            assignedFields.push(single(mapping));
        }
    }
    while(assignedFields.length > 0) {
        const assignedField = assignedFields.shift()!;
        for (const mapping of fieldMapping) {
            if (mapping.size > 1) {
                mapping.delete(assignedField);
                if (mapping.size === 1) {
                    assignedFields.push(single(mapping));
                }
            }
        }
    }
    let myTicketValue = 1;
    for (let fieldIndex = 0; fieldIndex < myTicket.length; fieldIndex++) {
        if (ticketRules[single(fieldMapping[fieldIndex])].name.startsWith("departure")) {
            myTicketValue *= myTicket[fieldIndex];
        }
    }
    return [errorRate, myTicketValue];
}

function parseTicketRule(line: string) {
    const [name, ranges] = line.split(": ");
    return new TicketRule(
        name,
        ranges.split(" or ").map(range => range.split("-").map(toNum)) as [FieldRange, FieldRange]);
}

function parseTicket(line: string) {
    return line.split(",").map(toNum);
}

type FieldRange = [start: number, end: number];

class TicketRule {
    constructor(
        public name: string,
        public range: [FieldRange, FieldRange]
    ) { }

    public validate(value: number) {
        return this.range.some(([start, end]) => start <= value && value <= end);
    }
}
