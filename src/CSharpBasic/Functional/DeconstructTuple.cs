namespace CSharpBasic.Functional;

public class DeconstructTuple
{
    public (string, int, double) QueryCityData(string name)
    {
        return name == "New York City" ? (name, 8175133, 468.48) : (name, 0, 0);
    }
}