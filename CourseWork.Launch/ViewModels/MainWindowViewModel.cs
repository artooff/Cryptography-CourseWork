using CourseWork.Client;
using CourseWork.Modes;
using CourseWork.Modes.EncryptModes;
using CourseWork.Modes.FilesOperating;
using CourseWork.Modes.Functions;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF.MVVM;

//var filePath = "C:\\Users\\Andrey\\Documents\\Весна.wav";
//var hostingClient = new HostingClient();

////получили публичный ключ с сервера:
//var publicKeyResponse = await hostingClient.GetPublicKeyFromsServer();
//var serverPublicKey = publicKeyResponse.Key;

//////сгенерировали клиентский ключ:
//var clientRC6 = new RC6.RC6();
//clientRC6.GenerateKey(128);
//var clientKey = clientRC6.MainKey;

////зашифровали клиентский ключ публичным ключем:
//ElGamal.ElGamal clientElGamal = new ElGamal.ElGamal();
//clientElGamal.PublicKey = new ElGamal.PublicKey
//{
//    P = BigInteger.Parse(serverPublicKey.P),
//    G = BigInteger.Parse(serverPublicKey.G),
//    Y = BigInteger.Parse(serverPublicKey.Y),
//};
//var encryptedClientKey = clientElGamal.Encrypt(clientKey);


//Console.WriteLine("Client key:");
//Console.WriteLine(BitConverter.ToString(clientRC6.MainKey));
//Console.WriteLine("Encrypted client key:");
//Console.WriteLine(encryptedClientKey.ToString());

//await hostingClient.SendClientKeyToServer(encryptedClientKey);

//var filesInfoResponse = await hostingClient.GetFilesInfoFromServer();
//var filesList = filesInfoResponse.FileNames.ToList<string>();

namespace CourseWork.Launch.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private HostingClient _hostingClient;
        private ElGamal.ElGamal _clientElGamal;
        
        private Lazy<ICommand> _sendCommand;
        private Lazy<ICommand> _downloadCommand;

        public ICommand SendCommand => _sendCommand.Value;
        public ICommand DownloadCommand => _downloadCommand.Value;
        public ObservableCollection<string> AvailableFiles { get; set; }

        public string SelectedFile { get; set; }
        public int ProgressPercent => 50;
        private RC6.RC6 _algorhytm;
        private CipherMode _cipherMode;
        private Padder _padder;
        private int _blockSize;
        private int _processorCount;

        public MainWindowViewModel(){
            _hostingClient = new HostingClient();

            _algorhytm = new RC6.RC6();
            _cipherMode = new ModeFactory().CreateCipherMode(EncryptionMode.CBC, _algorhytm);
            _blockSize = _algorhytm.GetBlockSize();
            _padder = new Padder(_blockSize);
            _processorCount = Environment.ProcessorCount;

            KeyDistribution();
            
            AvailableFiles = new ObservableCollection<string>();
            RefreshAvailableFiles();

            _sendCommand = new Lazy<ICommand>(() => new RelayCommand(_ => SendFile()));
            _downloadCommand = new Lazy<ICommand>(() => new RelayCommand(_ => DownloadFile()));

        }

        private async void SendFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() != null)
            {
                Encrypt(fileDialog.FileName);
                //var task = _hostingClient.SendFileToServer(Path.GetFileName(fileDialog.FileName), encryptedFile, _cipher.IV);
                //await Task.WhenAll(task);
                RefreshAvailableFiles();
            }
            //open file dialog
            //send file to server
        }

        public void OnBlocksSetEncrypted(byte[] encryptedBlocks, bool isLastBlock)
        {

        }

        private void DownloadFile()
        {
            MessageBox.Show("download file " + SelectedFile);
            RefreshAvailableFiles();

            //download selected file from server
        }

        public async void Encrypt(string filePath)
        {
            var tasks = new List<Task>();

            var fileReader = new FileReader(filePath, _blockSize);
            var iterationsCount = fileReader.BlocksNumber % _processorCount == 0
                ? fileReader.BlocksNumber / _processorCount
                : fileReader.BlocksNumber / _processorCount + 1;

            var outputBuffer = new byte[iterationsCount][];
            var isLastBlock = false;
            for (int i = 0; i < iterationsCount; i++)
            {
                var readedBlocks = fileReader.GetNextBlocks(_processorCount);
                if (i == iterationsCount - 1)
                {
                    if (readedBlocks.Last().Length == _blockSize)
                    {
                        readedBlocks.Add(Enumerable.Repeat((Byte)_blockSize, _blockSize).ToArray());
                    }
                    else
                    {
                        readedBlocks[readedBlocks.Count - 1] = _padder.PadBuffer(readedBlocks.Last());
                    } 
                    isLastBlock = true;
                }
                var encryptedBlocks = _cipherMode.EncryptBlocks(readedBlocks);
                var task = _hostingClient.SendFileBlockToServer(Path.GetFileName(filePath), encryptedBlocks, _cipherMode.IV, i, iterationsCount);
                await Task.WhenAll(task);
            }
            _cipherMode.Reset();
        }

        private async void KeyDistribution()
        {
            //получить публичный ключ с сервера:
            var publicKeyResponse = await _hostingClient.GetPublicKeyFromServer();
            var serverPublicKey = publicKeyResponse.Key;

            //зашифровать клиентский ключ публичным ключем:
            _clientElGamal = new ElGamal.ElGamal();
            _clientElGamal.PublicKey = new ElGamal.PublicKey
            {
                P = BigInteger.Parse(serverPublicKey.P),
                G = BigInteger.Parse(serverPublicKey.G),
                Y = BigInteger.Parse(serverPublicKey.Y),
            };

            _algorhytm.GenerateKey(128);
            //отправить клиентский ключ серверу
            await _hostingClient.SendClientKeyToServer(_clientElGamal.Encrypt(_algorhytm.MainKey));
        }

        private async void RefreshAvailableFiles()
        {
            var filesInfoResponse = await _hostingClient.GetFilesInfoFromServer();
            AvailableFiles.Clear();
            foreach(var fileName in filesInfoResponse.FileNames)
            {
                AvailableFiles.Add(fileName);
            }
        }
    }
}
