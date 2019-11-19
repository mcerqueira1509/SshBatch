using Renci.SshNet;
using System;
using System.IO;
using System.Threading;

namespace SshBatch
{
    public interface ISsh
    {
        string Command(string command);
        void Setup(string host, string port, string username, string password, int? timeoutMilis = null);
        void Connect();
        void Disconnect();
    }

    public class Ssh : ISsh, IDisposable
    {
        SshClient client;
        const int defaultPort = 22;
        const int secondInMilis = 1000;
        int timeoutMiliseconds = 15 * secondInMilis;

        public string Command(string command) => ExecAndWait(command);

        private string ExecAndWait(string textCmd, int? nWaitTime = null)
        {
            if (string.IsNullOrWhiteSpace(textCmd) || (textCmd[0] == '#'))
                return null;
            int waitTime = (nWaitTime is null) ? timeoutMiliseconds : (int)nWaitTime;
            var sshCmd = client.CreateCommand(textCmd);
            var asynch = sshCmd.BeginExecute();
            int elapsedTime = 0;
            while ((!asynch.IsCompleted) && (elapsedTime < waitTime))
            {
                Thread.Sleep(secondInMilis);
                elapsedTime += secondInMilis;
            }
            if (CheckTimeout(waitTime, elapsedTime))
                return string.Format("(Timeout Experied: Waited {0} seconds for command: {1} )", waitTime / secondInMilis, textCmd);
            var strmReader = new StreamReader(sshCmd.ExtendedOutputStream);
            return strmReader.ReadToEnd() + sshCmd.EndExecute(asynch);
        }

        private static bool CheckTimeout(int waitTime, int elapsedTime)
        {
            return (elapsedTime >= waitTime) && (waitTime > 0);
        }

        public void Setup(string host, string port, string username, string password, int? timeoutMilis)
        {
            if (!(client is null))
                throw new InvalidOperationException("Client for the connection is already set.");
            client = new SshClient(host, GetPort(port), username, password);
            if (timeoutMilis is int t)
                timeoutMiliseconds = t;
        }

        private static int GetPort(string textPort)
        {
            if (int.TryParse(textPort, out int port))
                return port;
            return defaultPort;
        }

        public void Connect()
        {
            client.Connect();
        }

        public void Disconnect()
        {
            client.Disconnect();
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
