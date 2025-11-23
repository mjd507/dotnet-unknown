using System.Text.Json.Serialization;
using DotNetUnknown.Json;

namespace DotNetUnknown.Tests.Json;

[TestFixture]
internal sealed class JsonPolymorphicTests
{
    [JsonDerivedType(typeof(Car), typeDiscriminator: "car")]
    [JsonDerivedType(typeof(Truck), typeDiscriminator: "truck")]
    internal class Vehicle
    {
        public required string Make { get; init; }
        public required string Model { get; init; }

        internal class Car : Vehicle
        {
            public required int SeatingCapacity { get; init; }
            public double TopSpeed { get; init; }
        }

        internal class Truck : Vehicle
        {
            public double PayloadCapacity { get; init; }
        }
    }

    internal static IEnumerable<Vehicle> VehiclesProvider
    {
        get
        {
            yield return new Vehicle.Car()
            {
                Make = "Mercedes-Benz",
                Model = "s500",
                SeatingCapacity = 5,
                TopSpeed = 240.0
            };
            yield return new Vehicle.Truck()
            {
                Make = "Isuzu",
                Model = "NQR",
                PayloadCapacity = 7500.0
            };
        }
    }

    // Given
    [TestCaseSource(nameof(VehiclesProvider))]
    public void TestPolymorphism(Vehicle vehicle)
    {
        // [Serialize] When
        var actualJson = JsonUtils.Serialize(vehicle);
        // [Serialize] Then
        const string expectedCarJson =
            """
            {"$type":"car","seatingCapacity":5,"topSpeed":240,"make":"Mercedes-Benz","model":"s500"}
            """;
        const string expectedTruckJson =
            """
            {"$type":"truck","payloadCapacity":7500,"make":"Isuzu","model":"NQR"}
            """;
        var isCar = vehicle is Vehicle.Car;
        Assert.That(actualJson, Is.EqualTo(isCar ? expectedCarJson : expectedTruckJson));

        // [Deserialize] When
        var deserializedVehicle = JsonUtils.Deserialize<Vehicle>(actualJson)!;
        // [Deserialize] Then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deserializedVehicle, Is.Not.Null);
            Assert.That(deserializedVehicle.Make, Is.EqualTo(vehicle.Make));
            Assert.That(deserializedVehicle.Model, Is.EqualTo(vehicle.Model));
            if (isCar)
            {
                Assert.That(((Vehicle.Car)deserializedVehicle).SeatingCapacity, Is.EqualTo(5));
                Assert.That(((Vehicle.Car)deserializedVehicle).TopSpeed, Is.EqualTo(240.0));
            }
            else
            {
                Assert.That(((Vehicle.Truck)deserializedVehicle).PayloadCapacity, Is.EqualTo(7500.0));
            }
        }
    }
}