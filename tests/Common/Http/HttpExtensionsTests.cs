using Xerris.Extensions.Common.Http;

namespace Xerris.Extensions.Common.Tests.Http;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedAutoPropertyAccessor.Local
public class HttpExtensionsTests
{
    private record Foo
    {
        public SomeEnum? Bar { get; init; }

        public string? Baz { get; init; }

        public string Qux { get; init; } = null!;

        public int? Garply { get; init; }

        public Guid? Plugh { get; init; }
    }

    private enum SomeEnum
    {
        Qux
    }

    [Fact]
    public void ToQueryString_ignores_empty_values()
    {
        var queryString = new Foo
        {
            Qux = "foo",
            Baz = null,
            Garply = 1
        }.ToQueryString();

        queryString.Should().Be("?Qux=foo&Garply=1");
    }

    [Fact]
    public void ToQueryString_returns_empty_string_if_no_properties_have_a_value()
    {
        var queryString = new Foo().ToQueryString();

        queryString.Should().BeEmpty();
    }

    [Fact]
    public void ToQueryString_converts_to_expected_value()
    {
        var foo = new Foo
        {
            Bar = SomeEnum.Qux,
            Baz = "erple",
            Qux = "foo",
            Garply = 1,
            Plugh = Guid.NewGuid()
        };

        var queryString = foo.ToQueryString();

        queryString.Should().Be($"?Bar=Qux&Baz=erple&Qux=foo&Garply=1&Plugh={foo.Plugh}");
    }
}
