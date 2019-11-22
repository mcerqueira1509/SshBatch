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
        private string errorMessage;
        private string[] lines;
        private string[] configLine;

        public string ProcessParams(params string[] args)
        {
            if (!CheckInitialConfiguration(args))
                return HelpText(errorMessage);

            ssh.Setup(configLine[0], configLine[1], configLine[2], configLine[3]);
            ssh.Connect();
            string result = "";
            for (int i = 1; i < lines.Length; i++)
                result += ssh.Command(lines[i]);
            ssh.Disconnect();
            return result;
        }

        private bool CheckInitialConfiguration(string[] args)
        {
            if (args.Length == 0)
                return SetErroMessage("Error: batch filename was not in parameters.");
            else if (IsAskingHelp(args))
                return SetErroMessage("");
            else if (!fileReader.Exists(args[0]))
                return SetErroMessage(string.Format("Error: batch file {0} does not exist.", args[0]));
            else
            {
                lines = fileReader.ReadAllLines(args[0]);
                if (lines.Length < 1)
                    return SetErroMessage("Error: batcth file was empty.");
                configLine = lines[0].Split(' ').AsQueryable().Where(a => !string.IsNullOrEmpty(a)).ToArray();
                if (configLine.Length < 4)
                    return SetErroMessage("Error: batch file with invalid arguments.");
            }
            return true;
        }

        private static bool IsAskingHelp(string[] args)
        {
            return (new string[] { "-h", "--h", "-help", "help" }).Contains(args[0].ToLower());
        }

        private bool SetErroMessage(string text)
        {
            errorMessage = text;
            return false;
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
