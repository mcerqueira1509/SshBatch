using System.IO;

namespace SshBatch
{
    public interface IFileReader
    {
        bool Exists(string path);
        string[] ReadAllLines(string path);
    }

    public class FileReader : IFileReader
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }
    }
}
