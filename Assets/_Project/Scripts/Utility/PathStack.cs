using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace NJG.Utilities
{
    public enum PathType
    {
        Absolute,
        Relative
    }

    /// <summary>
    ///     Represent platform agnostic paths as an array of strings, avoiding buggery around trailing/leading slashes, etc.
    ///     PathStacks can be converted to/from string paths.
    /// </summary>
    public class PathStack
    {
        private readonly IEnumerable<string> _tokens;
        private readonly PathType _type;

        public PathStack(PathType type, IEnumerable<string> initialPath)
        {
            _type = type;
            _tokens = initialPath;
        }

        public PathStack(IEnumerable<string> initialPath)
        {
            _type = PathType.Relative;
            _tokens = initialPath;
        }

        public PathStack(PathType type, params string[] initialPath)
        {
            _type = type;
            _tokens = initialPath;
        }

        public PathStack(params string[] initialPath)
        {
            _type = PathType.Relative;
            _tokens = initialPath;
        }

        public int Length => _tokens.Count();

        public static PathStack FromPathString(string pathString, char? pathDelimeter = null)
        {
            pathDelimeter ??= Path.DirectorySeparatorChar;

            IEnumerable<string> tokens = pathString.Split(pathDelimeter.Value)
                                                   .Where(token => token != null && token.Trim().Length > 0);

            bool isAbsolute = pathString.Length > 0 && Regex.IsMatch(pathString, "^(" + pathDelimeter + "|\\w:)");

            PathType type = isAbsolute ? PathType.Absolute : PathType.Relative;

            return new PathStack(type, tokens);
        }

        public PathStack Push(params string[] newTokens) => new(_type, _tokens.Concat(newTokens));

        public PathStack Pop(int number = 1) => new(_type, _tokens.Reverse().Skip(number).Reverse());

        public string Peek() => _tokens.Last();

        public bool IsEmpty() => Length == 0;

        public override string ToString()
        {
            string result = "";
            if (_type == PathType.Absolute)
                result += Path.DirectorySeparatorChar;

            result += string.Join(Path.DirectorySeparatorChar.ToString(), _tokens.ToArray());
            return result;
        }

        public static PathStack operator +(PathStack self, string s) => self.Push(s);

        public static PathStack operator +(PathStack self, PathStack other)
        {
            if (other._type == PathType.Absolute)
                throw new InvalidOperationException(
                    "Cannot add absolute path to another path, you can only add ONTO Absolute Path Stacks");

            return new PathStack(self._type, self._tokens.Concat(other._tokens));
        }
    }
}