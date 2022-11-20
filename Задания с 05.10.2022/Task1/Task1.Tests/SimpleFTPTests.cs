namespace SimpleFTP.Tests;

using SimpleFTP;

public class SimpleFTPTests
{
    private const string host = "127.0.0.1";
    private const int port = 8888;

    private FTPClient client = new(host, port);
    private FTPServer server = new(host, port);

    [OneTimeSetUp]
    public void Setup()
    {
        server.Start();
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        server.Stop();
    }

    [Test]
    public async Task ListCommandTest()
    {
        var path = "../../../Dir1";
        var result = await client.List(path);
        Assert.AreEqual($"2 {path}\\File1.txt false {path}\\Dir2 true", result);
    }

    [Test]
    public async Task GetCommandTest()
    {
        var path = "../../../Dir1/File1.txt";
        var downloadedFileInfo = await client.Get(path);

        var sourceFileBytes = File.ReadAllBytes(path);
        var sourceFileInfo = sourceFileBytes.Length.ToString();
        for (int i = 0; i < sourceFileBytes.Length; i++)
        {
            sourceFileInfo += " " + sourceFileBytes[i].ToString();
        }

        Assert.AreEqual(sourceFileInfo, downloadedFileInfo);
    }

    [Test]
    public void NonExistentDirectoryForListTest()
    {
        var path = "../../../Dir3";
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.List(path));
    }

    [Test]
    public void NonExistentFileForGetTest()
    {
        var path = "../../../File3.txt";
        Assert.ThrowsAsync<FileNotFoundException>(async () => await client.Get(path));
    }
}