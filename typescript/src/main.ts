import { measure, padDay, run } from "./helpers";

const year = "2020";
const day = "25";

void import(`./${year}/${padDay(day)}`).then(
    (m: AocSolutionModule) => run(year, day, process.argv[2], measure(m.solve)),
    console.error);
interface AocSolutionModule {
    solve: (lines: string[], fun?: boolean) => unknown;
}

