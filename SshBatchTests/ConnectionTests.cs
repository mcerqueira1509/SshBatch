using Moq;
using NUnit.Framework;
using SshBatch;

namespace SshBatchTests
{
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
        public void OneCommandInFile_SshIsExecuted()
        {
            string filename = PrepareFile("OneCommand.txt", "host port user password", "ls");
            PrepareSsh();
            var result = p.ProcessParams(filename);

            Assert.AreEqual(1, sshLog.Count);
            Assert.AreEqual("ls", sshLog[0]);
            Assert.AreEqual("Return of cmd:ls", result);
        }

        [Test]
        public void SpacesInConfigLine_SshIsExecuted()
        {
            string filename = PrepareFile("file.txt", "  host  port     user  password", "ls");
            string host = null, port = null, user = null, password = null;
            PrepareSsh();
            ssh.Setup(s => s.Setup(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()))
                .Callback<string, string, string, string, int?>(
                (h, p, u, pw, dt) =>
                {
                    host = h;
                    port = p;
                    user = u;
                    password = pw;
                });

            var result = p.ProcessParams(filename);

            Assert.AreEqual("host", host);
            Assert.AreEqual("port", port);
            Assert.AreEqual("user", user);
            Assert.AreEqual("password", password);

            foreach (var item in sshLog)
                System.Console.WriteLine(item);
            Assert.AreEqual("ls", sshLog[0]);
            Assert.AreEqual("Return of cmd:ls", result);
        }

        [Test]
        public void TabsInConfigLine_SshIsExecuted()
        {
            string filename = PrepareFile("file.txt", "   host     port       user    password", "ls");
            string host = null, port = null, user = null, password = null;
            PrepareSsh();
            ssh.Setup(s => s.Setup(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()))
                .Callback<string, string, string, string, int?>(
                (h, p, u, pw, dt) =>
                {
                    host = h;
                    port = p;
                    user = u;
                    password = pw;
                });

            var result = p.ProcessParams(filename);

            Assert.AreEqual("host", host);
            Assert.AreEqual("port", port);
            Assert.AreEqual("user", user);
            Assert.AreEqual("password", password);
            Assert.AreEqual("Return of cmd:ls", result);
        }



    }
}
