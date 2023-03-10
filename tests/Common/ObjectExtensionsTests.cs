namespace Xerris.Extensions.Common.Tests;

public class ObjectExtensionsTests
{
    [Fact]
    public void Yield_returns_enumerable_containing_target_object()
    {
        var foo = new { bar = "baz" };

        var enumerable = foo.Yield();

        enumerable.Single().Should().BeSameAs(foo);
    }

    [Fact]
    public void Yield_returns_enumerable_containing_target_value()
    {
        var value = 1;

        var enumerable = value.Yield();

        enumerable.Single().Should().Be(value);
    }

    [Fact]
    public void Yield_returns_empty_enumerable_for_null_object()
    {
        string? value = null;

        var enumerable = value.Yield();

        enumerable.Should().BeEmpty();
    }
}
