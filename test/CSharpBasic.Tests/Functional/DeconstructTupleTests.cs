using CSharpBasic.Functional;

namespace CSharpBasic.Tests.Functional;

[TestFixture]
internal sealed class DeconstructTupleTest
{
    [OneTimeSetUp]
    public void Setup()
    {
        _deconstructTuple = new DeconstructTuple();
    }

    private DeconstructTuple _deconstructTuple;

    [TestCase("New York City", 8175133, 468.48)]
    [TestCase("Shanghai", 0, 0)]
    public void TestQueryCityData(string city, int expectedPopulation, double expectedArea)
    {
        var (returnedCity, population, area) = _deconstructTuple.QueryCityData(city);

        Assert.Multiple(() =>
        {
            Assert.That(returnedCity, Is.EqualTo(city));
            Assert.That(population, Is.EqualTo(expectedPopulation));
            Assert.That(area, Is.EqualTo(expectedArea));
        });
    }
}