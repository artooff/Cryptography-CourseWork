using CourseWork.Server;
using Google.Protobuf;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;

namespace CourseWork.Client
{
    public class HostingClient
    {
        private readonly ILogger<HostingClient> _logger;
        private readonly Hosting.HostingClient _client;

        public HostingClient(ILogger<HostingClient> logger = null)
        {
            _logger = logger;

            var httpHandler = new HttpClientHandler();

            httpHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress("https://localhost:7101",
                new GrpcChannelOptions
                {
                    HttpHandler = httpHandler
                });

            _client = new Hosting.HostingClient(channel);

        }

        public async Task<GetPublicKeyResponse> GetPublicKeyFromServer(CancellationToken token = default)
        {
            try
            {
                var request = new GetPublicKeyRequest
                {
                    ClientInfo = "123"
                };

                return await _client.GetPublicKeyFromServerAsync(request);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<SendClientKeyResponse> SendClientKeyToServer(ElGamal.EncryptedMessage key, CancellationToken token = default)
        {
            try
            {
                var request = new SendClientKeyRequest
                {
                    ClientInfo = "123",
                    Key = new ClientKey
                    {
                        Data = new EncryptedKey
                        {
                            PartA = key.A.ToString(),
                            PartB = key.B.ToString()
                        }
                    }
                };

                return await _client.SendClientKeyToServerAsync(request);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<GetFilesInfoResponse> GetFilesInfoFromServer(CancellationToken token = default)
        {
            try
            {
                var request = new GetFilesInfoRequest
                {
                    ClientInfo = "123"
                };
                return await _client.GetFilesInfoFromServerAsync(request);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task<SendFileBlockResponse> SendFileBlockToServer(string fileName, byte[] encryptedFileBlock, byte[] iv,
            int blockNumber, int blocksCount, CancellationToken token = default)
        {
            try
            {
                var request = new SendFileBlockRequest
                {
                    File = new EncryptedFileBlock
                    {
                        Name = fileName,
                        Data = ByteString.CopyFrom(encryptedFileBlock)
                    },
                    IV = ByteString.CopyFrom(iv),
                    BlockNumber = blockNumber,
                    BlocksCount = blocksCount 
                };
                return await _client.SendFileToServerAsync(request);

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}