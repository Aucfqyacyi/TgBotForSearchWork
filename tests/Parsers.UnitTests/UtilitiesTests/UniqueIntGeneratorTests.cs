using Parsers.Utilities;

namespace Parsers.UnitTests.UtilitiesTests;

public class UniqueIntGeneratorTests
{
    [Fact]
    public void Generate_UniqueInts()
    {
        int lastNum = 0;
        for (int i = 0; i < 1000; i++)
        {
            int num = UniqueIntGenerator.Generate();
            Assert.NotEqual(num, lastNum);
            Assert.True(lastNum < num);
            lastNum = num;
        }
    }
}
