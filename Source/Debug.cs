using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace LarsOfTheStars.Source
{
    public class Debug
    {
        public static List<double> SweepTimes = new List<double>();
        public static List<int> SweepLoads = new List<int>();
        public static List<double> FrameRates = new List<double>();
        public static List<double> FrameTimes = new List<double>();
        public static int SnapshotDemoninator = 1;
        public static double LastSweepTime;
        public static int LastSweepLoad = 0;
        public static double AverageSweepTime = 0;
        public static double AverageSweepLoad = 0;
        public static double AverageFrameRate = 0;
        public static double AverageFrameTime = 0;
        public static double LastFrameTime = 0;
        public static void Initialize()
        {
            Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "LarsOfTheStars"));
            Stream stream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "LarsOfTheStars", "Debug.log"), FileMode.OpenOrCreate);
            stream = GenerateSystemInfo(stream);
        }
        public static void LogSnapshot()
        {
            Stream stream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "LarsOfTheStars", string.Format("{0}-{1}-{2}_{3}.{4}.{5}.log", DateTime.Now.Year.ToString().PadLeft(2, '0'), DateTime.Now.Month.ToString().PadLeft(2, '0'), DateTime.Now.Day.ToString().PadLeft(2, '0'), DateTime.Now.Hour.ToString().PadLeft(2, '0'), DateTime.Now.Minute.ToString().PadLeft(2, '0'), DateTime.Now.Second.ToString().PadLeft(2, '0'))), FileMode.OpenOrCreate);
            stream = GenerateSnapshot(stream);
        }
        public static Stream GenerateSnapshot(Stream output)
        {
            Process proc = Process.GetCurrentProcess();
            StreamWriter writer = new StreamWriter(output);
            writer.WriteLine("====\tPROC INFO");
            writer.WriteLine("Used Memory:\t\t" + Math.Round(proc.PrivateMemorySize64 / 1000000.0, 2)  + " MB");
            writer.WriteLine("Total CPU Time:\t\t" + proc.TotalProcessorTime.Seconds + "s");
            writer.WriteLine("Current Frame Rate:\t" + (decimal)(Game.FramesPerSecond));
            writer.WriteLine("Current Sweep Time:\t" + (decimal)(LastSweepTime) + "s");
            writer.WriteLine("Current Sweep Load:\t" + (decimal)(LastSweepLoad));
            writer.WriteLine("Current Frame Time:\t" + (decimal)(LastFrameTime) + "s");
            writer.WriteLine("Average Sweep Time:\t" + (decimal)(AverageSweepTime) + "s");
            writer.WriteLine("Average Sweep Load:\t" + (decimal)(AverageSweepLoad));
            writer.WriteLine("Average Frame Time:\t" + (decimal)(AverageFrameTime) + "s");
            writer.WriteLine("Average Frame Rate:\t" + (decimal)(AverageFrameRate) + "/s");
            writer.WriteLine("====\tGAME INFO");
            writer.WriteLine("Client Count:\t\t" + Game.ClientEntities.Count);
            writer.WriteLine("Server Count:\t\t" + Game.ServerEntities.Count);
            writer.Flush();
            writer.Close();
            return output;
        }
        public static Stream GenerateSystemInfo(Stream output)
        {
            ManagementObjectSearcher searcher;
            ManagementObjectCollection results;
            StreamWriter writer = new StreamWriter(output);
            writer.WriteLine("====\tUSER INFO");
            writer.WriteLine("Username:\t\t" + Environment.UserName);
            writer.WriteLine("Machine:\t\t" + Environment.MachineName);
            writer.WriteLine("Path:\t\t\t" + Environment.CurrentDirectory);
            writer.WriteLine("Framework:\t\t" + Environment.Version);
            writer.WriteLine("====\tSYSTEM INFO");
            writer.WriteLine(GetOS());
            writer.WriteLine("====\tCPU INFO");
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"); results = searcher.Get();
            foreach (ManagementObject result in results)
            {
                writer.WriteLine("Name:\t\t\t" + Convert.ToString(result["Name"]));
                writer.WriteLine("Speed:\t\t\t" + Convert.ToInt32(result["MaxClockSpeed"]) / 1000.0 + " GHz");
            }
            writer.WriteLine("Architecture:\t\t" + (Environment.Is64BitOperatingSystem ? "x64" : "x86"));
            writer.WriteLine("Cores:\t\t\t" + Environment.ProcessorCount);
            writer.WriteLine("====\tMEMORY INFO");
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"); results = searcher.Get();
            foreach (ManagementObject result in results)
            {
                writer.WriteLine("Total RAM:\t\t" + Math.Round(Convert.ToInt64(result["TotalVisibleMemorySize"]) / 1000000.0, 2) + " GB");
                writer.WriteLine("Available RAM:\t\t" + Math.Round(Convert.ToInt64(result["FreePhysicalMemory"]) / 1000000.0, 2) + " GB");
            }
            writer.WriteLine("Drives:");
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                writer.WriteLine("\t" + drive.Name + " (Type: " + drive.DriveType + " - " + (drive.IsReady ? drive.DriveFormat : "NULL") + ")");
                if (drive.IsReady)
                {
                    writer.WriteLine("\tTotal:\t\t" + Math.Round(drive.TotalSize / 1000000000.0, 2) + " GB");
                    writer.WriteLine("\tAvailable:\t" + Math.Round(drive.TotalFreeSpace / 1000000000.0, 2) + " GB");
                }
                else
                {
                    writer.WriteLine("\tTotal:\t\t0 GB");
                    writer.WriteLine("\tAvailable:\t0 GB");
                }
            }
            writer.WriteLine("====\tGRAPHICS INFO");
            searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"); results = searcher.Get();
            foreach (ManagementObject result in results)
            {
                writer.WriteLine("Name:\t\t\t" + Convert.ToString(result["Name"]));
                writer.WriteLine("Memory:\t\t\t" + Math.Round(Convert.ToInt64(result["AdapterRAM"]) / 1000000000.0, 2) + " GB");
            }
            writer.Flush();
            writer.Close();
            return output;
        }
        public static string GetOS()
        {
            OperatingSystem system = Environment.OSVersion;
            string os = "Operating System:\r\n";
            os += "\tPlatform:\t" + system.Platform.ToString() + "\r\n";
            os += "\tVersion:\t" + system.VersionString;
            return os;
        }
    }
}
