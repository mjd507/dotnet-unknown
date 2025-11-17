namespace CSharpBasic.TypeSystem;

interface IEquatable<in T>
{
    bool Equals(T obj);
}

public class Car(string make, string model, string year) : IEquatable<Car>
{
    public string Make { get; } = make;
    public string Model { get; } = model;
    public string Year { get; } = year;

    public bool Equals(Car car)
    {
        return Make == car?.Make && Model == car.Model && Year == car.Year;
    }
}