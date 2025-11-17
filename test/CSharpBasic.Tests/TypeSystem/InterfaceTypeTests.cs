using CSharpBasic.TypeSystem;

namespace CSharpBasic.Tests.TypeSystem;

public class InterfaceTypeTests
{
    [Test]
    public void SameCarByOverrideEquals()
    {
        var car1 = new Car("China", "Toyota", "2025");
        var car2 = new Car("China", "Toyota", "2025");
        Assert.That(car1.Equals(car2), Is.True);
    }
}