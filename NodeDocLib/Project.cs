using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NodeDocLib
{
    public class Project
    {
        public string Path { get; set; }
        public Node Root { get; set; }
        public List<Entry> Entries { get; set; }
        public List<Node> AllNodes { get; set; }

        public Project(string path)
        {
            Root = NodeDefinitionLoader.LoadDocument(System.IO.Path.Combine(path, "structure.json"));
            AllNodes = Root.SelfAndChildren().ToList();
            Entries = Directory.EnumerateFiles(System.IO.Path.Combine(path, "entries")).Select(s => new Entry(s)).ToList();

            foreach(var entry in Entries)
            {
                var foundNode = AllNodes.FirstOrDefault(n => n.Name == entry.MainCategory);
                if (foundNode != null)
                {
                    entry.LinkedCategory = foundNode;
                    foundNode.Entries.Add(entry);
                }
                else
                {
                    var foundEntry = Entries.FirstOrDefault(n => n.Name == entry.MainCategory);
                    if (foundEntry != null)
                    {
                        entry.LinkedEntry = foundEntry;
                        foundEntry.SubEntries.Add(entry);
                    }
                }
            }
        }

        public static string GetSectionLevel(string name, int level)
        {
            switch(level)
            {
                case 0:
                    return @"\chapter{" + name + "}";
                case 1:
                    return @"\section{" + name + "}";
                case 2:
                    return @"\subsection{" + name + "}";
                case 3:
                    return @"\paragraph{" + name + "}";
                case 4:
                    return @"\subparagraph{" + name + "}";
                default:
                    return "Document too deep (too many subsections) for " + name;
            }
        }

        public void Export(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                foreach(var node in Root.Children)
                {
                    DisplayNode(writer, node, 0);
                }
            }
        }

        private static void DisplayNode(StreamWriter writer, Node node, int depth)
        {
            writer.WriteLine(@"%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            writer.WriteLine(@"%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
            writer.WriteLine(GetSectionLevel(node.Name, depth));
            writer.WriteLine(@"\label{" + node.Name + "}");
            writer.WriteLine();

            foreach (var entry in node.Entries)
            {
                writer.WriteLine(@"%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                writer.WriteLine(GetSectionLevel(entry.Name, depth + 1));
                writer.WriteLine(@"\label{" + entry.Name + "}");

                if (entry.Tags.Count > 0)
                {
                    writer.Write(@"Linked categories: ");
                    foreach (var tag in entry.Tags)
                    {
                        writer.WriteLine("\"" + tag + "\" (see \\ref{" + tag + "} page \\pageref{" + tag + "}) ");
                    }
                    writer.WriteLine();
                }

                writer.WriteLine(entry.Content);
                writer.WriteLine();

                foreach (var subEntry in entry.SubEntries)
                {
                    writer.WriteLine(@"%%%%");
                    writer.WriteLine(GetSectionLevel(subEntry.Name, depth + 2));
                    writer.WriteLine(@"\label{" + subEntry.Name + "}");

                    if (subEntry.Tags.Count > 0)
                    {
                        writer.Write(@"Linked categories: ");
                        foreach (var tag in subEntry.Tags)
                        {
                            writer.Write("\"" + tag + "\" (see \\ref{" + tag + "} page \\pageref{" + tag + "}) ");
                        }
                    }

                    writer.WriteLine(subEntry.Content);
                    writer.WriteLine();
                }
            }

            foreach(var subNode in node.Children)
            {
                DisplayNode(writer, subNode, depth + 1);
            }

            writer.WriteLine();
            writer.WriteLine();
        }
    }
}
