import { performance } from "perf_hooks";
import * as fs from "fs";
import * as path from "path";
import * as util from "util";
import request from "request";

const req = util.promisify<string, request.CoreOptions, request.Response>(request);

export function run(year: string, day: string, cookie: string, solve: (lines: string[]) => unknown) {
    return getInput(year, day, cookie)
        .then(input => {
            const lines = input.split("\n");
            lines.pop();
            const result = solve(lines);
            if (result instanceof Promise) {
                result.then(console.log, console.error);
            } else {
                console.log(result);
            }
        }, console.error);
}

export function measure<Args extends any[], Return>(f: (...args: Args) => Return) {
    return (...args: Args) => {
        const start = performance.now();
        const result = f(...args);
        const end = performance.now();
        console.log(`Took ${(end - start).toLocaleString()} ms.`);
        return result;
    };
}

/*
    For solutions where it's fun to display other things than just the results,
    call this method to activate that.
*/
export function haveFun(solve: (lines: string[], fun: boolean) => unknown) {
    return (lines: string[]) => solve(lines, true);
}

export function padDay(day: string) {
    return day.length < 2 ? `0${day}` : day;
}

async function getInput(year: string, day: string, cookie: string) {
    if (!inputExists(year, day)) {
        const response = await requestInput(`https://adventofcode.com/${year}/day/${day}/input`, cookie);
        if (response.statusCode !== 200) {
            throw new RequestError(response.statusCode, response.statusMessage, response.body);
        } else {
            saveInput(year, day, response.body).catch(console.error);
            return response.body as string;
        }
    } else {
        return (await readInput(year, day)).toString();
    }
}

function requestInput(inputUrl: string, cookie: string) {
    return req(inputUrl, { headers: { cookie } });
}

async function saveInput(year: string, day: string, input: string) {
    const filepath = inputPath(year, day);
    await fs.promises.mkdir(path.dirname(filepath), { recursive: true });
    return fs.promises.writeFile(filepath, input);
}

function readInput(year: string, day: string) {
    return fs.promises.readFile(inputPath(year, day));
}

function inputExists(year: string, day: string) {
    return fs.existsSync(inputPath(year, day));
}

function inputPath(year: string, day: string) {
    return `./src/${year}/inputs/${padDay(day)}.txt`;
}
class RequestError extends Error {
    constructor(
        public statusCode: number,
        public statusMessage: string,
        public body: string,
    ) {
        super(`${statusCode} ${statusMessage}\n${body}`);
    }
}
