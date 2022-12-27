namespace SimpleFTP.Tests;

using SimpleFTP;

public class SimpleFTPTests
{
    private const string host = "127.0.0.1";
    private const int port = 8888;

    private FTPClient client = new(host, port);
    private FTPClient client2 = new(host, port);
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
        Assert.That($"2 {path}\\File1.txt false {path}\\Dir2 true" == result);
    }

    [Test]
    public async Task GetCommandTest()
    {
        var path = "../../../Dir1/File1.txt";
        var downloadedFileBytes = await client.Get(path);
        var sourceFileBytes = File.ReadAllBytes(path);
        Assert.That(sourceFileBytes.SequenceEqual(downloadedFileBytes));
    }

    [Test]
    public void NonExistentDirectoryForListTest()
    {
        var path = "../../../Dir3";
        Assert.ThrowsAsync<DirectoryNotFoundException>(async () => await client.List(path));
    }

    [Test]
    public async Task TwoClientsExecuteListRequestTest()
    {
        var path = "../../../Dir1";

        var task1 = client.List(path);
        var task2 = client2.List(path);

        var result1 = await task1;
        var result2 = await task2;

        Assert.That(result1 == result2);
    }

    [Test]
    public async Task TwoClientsExecuteGetRequestTest()
    {
        var path = "../../../Dir1/File1.txt";

        var task1 = client.Get(path);
        var task2 = client2.Get(path);

        var downloadedFileInfoFromFirstClient = await task1;
        var downloadedFileInfoFromSecondClient = await task2;

        Assert.That(downloadedFileInfoFromFirstClient.SequenceEqual(downloadedFileInfoFromSecondClient));
    }
}