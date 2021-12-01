import { intersect, single } from "../utils";

export function solve(lines: string[]) {
    const allergenMap = new Map<string, Set<string>>();
    const allIngredients: string[] = [];
    for (const line of lines) {
        const [allergens, ingredients] = parseLines(line);
        allIngredients.push(...ingredients);
        for (const allergen of allergens) {
            allergenMap.set(allergen, allergenMap.has(allergen)
                ? intersect(allergenMap.get(allergen)!, ingredients)
                : ingredients);
        }
    }

    const assignedIngredients = new Map<string, string>();
    const assignIngredients = () => {
        for (const [allergen, ingredients] of allergenMap) {
            if (ingredients.size === 1) {
                assignedIngredients.set(single(ingredients), allergen);
            }
        }
    };

    assignIngredients();
    let done = false;
    while (!done) {
        done = true;
        for (const ingredients of allergenMap.values()) {
            if (ingredients.size > 1) {
                for (const ingredient of ingredients) {
                    if (assignedIngredients.has(ingredient)) {
                        ingredients.delete(ingredient);
                        done = false;
                    }
                }
            }
        }
        assignIngredients();
    }
    return [
        allIngredients.filter(ingredient => !assignedIngredients.has(ingredient)).length,
        Array.from(assignedIngredients.keys())
            .sort((a, b) => assignedIngredients.get(a)!.localeCompare(assignedIngredients.get(b)!))
            .join(),
    ];
}

function parseLines(line: string) {
    const m = /(.+) \(contains (.+)\)/.exec(line)!;
    return [
        m[2].split(", "),
        new Set(m[1].split(" ")),
    ] as const;
}
