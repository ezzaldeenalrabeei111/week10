namespace GreetingApp.Services;

public interface IGreetingService
{
    void Greet(string name);
}

public class ConsoleGreetingService : IGreetingService
{
    public void Greet(string name)
    {
        Console.WriteLine($"Hello, {name}! Welcome to Dependency Injection in .NET.");
    }
}
