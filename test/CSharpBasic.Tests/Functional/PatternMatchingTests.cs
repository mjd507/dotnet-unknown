using CSharpBasic.Functional;

namespace CSharpBasic.Tests.Functional;

[TestFixture]
internal sealed class PatternMatchingTests
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _patternMatching = new PatternMatching();
    }

    private PatternMatching _patternMatching;

    [TestCase(0, "solid")]
    [TestCase(32, "solid/liquid transition")]
    [TestCase(100, "liquid")]
    [TestCase(212, "liquid / gas transition")]
    [TestCase(250, "gas")]
    public void TestWaterState(int tempInFahrenheit, string expectedWaterState)
    {
        var state = _patternMatching.WaterState(tempInFahrenheit);
        Assert.That(state, Is.EqualTo(expectedWaterState));
    }


    [TestCase(11, 1500.00, 0.10)]
    [TestCase(6, 700, 0.05)]
    [TestCase(4, 300, 0.02)]
    [TestCase(2, 100, 0.00)]
    [TestCase(-1, -1, 0.00)]
    public void TestCalculateDiscount(int items, double cost, double expectedDiscount)
    {
        if (items == -1)
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _patternMatching.CalculateDiscount(null));
            Assert.That(ex.Message, Contains.Substring("Can't calculate discount on null order"));
            return;
        }

        var order = new PatternMatching.Order(items, (decimal)cost);
        var discount = _patternMatching.CalculateDiscount(order);
        Assert.That(discount, Is.EqualTo((decimal)expectedDiscount));
    }
}