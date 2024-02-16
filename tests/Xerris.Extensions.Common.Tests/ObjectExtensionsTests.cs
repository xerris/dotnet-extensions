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

    [Fact]
    public void ValueOrDefault_returns_default_value_for_selected_property_when_null()
    {
        var foo = new { bar = (string?) null };

        var value = foo.ValueOrDefault(x => x?.bar);

        value.Should().BeNull();
    }

    [Fact]
    public void ValueOrDefault_returns_selected_value_for_selected_property_when_not_null()
    {
        var foo = new { bar = "baz" };

        var value = foo.ValueOrDefault(x => x?.bar);

        value.Should().Be("baz");
    }

    [Fact]
    public void ValueOrDefault_returns_default_value_for_selected_type_when_parent_object_is_null()
    {
        var foo = (List<string>) null!;

        var value = foo.ValueOrDefault(x => x?.Count);

        value.Should().BeNull();
    }
}
