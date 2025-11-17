namespace CSharpBasic.TypeSystem;

public abstract class InterfaceType
{
    interface IEquatable<T>
    {
        bool Equals(T obj);
    }

    public class Car : IEquatable<Car>
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? Year { get; set; }

        public bool Equals(Car? car)
        {
            return (Make, Model, Year) ==
                   (car?.Make, car?.Model, car?.Year);
        }
    }

    // ==============================================

    // Internal type that cannot be exposed publicly
    internal class InternalConfiguration
    {
        public string Setting { get; set; } = "";
    }

    // Internal interface that CAN be implemented with public members
    // because it only uses public types in its signature
    internal interface ILoggable
    {
        void Log(string message); // string is public, so this works with implicit implementation
    }

    // Interface with internal accessibility using internal types
    internal interface IConfigurable
    {
        void Configure(InternalConfiguration config); // Internal type prevents implicit implementation
    }

    // This class shows both implicit and explicit interface implementation
    public class ServiceImplementation : ILoggable, IConfigurable
    {
        // Explicit implementation required for IConfigurable because it uses internal types
        void IConfigurable.Configure(InternalConfiguration config)
        {
            // Implementation here
            Console.WriteLine($"Configured with: {config.Setting}");
        }

        // Implicit implementation works for ILoggable because string is public
        public void Log(string message)
        {
            Console.WriteLine($"Log: {message}");
        }

        // If we tried implicit implementation for IConfigurable, this wouldn't compile:
        // public void Configure(InternalConfiguration config) // Error: cannot expose internal type
    }
}