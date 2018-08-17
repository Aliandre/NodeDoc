using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace NodeDocLib
{
    [DebuggerDisplay("{Name} ({MainCategory})")]
    public class Entry
    {
        public string FilePath { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Content { get; set; }
        public string MainCategory { get; set; }
        public Node LinkedCategory { get; set; }
        public Entry LinkedEntry { get; set; }
        public List<Entry> SubEntries { get; set; }

        public Entry(string path)
        {
            Tags = new List<string>();
            SubEntries = new List<Entry>();
            Name = Path.GetFileNameWithoutExtension(path);

            using (var reader = new StreamReader(path))
            {
                var header = reader.ReadLine();
                if (header.Contains(","))
                {
                    MainCategory = header.Split(',')[0];
                    Tags = header.Split(',').Skip(1).ToList();
                }
                else
                {
                    MainCategory = header;
                }

                Tags.Add(MainCategory);
                Content = reader.ReadToEnd();
            }
        }
    }
}
