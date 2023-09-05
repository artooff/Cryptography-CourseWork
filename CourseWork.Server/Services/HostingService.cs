using CourseWork.ElGamal;
using CourseWork.Modes;
using CourseWork.Modes.EncryptModes;
using CourseWork.Modes.Functions;
using CourseWork.RC6;
using Google.Protobuf;
using Grpc.Core;
using System.Numerics;

namespace CourseWork.Server.Services
{
    public class HostingService : Hosting.HostingBase
    {
        private readonly ILogger<HostingService> _logger;
        private readonly string _filesDirectory = Directory.GetCurrentDirectory() + "\\Files\\";
        private List<string> _availableFiles = new List<string>();


        private readonly ElGamal.ElGamal _elGamal;
        private readonly RC6.RC6 _algorhytm;
        private readonly Padder _padder;
       // private readonly Cipher _cipher;
        private byte[] _clientKey;
        private byte[]? _lastEncryptedBlock;

        private FileStream _currentFileStream;
        public HostingService(ILogger<HostingService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _availableFiles = Directory.GetFiles(_filesDirectory).Select(path => Path.GetFileName(path)).ToList();  
            _elGamal = new ElGamal.ElGamal();
            _algorhytm = new();
            _padder = new Padder(_algorhytm.GetBlockSize());
        }

        public override async Task<GetPublicKeyResponse> GetPublicKeyFromServer(GetPublicKeyRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation(nameof(GetPublicKeyFromServer) + " method called");
                //TODO: get info about client and write to list
                _elGamal.KenerateGeys(40);
                _logger.LogInformation(_elGamal.PublicKey.ToString());
                return new GetPublicKeyResponse
                {
                    IsSuccess = true,
                    Key = new PublicKey
                    {
                        P = _elGamal.PublicKey.P.ToString(),
                        G = _elGamal.PublicKey.G.ToString(),
                        Y = _elGamal.PublicKey.Y.ToString()
                    }
                };
            }
            catch(Exception ex)
            {
                return new GetPublicKeyResponse
                {
                    IsSuccess = false
                };
            }
        }

        public override async Task<SendClientKeyResponse> SendClientKeyToServer(SendClientKeyRequest request, ServerCallContext context)
        {
            try
            {
                //decrypt key
                _clientKey = _elGamal.Decrypt(new ElGamal.EncryptedMessage
                {
                    A = BigInteger.Parse(request.Key.Data.PartA),
                    B = BigInteger.Parse(request.Key.Data.PartB)

                });
                
                _logger.LogInformation("Got client key:\n" + BitConverter.ToString(_clientKey));
                //registr client
                return new SendClientKeyResponse
                {
                    IsSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new SendClientKeyResponse
                {
                    IsSuccess = false
                };
            }
        }

        public override async Task<GetFilesInfoResponse> GetFilesInfoFromServer(GetFilesInfoRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation(nameof(GetFilesInfoFromServer) + " method called");
                var response = new GetFilesInfoResponse
                {
                    IsSuccess = true
                };
                response.FileNames.AddRange(_availableFiles);
                return response;
            }
            catch (Exception ex)
            {
                return new GetFilesInfoResponse { IsSuccess = false };
            }
        }

        public override async Task<SendFileBlockResponse> SendFileToServer(SendFileBlockRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation(nameof(SendFileToServer) + " method called");
                _logger.LogInformation("Got file " + request.File.Name);

                 var fileStream = new FileStream(_filesDirectory + request.File.Name, FileMode.Append);

                using (fileStream)
                {
                    var byteArray = request.File.Data.ToByteArray();
                    var decryptedArray = Decrypt(byteArray, request);
                    fileStream.Write(decryptedArray, 0, decryptedArray.Length);
                }

                //var cipher = new Cipher(EncryptionMode.CBC);
                //cipher.GenerateKey(128, _clientKey);
                //cipher.IV = request.IV.ToByteArray();
                //var decryptedFile = cipher.Decrypt(request.File.Data.ToByteArray());
                //System.IO.File.WriteAllBytes(_filesDirectory + request.File.Name, decryptedFile);
                //_availableFiles.Add(request.File.Name);
                return new SendFileBlockResponse
                {
                    FileName = request.File.Name,
                    IsSuccess = true,
                    FullFileSended = (request.BlockNumber == request.BlocksCount)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(SendFileToServer) + " exception");
                return new SendFileBlockResponse
                {
                    IsSuccess = false
                };
            }
        }

        //public byte DecryptBlocks(byte[] blocksArray)
        //{

        //}

        public byte[] Decrypt(byte[] inputBuffer, SendFileBlockRequest request)
        {
            var cipherMode = new ModeFactory().CreateCipherMode(EncryptionMode.CBC, _algorhytm);
            _algorhytm.GenerateKey(128, _clientKey);
            cipherMode.IV = request.IV.ToByteArray();
            var inputBlocks = Functions.GetBlocksListFromArray(inputBuffer, _algorhytm.GetBlockSize());
            var decryptedBlocks = cipherMode.DecryptBlocks(inputBlocks, request.BlockNumber == 0 ? null : _lastEncryptedBlock);
            _lastEncryptedBlock = inputBlocks.Last();
            if (request.BlockNumber == request.BlocksCount - 1)
            {
                decryptedBlocks = _padder.RemovePadding(decryptedBlocks);
            }
            ////разбить массив на processorCount блоков по 16 байт
            //var inputBlocks = GetBlocksList(inputBuffer);
            //var iterationsCount = inputBlocks.Count % _processorCount == 0
            //    ? inputBlocks.Count / _processorCount
            //    : inputBlocks.Count / _processorCount + 1;
            //var outputBuffer = new byte[iterationsCount][];

            //for (int i = 0; i < iterationsCount; i++)
            //{
            //    var decryptedBlocks = _cipherMode.DecryptBlocks(inputBlocks.GetRange(
            //        i * _processorCount, Math.Min(_processorCount, inputBlocks.Count - i * _processorCount)));
            //    outputBuffer[i] = decryptedBlocks;

            //    if (i == outputBuffer.Length - 1)
            //    {
            //        outputBuffer[outputBuffer.Length - 1] = _padder.RemovePadding(outputBuffer.Last());
            //    }
            //}

            return decryptedBlocks;
        }

        public void FileCombiner()
        {

        }
    }
}
