import readline from "readline-sync";
import { AsciiIO, IntcodeProcessor, IO } from "./intcode";

export function solve(lines: string[]) {
    const droid = new Droid(lines[0].split(",").map(s => +s));
    droid.takeAllItems();
    droid.goto("Security Checkpoint");
    droid.passSecurity();
    droid.switchToManual();
}

const untakeableItems = [
    "molten lava",
    "escape pod",
    "infinite loop",
    "photons",
    "giant electromagnet",
];

type Direction = "north" | "south" | "east" | "west";

function oppositeDirectionOf(direction: Direction) {
    return {
        east: "west",
        north: "south",
        south: "north",
        west: "east",
    }[direction];
}

class Droid {
    private io = new AsciiIO();
    private proc: IntcodeProcessor;
    private lastRoom: Room | undefined;

    constructor(program: number[]) {
        this.proc = new IntcodeProcessor(program, this.io);
    }

    public takeAllItems(comesFrom?: string) {
        const room = new Room(this.executeCommands());
        console.log(room.name);
        if (room.name === "== Security Checkpoint ==") {
            return ;
        }
        room.items.filter(item => !untakeableItems.includes(item)).forEach(item => {
            console.log("taking item" + item);
            this.takeItem(item);
            this.executeCommands();
        });
        room.doors.filter(door => door !== comesFrom).forEach(direction => {
            this.move(direction);
            const oppositeDirection = oppositeDirectionOf(direction as Direction);
            this.takeAllItems(oppositeDirection);
            this.move(oppositeDirection);
            this.lastRoom = new Room(this.executeCommands());
        });
    }

    public goto(roomName: string, comesFrom?: string) {
        const room = this.lastRoom ?? new Room(this.executeCommands());
        console.log(room.name);
        this.lastRoom = undefined;
        if (room.name !== `== ${roomName} ==`) {
            for (const direction of room.doors.filter(door => door !== comesFrom)) {
                this.move(direction);
                const oppositeDirection = oppositeDirectionOf(direction as Direction);
                if (!this.goto(roomName, oppositeDirection)) {
                    this.move(oppositeDirection);
                    this.executeCommands();
                } else {
                    return true;
                }
            }
        } else {
            return true;
        }
    }

    public passSecurity(index = 0) {
        this.move("south");
        const result = this.executeCommands();
        if (!result.includes("Alert!")) {
            console.log(result);
            return true;
        }
        const items = this.getInventory();
        if (index < items.length) {
            this.dropItem(items[index]);
            if (this.passSecurity(index)) {
                return true;
            }
            this.takeItem(items[index]);
            if (this.passSecurity(index + 1)) {
                return true;
            }
        }
    }

    public switchToManual() {
        this.proc.io = new ConsoleIO();
        this.proc.run();
    }

    private executeCommands() {
        this.proc.run();
        return this.io.readString();
    }

    private takeItem(item: string) {
        this.io.writeInstruction("take " + item);
    }

    private dropItem(item: string) {
        this.io.writeInstruction("drop " + item);
    }

    private move(direction: string) {
        this.io.writeInstruction(direction);
    }

    private getInventory() {
        this.io.writeInstruction("inv");
        return getList(parseDescription(this.executeCommands()), "Items in your inventory:");
    }
}

class Room {
    public name: string;
    public doors: string [];
    public items: string[];

    private description: string[];

    constructor(description: string) {
        this.description = parseDescription(description);
        this.name = this.description[0];
        this.doors = getList(this.description, "Doors here lead:");
        this.items = getList(this.description, "Items here:");
    }

}

function parseDescription(description: string) {
    return description.split("\n").map(s => s.trim()).filter(s => s !== "");
}

function getList(description: string[], header: string) {
    const result: string[] = [];
    let index = description.indexOf(header);
    if (index >= 0) {
        index++;
        while (index < description.length && description[index].startsWith("-")) {
            result.push(description[index].substring(2));
            index++;
        }
    }
    return result;
}

class ConsoleIO implements IO {
    private output: number[] = [];
    private input: number[] = [];

    public write(v: number) {
        if (String.fromCharCode(v) === "\n") {
            console.log(String.fromCharCode(...this.output));
            this.output = [];
        } else {
            this.output.push(v);
        }
    }

    public read(): number {
        if (this.input.length === 0) {
            this.input = (readline.prompt() + "\n").split("").map(s => s.charCodeAt(0));
        }
        return this.input.shift()!;
    }
}
