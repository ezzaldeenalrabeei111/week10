using Microsoft.Extensions.DependencyInjection;
using GreetingApp.Services;

// Setup DI Container
var serviceProvider = new ServiceCollection()
    .AddTransient<IGreetingService, ConsoleGreetingService>()
    .BuildServiceProvider();

// Resolve and use service
var greeter = serviceProvider.GetRequiredService<IGreetingService>();
greeter.Greet("EZZALDEEN");
