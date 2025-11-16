// See https://aka.ms/new-console-template for more information

using CSharpBasic.OOP;
using CSharpBasic.TypeSystem;

// record type 
var person1 = new RecordType.Person("John", "Doe");
var person2 = new RecordType.Person("John", "Doe");
Console.WriteLine(person1.Equals(person2)); // True
Console.WriteLine(person1 == person2); // True
Console.WriteLine(ReferenceEquals(person1, person2)); // False

var person3 = person1 with { FirstName = "Johnathan" };
var person4 = person2 with { FirstName = "Johnathan" };
Console.WriteLine(person3.Equals(person4)); // True
Console.WriteLine(person3 == person4); // True
Console.WriteLine(ReferenceEquals(person3, person4)); // False

// class type
var classPerson1 = new ClassType.Person { FirstName = "John", LastName = "Doe" };
var classPerson2 = new ClassType.Person { FirstName = "John", LastName = "Doe" };
Console.WriteLine(classPerson1.Equals(classPerson2)); // False
Console.WriteLine(classPerson1 == classPerson2); // False


// struct type
var coords1 = new StructType.Coords(0, 0);
var coords2 = coords1; // Create new struct object. Note that struct can be initialized without using "new".
Console.WriteLine(ReferenceEquals(coords1, coords2)); // False
coords2.X = 1;
coords2.Y = 1;
Console.WriteLine($"coords1.X={coords1.X}, coords1.Y={coords1.Y}"); // 0,0


// Polymorphism
List<Shape> shapes =
[
    new Circle(),
    new Rectangle(),
    new Triangle()
];

foreach (var shape in shapes)
{
    shape.Draw();
}