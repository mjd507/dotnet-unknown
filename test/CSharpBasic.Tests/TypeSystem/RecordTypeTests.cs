using CSharpBasic.TypeSystem;

namespace CSharpBasic.Tests.TypeSystem;

internal sealed class RecordTypeTests
{
    [Test]
    public void TestRecordEquals()
    {
        var person1 = new Person("John", "Doe");
        var person2 = new Person("John", "Doe");
        Assert.That(person1, Is.EqualTo(person2));
        Assert.That(person1, Is.Not.SameAs(person2));

        var person3 = person1 with { FirstName = "Johnathan" };
        var person4 = person2 with { FirstName = "Johnathan" };
        Assert.That(person3, Is.EqualTo(person4));
        Assert.That(person3, Is.Not.SameAs(person4));
    }
}