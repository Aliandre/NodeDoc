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

        public void Export()
        {
            using (var writer = new StreamWriter("out.tex"))
            {
                foreach(var node in Root.Children)
                {
                    writer.WriteLine(@"%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                    writer.WriteLine(@"%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                    writer.WriteLine(@"\chapter{" + node.Name + "}");
                    writer.WriteLine(@"\label{" + node.Name + "}");

                    foreach (var entry in node.Entries)
                    {
                        writer.WriteLine(@"%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%");
                        writer.WriteLine(@"\section{" + entry.Name + "}");
                        writer.WriteLine(@"\label{" + entry.Name + "}");

                        if (entry.Tags.Count > 0)
                        {
                            writer.Write(@"Linked categories: ");
                            foreach(var tag in entry.Tags)
                            {
                                writer.Write("\"" + tag + "\" (see \\ref{" + tag + "} page \\pageref{" + tag + "}) ");
                            }
                        }

                        foreach(var subEntry in entry.SubEntries)
                        {
                            writer.WriteLine(@"%%%%");
                            writer.WriteLine(@"\subsection{" + subEntry.Name + "}");
                            writer.WriteLine(@"\label{" + subEntry.Name + "}");

                            if (subEntry.Tags.Count > 0)
                            {
                                writer.Write(@"Linked categories: ");
                                foreach (var tag in subEntry.Tags)
                                {
                                    writer.Write("\"" + tag + "\" (see \\ref{" + tag + "} page \\pageref{" + tag + "}) ");
                                }
                            }
                        }
                        
                        writer.WriteLine(entry.Content);
                    }
                }
            }
        }
    }
}
