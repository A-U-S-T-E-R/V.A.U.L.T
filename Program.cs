using System;
using System.IO;
using System.Text;

namespace Vault_Encryptor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("V.A.U.L.T Advanced Encryptor - v.0.1-alpha | CORE MODE / CONSOLE\n");

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            while (true)
            {
                Console.Write("\nPlease enter command> ");
                string command = Console.ReadLine();
                if (command == "help") {
                    Console.WriteLine(" - help => Shows a list of every available command.");
                    Console.WriteLine(" - encrypt-file => Starts the encrypting process for a specific file on your computer.");
                    Console.WriteLine(" - encrypt-path => Starts the encrypting process for a specific filepath on your computer.");
                    Console.WriteLine(" - decrypt-file => Starts the decrypting process for a specific file on your computer.");
                    Console.WriteLine(" - decrypt-path => Starts the decrypting process for a specific filepath on your computer.");
                    Console.WriteLine(" - exit => Closes this program-session.");
                }
                else if (command == "encrypt-file")
                {
                    EncryptFile();
                }
                else if (command == "decrypt-file")
                {
                    DecryptFile();
                }
                else if (command == "encrypt-path")
                {
                    EncryptFolder();
                }
                else if (command == "decrypt-path")
                {
                    DecryptFolder();
                }
                else if (command == "exit")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("UNKNOWN COMMAND - Please try again.");
                }
            }
        }
        public static string ConsoleInput(string question)
        {
            Console.Write("\n" + question + ">");
            return Console.ReadLine();
        }

        public static void EncryptFolder(string directoryPath = "", string code = "")
        {
            if (directoryPath == "")
            {
                directoryPath = ConsoleInput("Please enter the path of the folder you want to be encrypted (WITHOUT TRAILING BACKSLASH)");
            }
            if (code == "")
            {
                code = ConsoleInput("Please enter the unique encryption key (NOTE: If you forget this, the data will be lost!)");
            }

            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                EncryptFile(file, code);
            }
        }

        public static void DecryptFolder(string directoryPath = "", string code = "")
        {
            if (directoryPath == "")
            {
                directoryPath = ConsoleInput("Please enter the path of the folder you want to be decrypted (WITHOUT TRAILING BACKSLASH)");
            }
            if (code == "")
            {
                code = ConsoleInput("Please enter the unique decryption key (NOTE: IF YOU ENTER THIS WRONG, YOUR FILES WILL BE GONE FOREVER!)");
            }

            string[] files = Directory.GetFiles(directoryPath);
            foreach (string file in files)
            {
                DecryptFile(file, code);
            }
        }

        public static void EncryptFile(string filepath = "", string code = "")
        {
            if (filepath == "")
            {
                filepath = ConsoleInput("Please enter the path of the file you want to be encrypted");
            }
            if (code == "")
            {
                code = ConsoleInput("Please enter the unique encryption key (NOTE: If you forget this, the data will be lost!)");
            }

            byte[] fileBytes = File.ReadAllBytes(filepath);
            byte[] formattedBytes = new byte[fileBytes.Length];
            for (int i = 0; i < fileBytes.Length; i++)
            {
                formattedBytes[i] = (byte)(fileBytes[i] + code[(i % code.Length)]);
            }
            File.WriteAllBytes(filepath + ".vault", formattedBytes);
            File.Delete(filepath);
        }

        public static void DecryptFile(string filepath = "", string code = "")
        {
            if (filepath == "")
            {
                filepath = ConsoleInput("Please enter the path of the file you want to be decrypted");
            }
            if (code == "")
            {
                code = ConsoleInput("Please enter the unique encryption key (NOTE: IF YOU ENTER THIS WRONG, YOUR FILES WILL BE GONE FOREVER!)");
            }

            byte[] fileBytes = File.ReadAllBytes(filepath);
            byte[] formattedBytes = new byte[fileBytes.Length];
            for (int i = 0; i < fileBytes.Length; i++)
            {
                formattedBytes[i] = (byte)(fileBytes[i] - code[(i % code.Length)]);
            }
            File.WriteAllBytes(filepath.Replace(".vault",""), formattedBytes);
            File.Delete(filepath + ".vault");
        }

        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

            // We actually have no idea what the encoding is if we reach this point, so
            // you may wish to return null instead of defaulting to ASCII
            return Encoding.ASCII;
        }
    }
}
