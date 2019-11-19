using System;
using System.Linq;
using System.Text;

namespace SshBatch
{
    public class SshBatchProcessor
    {
        public SshBatchProcessor(IFileReader reader = null, ISsh sshComp = null)
        {
            fileReader = reader ?? new FileReader();
            ssh = sshComp ?? new Ssh();
        }
        
        private readonly IFileReader fileReader;
        private readonly ISsh ssh;

        public string ProcessParams(params string[] args)
        {
            string[] lines;
            string[] configLine;
            string result = "";

            if (args.Length == 0)
                return HelpText("Error: batch filename was not in parameters.");
            else if ((new string[] { "-h", "--h", "-help", "help" }).Contains(args[0].ToLower()))
                return HelpText();
            else if (!fileReader.Exists(args[0]))
                return HelpText(string.Format("Error: batch file {0} does not exist.", args[0]));
            else 
            {
                lines = fileReader.ReadAllLines(args[0]);
                if (lines.Length < 1)
                    return HelpText("Error: batcth file was empty.");
                configLine = lines[0].Split(' ');
                if (configLine.Length < 4)
                    return HelpText("Error: batch file with invalid arguments.");
            }
            ssh.Setup(configLine[0], configLine[1], configLine[2], configLine[3]);
            ssh.Connect();
            for (int i = 1; i < lines.Length; i++)
                result += ssh.Command(lines[i]);
            ssh.Disconnect();

            return result;
        }


        private static string HelpText(string text = null)
        {
            var result = new StringBuilder();
            result.AppendLine(text)
            .AppendLine("How to use: provide a parameter especifing the file containing instructions.")
            .AppendLine("File formart:")
            .AppendLine("\tFirst line shoud be: \"server port user password [timeout]\"")
            .AppendLine("\t    custom timeout is optional")
            .AppendLine("\tEach following line after the first should be:")
            .AppendLine("\t    A terminal command or a comment starting with # or an empty line");
            return result.ToString();
        }
    }
}
