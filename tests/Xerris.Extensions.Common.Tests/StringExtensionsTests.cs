// ReSharper disable StringLiteralTypo
namespace Xerris.Extensions.Common.Tests;

public class StringExtensionsTests
{
    [Fact]
    public void Camel_case_extensions_have_no_effect_on_null_or_empty_strings()
    {
        StringExtensions.ToUpperCamelCase(null!).Should().BeNull();
        StringExtensions.ToLowerCamelCase(null!).Should().BeNull();

        string.Empty.ToUpperCamelCase().Should().BeEmpty();
        string.Empty.ToLowerCamelCase().Should().BeEmpty();
    }

    [Theory]
    [InlineData("foo", "foo", "Foo")]
    [InlineData("foo bar baz", "fooBarBaz", "FooBarBaz")]
    [InlineData("FOO bar BAZ", "fooBarBaz", "FooBarBaz")]
    [InlineData("fooBar BAZ", "fooBarBaz", "FooBarBaz")]
    [InlineData("alreadyCamelCase", "alreadyCamelCase", "AlreadyCamelCase")]
    [InlineData("AlreadyCamelCase", "alreadyCamelCase", "AlreadyCamelCase")]
    [InlineData("Words like crème brûlée have their accents removed ", "wordsLikeCremeBruleeHaveTheirAccentsRemoved", "WordsLikeCremeBruleeHaveTheirAccentsRemoved")]
    [InlineData("Numbers like 314 become part of the string", "numbersLike314BecomePartOfTheString", "NumbersLike314BecomePartOfTheString")]
    [InlineData("9/11 changed the world", "911ChangedTheWorld", "911ChangedTheWorld")]
    [InlineData("   \t\r\n  white space is removed from the start", "whiteSpaceIsRemovedFromTheStart", "WhiteSpaceIsRemovedFromTheStart")]
    [InlineData("and the end\r\n", "andTheEnd", "AndTheEnd")]
    [InlineData("\t or both ", "orBoth", "OrBoth")]
    [InlineData("     ", "", "")] // or entirely
    [InlineData("'puncuation\"usually\"starts a [new] word'.", "puncuationUsuallyStartsANewWord", "PuncuationUsuallyStartsANewWord")]
    [InlineData("but it can't when it's a contraction of two words.", "butItCantWhenItsAContractionOfTwoWords", "ButItCantWhenItsAContractionOfTwoWords")]
    [InlineData("ToCamelCase_should_convert_any_string_to_camelCase", "toCamelCaseShouldConvertAnyStringToCamelCase", "ToCamelCaseShouldConvertAnyStringToCamelCase")]
    public void Camel_case_extensions_convert_any_string_to_camel_case(
        string input, string expectedLowerCamelCase, string expectedUpperCamelCase)
    {
        input.ToLowerCamelCase().Should().Be(expectedLowerCamelCase);
        input.ToUpperCamelCase().Should().Be(expectedUpperCamelCase);
    }

    [Theory]
    [InlineData("Éric Söndergard", "Eric Sondergard")]
    [InlineData("Gêorgé Costanzà", "George Costanza")]
    [InlineData("Hafthór Júlíus Björnsson", "Hafthor Julius Bjornsson")]
    public static void StripDiacritics_should_normalize_strings_with_diacritics(string input, string expected)
    {
        input.StripDiacritics().Should().Be(expected);
    }

    [Theory]
    [InlineData("Eric Sondergard", "Eric Sondergard")]
    [InlineData("George Costanza", "George Costanza")]
    [InlineData("Hafthor Julíus Bjornsson", "Hafthor Julius Bjornsson")]
    public static void StripDiacritics_should_not_modify_strings_with_no_diacritics(string input, string expected)
    {
        input.StripDiacritics().Should().Be(expected);
    }

    [Theory]
    [InlineData("24/05/2017")]
    [InlineData(@"24\05\2017")]
    [InlineData("24>05<2017:24|05?*2017")]
    [InlineData("Fo\"o")]
    public void ToValidFileName_should_convert_value_to_valid_filename(string value)
    {
        var invalidCharactersInResult = value.ToValidFileName()
            .ToCharArray()
            .Intersect(Path.GetInvalidFileNameChars());

        invalidCharactersInResult.Should().BeEmpty();
    }

    public static IEnumerable<object[]> InvalidFileNameChars =>
        Path.GetInvalidFileNameChars().Select(c => new object[] { c });

    [Theory]
#pragma warning disable CA1825 // Avoid zero-length array allocations
    [MemberData(nameof(InvalidFileNameChars))]
#pragma warning restore CA1825 // Avoid zero-length array allocations
    public void ToValidFileName_throws_ArgumentException_if_replacement_string_contains_invalid_chars(char invalid)
    {
        const string value = "17/05/2017";

        var action = () => value.ToValidFileName(invalid.ToString());

        action.Should().Throw<ArgumentException>();
    }
}
