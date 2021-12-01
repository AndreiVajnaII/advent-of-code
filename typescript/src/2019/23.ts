import { InMemoryIO, IntcodeProcessor } from "./intcode";

export function solve(lines: string[]) {
    const network = new Network(lines[0].split(",").map(s => +s), 50);
    return network.start();
}

class Network {
    private computers: Computer[] = [];

    constructor(program: number[], n: number) {
        for (let i = 0; i < n; i++) {
            this.computers.push(new Computer([...program], i));
        }
    }

    public start() {
        const packets: Packet[] = [];
        let currentAddress = 0;
        let currentPacket: Packet | undefined;
        let natx = 0;
        let naty = 0;
        let lasty = 0;
        let idle = 0;
        while (true) {
            packets.push(...this.computers[currentAddress].run());
            currentPacket = packets.shift();
            if (currentPacket) {
                idle = 0;
                if (currentPacket.address !== 255) {
                    currentAddress = currentPacket.address;
                    this.computers[currentAddress].send(currentPacket);
                } else {
                    natx = currentPacket.x;
                    naty = currentPacket.y;
                }
            } else {
                idle++;
                this.computers[currentAddress].noPacket();
                currentAddress = (currentAddress + 1) % this.computers.length;
                if (idle === 100) {
                    if (lasty === naty) {
                        return naty;
                    }
                    currentAddress = 0;
                    lasty = naty;
                    this.computers[currentAddress].send(new Packet(0, natx, naty));
                }
            }
        }
        return currentPacket?.y;
    }
}

class Computer {
    private io: InMemoryIO;
    private proc: IntcodeProcessor;

    constructor(program: number[], address: number) {
        this.io = new InMemoryIO();
        this.proc = new IntcodeProcessor(program, this.io);
        this.io.input.push(address);
        this.proc.run();
    }

    public run() {
        this.proc.run();
        const packets: Packet[] = [];
        while (this.io.output.length > 0) {
            packets.push(new Packet(this.io.shift()!, this.io.shift()!, this.io.shift()!));
        }
        return packets;
    }

    public send(packet: Packet) {
        this.io.input.push(packet.x, packet.y);
    }

    public noPacket() {
        this.io.input.push(-1);
    }
}

class Packet {
    constructor(public address: number, public x: number, public y: number) { }
}
