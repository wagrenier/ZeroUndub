using System;
using System.Diagnostics;
using System.IO;

namespace ZeroUndubProcess
{
    public static class ExternalProcess
    {
        public static void PssSwitchAudio(int fileId, string folder)
        {
            folder = folder.Replace("/", "\\");
            PssDemux(fileId, "eu", folder);
            PssDemux(fileId, "jp", folder);
            PssMux(fileId, folder);
        }

        public static void PssDemux(int fileId, string region, string folder)
        {
            var args =
                $"D /N \"{folder}\\{fileId}_{region}.PSS\" \"{folder}\\{fileId}_{region}.M2V\" \"{folder}\\{fileId}_{region}.WAV\"";

            var process = new Process
            {
                StartInfo =
                {
                    FileName = "PSS_Plex.exe",
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    Arguments = args
                }
            };

            process.Start();

            if (!process.WaitForExit(10000))
            {
                try
                {
                    process.Kill(true);
                    Console.WriteLine($"Extracting audio file: {fileId} took too long, it was killed!");
                }
                catch (InvalidOperationException)
                {
                }
            }
            
            File.Delete($"{folder}/{fileId}_{region}.PSS");
        }

        public static void PssMux(int fileId, string folder)
        {
            var args = $"M /N \"{folder}\\{fileId}_eu.M2V\" \"{folder}\\{fileId}_jp.WAV\" \"{folder}\\{fileId}.PSS\"";

            var process = new Process
            {
                StartInfo =
                {
                    FileName = "PSS_Plex.exe",
                    UseShellExecute = false,
                    CreateNoWindow = false,
                    Arguments = args
                }
            };

            process.Start();

            if (!process.WaitForExit(10000))
            {
                try
                {
                    process.Kill(true);
                    Console.WriteLine($"Extracting video file: {fileId} took too long, it was killed!");
                }
                catch (InvalidOperationException)
                {
                }
            }
                

            File.Delete($"{folder}/{fileId}_eu.WAV");
            File.Delete($"{folder}/{fileId}_jp.WAV");
            File.Delete($"{folder}/{fileId}_eu.M2V");
            File.Delete($"{folder}/{fileId}_jp.M2V");
        }
    }
}