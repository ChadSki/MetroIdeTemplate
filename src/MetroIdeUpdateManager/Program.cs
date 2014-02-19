﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;

namespace MetroIdeUpdateManager
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.Error.WriteLine("Error: not enough arguments");
                Console.Error.WriteLine("Usage: MetroIdeUpdateManager <update zip> <metroide exe> <parent pid>");
                return;
            }
            string zipPath = args[0];
            string exePath = args[1];
            int pid = Convert.ToInt32(args[2]);

            try
            {
                // Wait for Assembly to close
                try
                {
                    Process process = Process.GetProcessById(pid);
                    process.WaitForExit();
                    process.Close();
                }
                catch
                {
                }

                // Extract the update zip
                var fz = new FastZip {CreateEmptyDirectories = true};
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        fz.ExtractZip(zipPath, Directory.GetCurrentDirectory(), null);
                        break;
                    }
                    catch (IOException)
                    {
                        Thread.Sleep(1000);
                        if (i == 4)
                        {
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Assembly Update Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                File.Delete(zipPath);
            }
            catch
            {
            }

            // Launch "The New iPa... Assembly"
            Process.Start("Assembly://post-update");
        }
    }
}