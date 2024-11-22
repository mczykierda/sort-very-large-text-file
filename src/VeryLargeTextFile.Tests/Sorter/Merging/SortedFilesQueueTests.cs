using FluentAssertions;
using VeryLargeTextFile.Sorter.Merging;

namespace VeryLargeTextFile.Tests.Sorter.Merging;

public class SortedFilesQueueTests
{
    [Fact]
    public void WhenSimulatingMerge_ShouldHaveCorrectState()
    {
        var files = Enumerable.Range(0, 47).Select(x => new FileInfo(x.ToString())).ToList();

        var sut = new SortedFilesQueue(files, new MergeConfig(10));

        //first loop
        sut.HasFilesToMerge.Should().BeTrue();

        var chunk1 = sut.GetNextBatchOfFilesToMerge();

        chunk1.Should().HaveCount(10);
        chunk1.Should().ContainInOrder(files.Take(10));
        sut.HasFilesToMerge.Should().BeTrue();

        var m1 = new FileInfo("m1");
        sut.AddMergedFile(m1);

        // 2nd loop
        sut.HasFilesToMerge.Should().BeTrue();

        var chunk2 = sut.GetNextBatchOfFilesToMerge();

        chunk2.Should().HaveCount(10);
        chunk2.Should().ContainInOrder(files.Skip(10).Take(10));
        sut.HasFilesToMerge.Should().BeTrue();

        var m2 = new FileInfo("m2");
        sut.AddMergedFile(m2);

        //3rd loop
        sut.HasFilesToMerge.Should().BeTrue();

        var chunk3 = sut.GetNextBatchOfFilesToMerge();

        chunk3.Should().HaveCount(10);
        chunk3.Should().ContainInOrder(files.Skip(20).Take(10));
        sut.HasFilesToMerge.Should().BeTrue();

        var m3 = new FileInfo("m3");
        sut.AddMergedFile(m3);

        //4th loop
        sut.HasFilesToMerge.Should().BeTrue();

        var chunk4 = sut.GetNextBatchOfFilesToMerge();

        chunk4.Should().HaveCount(10);
        chunk4.Should().ContainInOrder(files.Skip(30).Take(10));
        sut.HasFilesToMerge.Should().BeTrue();

        var m4 = new FileInfo("m4");
        sut.AddMergedFile(m4);

        //5th loop
        sut.HasFilesToMerge.Should().BeTrue();

        var chunk5 = sut.GetNextBatchOfFilesToMerge();

        chunk5.Should().HaveCount(5);
        chunk5.Should().ContainInOrder(files.Skip(40).Take(5));
        sut.HasFilesToMerge.Should().BeTrue();

        var m5 = new FileInfo("m5");
        sut.AddMergedFile(m5);

        //6th loop
        sut.HasFilesToMerge.Should().BeTrue();

        var chunk6 = sut.GetNextBatchOfFilesToMerge();

        chunk6.Should().HaveCount(7);
        chunk6.Should().ContainInOrder(files.Skip(45).Union([m1,m2,m3,m4,m5]));

        sut.HasFilesToMerge.Should().BeFalse();
    }
}
