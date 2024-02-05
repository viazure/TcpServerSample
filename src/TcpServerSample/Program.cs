using Serilog;
using System.Text;

class Program
{
    static void Main()
    {
        // Register encoding providers
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // Configure Serilog logging
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug() // Set the minimum log level to Debug
            .WriteTo.Console() // Output logs to the console
            .WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Day) // Output logs to a file
            .CreateLogger();

        Console.WriteLine("Service starting...");

        var server = new TcpServerSample.TcpServer();
        server.Start();

        Console.WriteLine("Service is running... Press 'q' then Enter to quit.");

        // Use a loop to keep the program running
        while (true)
        {
            string? command = Console.ReadLine();
            if (command != null && command.Equals("q", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
        }

        Console.WriteLine("Service is shutting down...");
        server.Stop();
        Log.CloseAndFlush();
    }
}