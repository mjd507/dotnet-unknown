using CSharpBasic.TypeSystem;

namespace CSharpBasic.Tests.TypeSystem;

internal sealed class StructTypeTests
{
    [Test]
    public void TestStruct()
    {
        var coords1 = new Coords(0, 0);
        var coords2 = coords1; // Create new struct object. Note that struct can be initialized without using "new".
        coords2.X = 1;
        coords2.Y = 1;
        Assert.That(coords1, Is.EqualTo(new Coords(0, 0)));
        Assert.That(coords2, Is.EqualTo(new Coords(1, 1)));
    }
}