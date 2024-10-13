using Microsoft.Extensions.Configuration;
using Serilog;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpServerSample
{
    class TcpServer
    {
        private readonly TcpListener _listener;
        private readonly IConfiguration _configuration;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public TcpServer()
        {
            _configuration = Utils.ConfigurationProvider.Instance;
            _listener = new TcpListener(IPAddress.Any, _configuration.GetValue<int>("Port"));
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("The service has started.");

            // Start receiving client requests.
            _ = AcceptClientsAsync(_cancellationTokenSource.Token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _listener.Stop();
            Console.WriteLine("The service has stopped.");
        }

        private async Task AcceptClientsAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync(cancellationToken);
                    var clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;

                    Log.Information("Client connected from {ClientIP}:{ClientPort}", clientEndPoint?.Address, clientEndPoint?.Port);

                    // Handle client requests in separate methods or tasks.
                    await Task.Run(() => HandleClientAsync(client), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Log.Information("The reception of the client has been canceled.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while accepting a new client connection.");
            }
            finally
            {
                _listener.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            var clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            try
            {
                using (client)
                using (var networkStream = client.GetStream())
                {
                    Memory<byte> buffer = new byte[client.ReceiveBufferSize];

                    // Continuously read messages until the client disconnects
                    while (true)
                    {
                        int bytesRead = await networkStream.ReadAsync(buffer, cancellationToken: default);
                        if (bytesRead == 0)
                        {
                            Log.Information("Client {ClientIP}:{ClientPort} disconnected.", clientEndPoint?.Address, clientEndPoint?.Port);
                            break;
                        }

                        Encoding encoding = GetEncoding();
                        string message = encoding.GetString(buffer[..bytesRead].ToArray());
                        Log.Information($"Received: {message}");

                        string responseMessage = "ACK";
                        ReadOnlyMemory<byte> responseBytes = encoding.GetBytes(responseMessage);
                        await networkStream.WriteAsync(responseBytes, cancellationToken: default);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error handling client request.");
            }
        }

        private Encoding GetEncoding()
        {
            // Retrieve message encoding from configuration file
            const string DefaultEncoding = "UTF-8";
            string encodingName = _configuration.GetValue<string>("Encoding") ?? DefaultEncoding;
            Encoding encoding;

            // Try to obtain the encoding specified in the configuration, default to UTF-8 if it fails
            try
            {
                encoding = Encoding.GetEncoding(encodingName);
            }
            catch (ArgumentException)
            {
                encoding = Encoding.UTF8;
            }

            return encoding;
        }
    }
}
