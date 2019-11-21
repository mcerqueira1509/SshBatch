# SshBatch
A simple program to read instructions from a batch file in the task to connect to a ssh server and a series 
of commands to run. The output (as well as errors that occur) will be displayed to the console and then it 
can be redirected to a log file.
At the time, there was necessity of this automatized tool, and after fail attempts to use other ssh clients
(plink.exe -ssh host -m C:\path\commands.txt) I programmed this execution using Renci Ssh Component.
Its working but I am still making some improviments from time to time.