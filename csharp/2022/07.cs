using Aoc;

namespace Aoc2022;

public class Solver202207 : ISolver
{
    public dynamic Solve(string[] lines)
    {
        var visitor = new FileSystemVisitor();
        var orderedSizes = visitor.Visit(lines.Select(Parse))
            .OrderBy(node => node.Size)
            .Select(node => node.Size)
            .ToArray();
        var freeSpace = 70000000 - orderedSizes[^1];
        return (
            orderedSizes.TakeWhile(size => size < 100000).Sum(),
            orderedSizes.First(size => freeSpace + size >= 30000000)
        );
    }

    private static Action<IFileSystemCommandHandler> Parse(string line)
    {
        var terms = line.Split();
        return terms[0] switch
        {
            "$" => terms[1] switch
            {
                "cd" => handler => handler.ChangeDir(terms[2]),
                "ls" => handler => handler.List(),
                _ => throw new ArgumentException("Illegal command: " + line)
            },
            "dir" => handler => handler.DirInfo(terms[1]),
            _ => handler => handler.FileInfo(long.Parse(terms[0]))
        };
    }

    private interface IFileSystemCommandHandler
    {
        void ChangeDir(string dirName);
        void List();
        void DirInfo(string dirName);
        void FileInfo(long size);
    }

    private class FileSystemVisitor : IFileSystemCommandHandler
    {
        private readonly DirNode rootNode = new();
        private readonly Stack<DirNode> path = new();

        private DirNode CurrentDir => path.Peek();

        private DirNode? completedDir;
        
        public void ChangeDir(string dirName)
        {
            completedDir = null;
            switch (dirName)
            {
                case "/":
                    path.Clear();
                    path.Push(rootNode);
                    break;
                case "..":
                    ExitDir();
                    break;
                default:
                    path.Push(CurrentDir.GetDir(dirName));
                    break;
            }
        }

        public void List()
        {
            completedDir = null;
        }

        public void DirInfo(string dirName)
        {
            completedDir = null;
            CurrentDir.AddDir(dirName);
        }

        public void FileInfo(long size)
        {
            completedDir = null;
            CurrentDir.Size += size;
        }
        
        public IEnumerable<FileSystemNode> Visit(IEnumerable<Action<IFileSystemCommandHandler>> commands)
        {
            foreach (var command in commands)
            {
                command(this);
                if (completedDir is not null)
                {
                    yield return completedDir;
                }
            }
            if (completedDir is null)
            {
                while (path.Count > 1)
                {
                    ExitDir();
                    yield return completedDir!;
                }
            }
            yield return rootNode;
        }
        
        private void ExitDir()
        {
            completedDir = path.Pop();
            CurrentDir.Size += completedDir.Size;
        }

        private class DirNode : FileSystemNode
        {
            private readonly IDictionary<string, DirNode> directories = new Dictionary<string, DirNode>();

            public void AddDir(string dirName)
            {
                directories[dirName] = new DirNode();
            }

            public DirNode GetDir(string dirName) => directories[dirName];
        }
    }

    private class FileSystemNode
    {
        public long Size { get; set; }
    }
}