import { boxed, max } from "../utils";

export function solve(lines: string[]) {
    return boxed(lines.map(parseBoardingPass).map(decodeSeat))
        .reduce(seats => [seats.reduce(max), findMySeat(seats)]);
}

const parseBoardingPass = (line: string) => line.split("") as BoardingPassString;

function findMySeat(seats: number[]) {
    const sortedSeats = seats.sort();
    for (let i = 1; i < sortedSeats.length; i++) {
        if (sortedSeats[i] - sortedSeats[i-1] === 2) {
            return sortedSeats[i] - 1;
        }
    }
}

function decodeSeat(s: BoardingPassString): number {
    return s.reduce((result, c) => 2 * result + charValues[c], 0);
}

const charValues = {
    F: 0,
    B: 1,
    R: 1,
    L: 0,
};

type BoardingPassString = Array<keyof typeof charValues>;
