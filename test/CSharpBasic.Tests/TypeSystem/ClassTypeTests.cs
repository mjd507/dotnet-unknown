using CSharpBasic.TypeSystem;

namespace CSharpBasic.Tests.TypeSystem;

internal sealed class ClassTypeTests
{
    [Test]
    public void Test1()
    {
        var classPerson1 = new Animal { FirstName = "John", LastName = "Doe" };
        var classPerson2 = new Animal { FirstName = "John", LastName = "Doe" };
        Assert.That(classPerson1, Is.Not.EqualTo(classPerson2));
    }
}