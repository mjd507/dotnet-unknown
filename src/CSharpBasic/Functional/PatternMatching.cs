namespace CSharpBasic.Functional;

public class PatternMatching
{
    public string WaterState(int tempInFahrenheit) =>
        tempInFahrenheit switch
        {
            < 32 => "solid",
            32 => "solid/liquid transition",
            < 212 => "liquid",
            212 => "liquid / gas transition",
            _ => "gas",
        };

    public decimal CalculateDiscount(Order? order) =>
        order switch
        {
            (> 10, > 1000.00m) => 0.10m,
            (> 5, > 50.00m) => 0.05m,
            { Cost: > 250.00m } => 0.02m,
            null => throw new ArgumentNullException(nameof(order), "Can't calculate discount on null order"),
            _ => 0m,
        };

    public record Order(int Items, decimal Cost);
}