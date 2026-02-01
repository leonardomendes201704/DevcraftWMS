using FluentAssertions;
using PerfectApiTemplate.Application.Common.Pagination;

namespace PerfectApiTemplate.Tests.Unit.Application;

public sealed class CursorPaginationTests
{
    [Fact]
    public void Build_Should_Create_Cursor()
    {
        var cursor = CursorToken.Build("2026-02-01T10:00:00Z", Guid.Empty);

        cursor.Should().Contain("|");
    }

    [Fact]
    public void TryParse_Should_Return_False_When_Empty()
    {
        var result = CursorToken.TryParse(null, out var token);

        result.Should().BeFalse();
        token.Parts.Should().BeEmpty();
    }

    [Fact]
    public void TryParse_Should_Split_Parts()
    {
        var result = CursorToken.TryParse("a|b|c", out var token);

        result.Should().BeTrue();
        token.Parts.Should().ContainInOrder("a", "b", "c");
    }
}
