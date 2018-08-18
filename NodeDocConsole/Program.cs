using System.IO;
using NodeDocLib;

namespace NodeDocConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string path;
            if (args.Length != 1)
                path = Directory.GetCurrentDirectory();
            else
                path = args[0];

            var project = new Project(Path.GetFullPath(path));
            project.Export(Path.Combine(path, "out.tex"));
        }
    }
}
