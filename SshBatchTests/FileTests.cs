using Moq;
using NUnit.Framework;
using SshBatch;
using System.Collections.Generic;

namespace SshBatchTests
{
    public partial class Tests
    {
        SshBatchProcessor p;
        Mock<IFileReader> file;
        Mock<ISsh> ssh;
        readonly List<string> sshLog = new List<string>();

        [SetUp]
        public void Setup()
        {
            file = new Mock<IFileReader>();
            ssh = new Mock<ISsh>(MockBehavior.Strict);
            p = new SshBatchProcessor(file.Object, ssh.Object);
        }

        string PrepareFile(string filename, params string[] lines)
        {
            file.Setup(f => f.Exists(filename))
                .Returns(true);
            file.Setup(f => f.ReadAllLines(filename))
                .Returns(lines);
            return filename;
        }



        [Test]
        public void ArgumentAsksHelp_ReturnHelp()
        {
            var result = p.ProcessParams("-h");
            Assert.That(result.Contains("How to use:"));
        }

        [Test]
        public void NoArguments_ReturnError()
        {
            var result = p.ProcessParams();
            Assert.That(result.Contains("Error: batch filename was not in parameters."));
        }

        [Test]
        public void ArgumentFileDoesNotExists_ReturnError()
        {
            var result = p.ProcessParams("FileDoesNotExists.txt");
            Assert.That(result.Contains("Error: batch file"));
            Assert.That(result.Contains("does not exist.")); 
        }

        [Test]
        public void FileEmpty_ReturnError()
        {
            string filename = PrepareFile("fileEmpty.txt");
            var result = p.ProcessParams(filename);
            Assert.That(result.Contains("Error: batcth file was empty."));
        }

        [Test]
        public void IncompleteFirstLine_ReturnError()
        {
            string filename = PrepareFile("incompleteFirstLine.txt", "host port user");
            var result = p.ProcessParams(filename);
            Assert.That(result.Contains("Error: batch file with invalid arguments."));
        }



    }
}