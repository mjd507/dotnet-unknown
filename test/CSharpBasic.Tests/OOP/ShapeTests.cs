using CSharpBasic.OOP;

namespace CSharpBasic.Tests.OOP;

internal sealed class ShapeTests
{
    private List<Shape> _shapes;

    [OneTimeSetUp]
    public void Setup()
    {
        _shapes =
        [
            new Circle(),
            new Rectangle(),
            new Triangle()
        ];
    }

    [Test]
    public void TestShape()
    {
        foreach (var shape in _shapes)
        {
            shape.Draw();
        }

        Assert.Pass();
    }
}