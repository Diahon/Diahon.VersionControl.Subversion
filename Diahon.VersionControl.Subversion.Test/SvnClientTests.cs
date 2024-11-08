﻿using Diahon.VersionControl.Subversion.Protocol;

namespace Diahon.VersionControl.Subversion.Test;

public sealed class SvnClientTests(ITestOutputHelper output)
{
    [Fact]
    public async Task ConnectAsync_ShouldAuthenticate()
    {
        using var client = await SvnClient.ConnectAsync(Env.RepoUrl, Env.Credentials);
    }

    [Fact]
    public async Task List_ShouldFindFiles()
    {
        using var client = await SvnClient.ConnectAsync(Env.RepoUrl, Env.Credentials);

        var result = client.List(path: null, "*.txt");

        Assert.True(result.Count > 0);
        Assert.Equal(".txt", Path.GetExtension(result[0].RelativePath.Value));
    }

    [Fact]
    public async Task GetFile_ShouldGetWholeFileContent()
    {
        using var client = await SvnClient.ConnectAsync(Env.RepoUrl, Env.Credentials);

        var result = client.GetFileString("file.txt");
        output.WriteLine(result);

        Assert.Equal("Hello World", result);
    }

    [Fact]
    public async Task ShouldGetFoundFileTwice()
    {
        using var client = await SvnClient.ConnectAsync(Env.RepoUrl, Env.Credentials);

        for (int i = 0; i < 2; i++)
        {
            var listResult = client.List(path: null, "*.txt");
            var content = client.GetFileString(listResult[0].RelativePath.Value);
            output.WriteLine(content);

            Assert.True(content.Length > 0);
        }
    }

    [Fact]
    public async Task GetFile_ShouldHandleException_WhenNotExists()
    {
        using var client = await SvnClient.ConnectAsync(Env.RepoUrl, Env.Credentials);

        Assert.Throws<SvnError>(() => client.GetFile("does_not_exist.txt"));
    }

    [Fact]
    public async Task GetFile_ShouldDownloadValidBinaryFile()
    {
        using var client = await SvnClient.ConnectAsync(Env.RepoUrl, Env.Credentials);

        var files = client.List(path: null, "*.bin");
        var data = client.GetFile(files[0].RelativePath.Value);

        Assert.Equal(SvnContainer.TestBinContent, data);
    }
}
