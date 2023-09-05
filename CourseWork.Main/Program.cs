using CourseWork.Client;
using CourseWork.ElGamal;
using CourseWork.RC6;
using CourseWork.Modes.EncryptModes;
using CourseWork.Modes.FilesOperating;
using CourseWork.RC6;
using Google.Protobuf;
using System;
using System.Numerics;
using System.Text;
using CourseWork.Modes;

namespace CourseWork.Main
{
    class Program
    {
        public static async Task Main()
        {
            var filePath = "C:\\Users\\Andrey\\Documents\\3.txt";
            var decryptedFilePath = "C:\\Users\\Andrey\\Documents\\3decrypted.txt";
            var fileBytes = File.ReadAllBytes(filePath);
            //var cipher = new Cipher(EncryptionMode.CBC);
            //var encryptedBytes = cipher.Encrypt(filePath);
            //var decryptedBytes = cipher.Decrypt(encryptedBytes);
           // File.WriteAllBytes(decryptedFilePath, decryptedBytes);
            Console.WriteLine("encrypted");
        }
    }
}
