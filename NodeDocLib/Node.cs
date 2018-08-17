using System.Collections.Generic;
using System.Diagnostics;

namespace NodeDocLib
{
    [DebuggerDisplay("{ToDebugString}")]
    public class Node
    {
        public string Name { get; set; }
        public List<Node> Children { get; set; }
        public List<Entry> Entries { get; set; }

        public Node()
        {
            Children = new List<Node>();
            Entries = new List<Entry>();
        }

        public string ToDebugString
        {
            get {
                var str = Name;
                if (Children.Count > 0)
                {
                    str += $", {Children.Count} children";
                }
                if (Entries.Count > 0)
                {
                    str += $", {Entries.Count} entries";
                }
                return str;
            }
        }

        public IEnumerable<Node> SelfAndChildren()
        {
            yield return this;
            foreach(var child in Children)
            {
                foreach (var node in child.SelfAndChildren())
                    yield return node;
            }
        }
    }
}
