using Parsers.Extensions;

namespace Parsers.UnitTests.ExtensionsTests;

public class StringExtensionTests
{
    [Fact]
    public void GetWithoutTextInBrackets_StrWithTextInBrackets_StrNotContainTextInBrackets()
    {
		// Arrange
		string textInBrackets = "(some text)";
		string str = "Some text " + textInBrackets;

		//Act
		string newStr = str.GetWithoutTextInBrackets();

        //Assert
        Assert.NotEmpty(newStr);
        Assert.NotEqual(' ', newStr[^1]);
		Assert.DoesNotContain(textInBrackets, newStr);
	}

    [Fact]
    public void GetWithoutTextInBrackets_StrWithoutTextInBrackets_StrNotChanged()
    {
        // Arrange
        string textInBrackets = "some text";
        string str = "Some text " + textInBrackets;

        //Act
        string newStr = str.GetWithoutTextInBrackets();

        //Assert
        Assert.NotEmpty(newStr);
        Assert.Contains(textInBrackets, newStr);
    }

    [Fact]
    public void GetNumberFromUrl_UrlWithNumber_CorrectNumber()
    {
        // Arrange
        ulong firstNumber = 12321312;
        string urlWithNumber1 = $"http://www.somesite.eu/{firstNumber}/";
        string urlWithNumber2 = $"http://www.somesite.eu/{firstNumber}-sometext/";
        //Act

        //Assert
        Assert.Equal(firstNumber, urlWithNumber1.GetNumberFromUrl(3));
        Assert.Equal(firstNumber, urlWithNumber2.GetNumberFromUrl(3, "-"));
    }

    [Fact]
    public void GetNumberFromUrl_InCorrectUrl_ThrowExpection()
    {
        // Arrange
        ulong firstNumber = 12321312;
        string urlWithNumber1 = $"http://www.somesite.eu/{firstNumber}/";
        string urlWithNumber2 = $"http://www.somesite.eu/{firstNumber}-sometext/";
        string urlWithoutNumber = $"http://www.somesite.eu/sometext/";
        //Act

        //Assert
        Assert.Throws<IndexOutOfRangeException>(() => urlWithNumber1.GetNumberFromUrl(1000, "/"));
        Assert.ThrowsAny<Exception>(() => urlWithNumber1.GetNumberFromUrl(2));
        Assert.ThrowsAny<Exception>(() => urlWithNumber2.GetNumberFromUrl(2));
        Assert.ThrowsAny<Exception>(() => urlWithNumber2.GetNumberFromUrl(3, "     "));
        Assert.ThrowsAny<Exception>(() => urlWithoutNumber.GetNumberFromUrl(2));
    }

    [Fact]
    public void ParseHtml_TextWithTags_ReplaceThemOnNewLine()
    {
        // Arrange
        string text = "<p>Some text<br>next text<br>text text text</p>" +
                      "<p>Some text<br>next text<br>text text text</p>" +
                      "<ul>Some text<br>next text<br>text text text</ul>";

        //Act
        string newText = text.ParseHtml();
        string expectedText = "Some text\r\nnext text\r\ntext text text\r\n" +
                              "Some text\r\nnext text\r\ntext text text\r\n" +
                              "Some text\r\nnext text\r\ntext text text";
        //Assert
        Assert.Equal(expectedText, newText);
    }

    [Fact]
    public void ParseHtml_TextWithLiTags_ReplaceThemOnThreeDashesAndNewLine()
    {
        // Arrange
        string text = "<li>Some text next text text text text</li>" +
                      "<li>Some text next text text text text</li>" +
                      "<li>Some text next text text text text</li>";

        //Act
        string newText = text.ParseHtml();
        string expectedText = "--- Some text next text text text text\r\n" +
                              "--- Some text next text text text text\r\n" +
                              "--- Some text next text text text text";
        //Assert
        Assert.Equal(expectedText, newText);
    }

    [Fact]
    public void ParseHtml_TextWithMnemonics_ReplaceThemOnCorrectSymbols()
    {
        // Arrange
        string text = "&amp;Some text next&amp; &gt;text &lt;text &lt;text text&gt;";
        //Act
        string newText = text.ParseHtml();
        string expectedText = "&Some text next& >text <text <text text>";
        //Assert
        Assert.Equal(expectedText, newText);
    }

    [Fact]
    public void ParseHtml_TextWithHTags_ReplaceThemOnBTagsWithNewLine()
    {
        // Arrange
        string text = "<h1>Some text next text text text text</h1>" + 
                      "<h2>Some text next text text text text</h2>" +
                      "<h3>Some text next text text text text</h3>";

        //Act
        string newText = text.ParseHtml();
        string expectedText = "<b>Some text next text text text text</b>\r\n" +
                              "<b>Some text next text text text text</b>\r\n" +
                              "<b>Some text next text text text text</b>";
        //Assert
        Assert.Equal(expectedText, newText);
    }
}
