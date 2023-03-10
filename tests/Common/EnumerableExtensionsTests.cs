namespace Xerris.Extensions.Common.Tests;

public class EnumerableExtensionsTests
{
    [Fact]
    public void ForEach_executes_action_over_all_collection_items()
    {
        var list = new List<int>();

        var values = Enumerable.Range(1, 10).ToArray();

        values.ForEach(list.Add);

        list.Should().BeEquivalentTo(values);
    }
}
