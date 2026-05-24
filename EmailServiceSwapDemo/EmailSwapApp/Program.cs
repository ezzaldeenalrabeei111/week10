using Microsoft.Extensions.DependencyInjection;
using EmailSwapApp.Services;

// Setup DI Container
var services = new ServiceCollection();

// To swap implementations, just change this ONE line:
// services.AddTransient<IEmailService, SmtpEmailService>();
services.AddTransient<IEmailService, SendGridEmailService>();

var serviceProvider = services.BuildServiceProvider();

// Resolve and use service (Client code doesn't change)
var emailService = serviceProvider.GetRequiredService<IEmailService>();
emailService.SendEmail("alrabeeiezzaldeen@gmail.com", "Welcome", "Hello from DI!");
