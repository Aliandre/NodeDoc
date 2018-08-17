using System.IO;
using Newtonsoft.Json;

namespace NodeDocLib
{
    public static class NodeDefinitionLoader
    {
        public static Node LoadDocument(string path)
        {
            Node root;
            using (var reader = new StreamReader(path))
            {
                root = JsonConvert.DeserializeObject<Node>(reader.ReadToEnd());
            }
            return root;
        }
    }
}
