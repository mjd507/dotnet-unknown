// See https://aka.ms/new-console-template for more information

using CSharpBasic.TypeSystem;

// record type 
var person1 = new RecordType.Person("John", "Doe");
var person2 = new RecordType.Person("John", "Doe");
Console.WriteLine(person1.Equals(person2)); // True
Console.WriteLine(person1 == person2); // True

var person3 = person1 with { FirstName = "Johnathan" };
var person4 = person2 with { FirstName = "Johnathan" };
Console.WriteLine(person3.Equals(person4)); // True
Console.WriteLine(person3 == person4); // True

// class type
var classPerson1 = new ClassType.Person() { FirstName = "John", LastName = "Doe" };
var classPerson2 = new ClassType.Person() { FirstName = "John", LastName = "Doe" };
Console.WriteLine(classPerson1.Equals(classPerson2)); // False
Console.WriteLine(classPerson1 == classPerson2); // False

