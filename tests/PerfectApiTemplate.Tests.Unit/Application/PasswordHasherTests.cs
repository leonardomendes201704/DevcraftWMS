using FluentAssertions;
using PerfectApiTemplate.Application.Abstractions.Auth;
using PerfectApiTemplate.Application.Features.Auth;

namespace PerfectApiTemplate.Tests.Unit.Application;

public sealed class PasswordHasherTests
{
    private readonly IPasswordHasher _hasher = new PasswordHasher();

    [Fact]
    public void Hash_And_Verify_Should_Succeed()
    {
        var hash = _hasher.Hash("Pass@1234");
        var valid = _hasher.Verify(hash, "Pass@1234");

        valid.Should().BeTrue();
    }

    [Fact]
    public void Verify_With_Wrong_Password_Should_Fail()
    {
        var hash = _hasher.Hash("Pass@1234");
        var valid = _hasher.Verify(hash, "Wrong");

        valid.Should().BeFalse();
    }
}
