public class Solver202110 : ISolver
{
    private Dictionary<char, int> syntaxScores = new()
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137,
    };

    private Dictionary<char, long> autocompleteScores = new()
    {
        [')'] = 1,
        [']'] = 2,
        ['}'] = 3,
        ['>'] = 4,
    };

    private Dictionary<char, char> pairs = new()
    {
        ['('] = ')',
        ['['] = ']',
        ['{'] = '}',
        ['<'] = '>',
        [')'] = '(',
        [']'] = '[',
        ['}'] = '{',
        ['>'] = '<',
    };

    public dynamic Solve(string[] lines)
    {
        var syntaxResults = lines.Select(CheckSyntax);
        return (syntaxResults.Where(r => !r.IsOK())
                .Select(r => r.Result)
                .Select(SyntaxScore)
                .Sum(),
            TotalAutocompleteScore(syntaxResults.Where(r => r.IsOK())
                .Select(r => r.Stack)
                .Select(CompleteInstruction)));
    }

    private SyntaxResult CheckSyntax(string line)
    {
        var stack = new Stack<char>();
        foreach (char c in line)
        {
            if (IsClosing(c))
            {
                if (stack.Count == 0 || !IsMatching(stack.Pop(), c))
                {
                    return new SyntaxResult(c, stack);
                }
            }
            else
            {
                stack.Push(c);
            }
        }
        return new SyntaxResult(Char.MinValue, stack);
    }

    private IEnumerable<char> CompleteInstruction(Stack<char> stack)
    {
        while(stack.Count > 0)
        {
            yield return pairs[stack.Pop()];
        }
    }

    private bool IsClosing(char c)
    {
        return syntaxScores.Keys.Contains(c);
    }

    private bool IsMatching(char opening, char closing)
    {
        return pairs[opening] == closing;
    }

    private int SyntaxScore(char c)
    {
        return syntaxScores[c];
    }
    
    private long TotalAutocompleteScore(IEnumerable<IEnumerable<char>> completionStrings)
    {
        var scores = completionStrings.Select(AutocompleteScore).ToArray();
        Array.Sort(scores);
        return scores[scores.Length / 2];
    }

    private long AutocompleteScore(IEnumerable<char> completionString)
    {
        return completionString.Select(c => autocompleteScores[c])
            .Aggregate(0L, (score, v) => score * 5L + v);
    }
}

struct SyntaxResult
{
    public char Result { get; private set; }
    public Stack<char> Stack;

    public SyntaxResult(char result, Stack<char> stack)
    {
        Result = result;
        Stack = stack;
    }

    public bool IsOK()
    {
        return Result == Char.MinValue;
    }
}