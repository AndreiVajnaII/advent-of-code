import { groupLines, objFromEntries } from "../utils";

export function solve(lines: string[]) {
    return groupLines(lines)
        .map(joinPassportLines)
        .map(parsePassportLine)
        .filter(isValid)
        .length;
}

const joinPassportLines = (group: string[]) => group.join(" ");

function parsePassportLine(passportLine: string): PotentialPassport {
    return objFromEntries(passportLine.split(" ")
        .map(field => field.split(":") as [RequiredPassportField, string]));
}

function isValid(passport: PotentialPassport) {
    return validators.every(([field, validator]) => isValidField(passport[field], validator));
}

function isValidField(fieldValue: string | undefined, validator: Validator) {
    return fieldValue && validator(fieldValue);
}

const validators = [
    ["byr", numberValidator(1920, 2002)] as const,
    ["iyr", numberValidator(2010, 2020)] as const,
    ["eyr", numberValidator(2020, 2030)] as const,
    ["hgt", heightValidator] as const,
    ["hcl", regexValidator(/^#[0-9a-f]{6}$/)] as const,
    ["ecl", regexValidator(/^amb|blu|brn|gry|grn|hzl|oth$/)] as const,
    ["pid", regexValidator(/^\d{9}$/)] as const,
];

type Validator = (s: string) => boolean;
type RequiredPassportField = typeof validators extends Array<readonly [infer R, Validator]> ? R: never;
type PotentialPassport = Partial<Record<RequiredPassportField, string>>;


function numberValidator(min: number, max: number) {
    return (s: string) => min <= (+s) && (+s) <= max;
}

const heightValidators = {
    cm: numberValidator(150, 193),
    in: numberValidator(59, 76),
};

function heightValidator(s: string) {
    const m = /(\d+)(cm|in)/.exec(s);
    return !!m && heightValidators[m[2] as "cm" | "in"](m[1]);
}

function regexValidator(r: RegExp) {
    return (s: string) => r.test(s);
}
