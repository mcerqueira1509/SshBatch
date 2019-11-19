using Moq;
using NUnit.Framework;
using SshBatch;

namespace SshBatchTests
{
    public interface IString
    {
        int Func(int x, int y);
    }
    public partial class Tests
    {
        void PrepareSsh()
        {
            ssh.Setup(s => s.Setup(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>() ));
            ssh.Setup(s => s.Connect());
            ssh.Setup(s => s.Disconnect());

            ssh.Setup(s => s.Command(It.IsAny<string>()))
                .Callback<string>((strCmd) => sshLog.Add(strCmd))
                .Returns<string>(strCmd => "Return of cmd:" + strCmd);
        }

        [Test]
        public void NoCommands_ConnectAndDisconnect()
        {
            string filename = PrepareFile("JustFirstLine.txt", "host port user pasword");
            PrepareSsh();
            var result = p.ProcessParams(filename);
            ssh.Verify((m => m.Connect()), Times.Once);
            ssh.Verify((m => m.Disconnect()), Times.Once);
        }

        [Test]
        public void OneCommandInFile_IsSshExecuted()
        {
            string filename = PrepareFile("OneCommand.txt", "host port user pasword", "ls");
            PrepareSsh();
            var result = p.ProcessParams(filename);

            Assert.AreEqual(1, sshLog.Count);
            Assert.AreEqual("ls", sshLog[0]);
            Assert.AreEqual("Return of cmd:ls", result);
        }



    }
}
