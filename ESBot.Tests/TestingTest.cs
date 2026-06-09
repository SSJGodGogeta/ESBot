namespace ESBot.Tests;

public class TestingTest
{
    [Fact]
    public void FailingTest_ForTesting()
    {
        Assert.Fail("This test should fail (testing CI)");
    }
}