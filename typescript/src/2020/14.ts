import { replace } from "../utils";

export function solve(lines: string[]) {
    return [
        executeInstructions(lines, setMaskedValue),
        executeInstructions(lines, setFloatingAddresses),
    ];
}

function executeInstructions(lines: string[], updateMemory: UpdateMemoryFunc) {
    const memory = new Map<number, number>();
    let mask: BitMask = [];
    for (const line of lines) {
        const [dest, value] = line.split(" = ");
        if (dest === "mask") {
            mask = value.split("") as BitMask;
        } else {
            const target = +(/mem\[(\d+)\]/.exec(dest)![1]);
            updateMemory(memory, target, mask, +value);
        }
    }
    let result = 0;
    for (const v of memory.values()) {
        result += v;
    }
    return result;
}

function setMaskedValue(memory: Map<number, number>, address: number, mask: BitMask, value: number) {
    memory.set(address, applyMask(value, mask));
}

function setFloatingAddresses(memory: Map<number, number>, target: number, mask: BitMask, value: number) {
    generateAddresses(applyAddressMask(target, mask)).forEach(address => memory.set(address, value));
}

function applyMask(value: number, mask: BitMask) {
    const bitValue = pad(value.toString(2).split("") as BitValue);
    for (let i = 0; i < mask.length; i++) {
        switch (mask[i]) {
        case "1":
            bitValue[i] = "1";
            break;
        case "0":
            bitValue[i] = "0";
            break;
        }
    }
    return toBinary(bitValue);
}

function applyAddressMask(target: number, mask: BitMask) {
    const addressMask = pad(target.toString(2).split("")) as BitMask;
    for (let i = 0; i < mask.length; i++) {
        if (mask[i] !== "0") {
            addressMask[i] = mask[i];
        }
    }
    return addressMask;
}

function generateAddresses(addressMask: BitMask): number[] {
    const index = addressMask.findIndex(c => c === "X");
    return index === -1
        ? [parseInt(addressMask.join(""), 2)]
        : [
            ...generateAddresses(replace(addressMask, index, "0")),
            ...generateAddresses(replace(addressMask, index, "1")),
        ];
}

function pad<T extends string[]>(value: T) {
    while (value.length < 36) {
        value.unshift("0");
    }
    return value;
}

function toBinary(bitValue: BitValue) {
    return parseInt(bitValue.join(""), 2);
}

type BitMask = Array<"X" | "1" | "0">;
type BitValue = Array<"1" | "0">;

type UpdateMemoryFunc = (memory: Map<number, number>, target: number, mask: BitMask, value: number) => void;
