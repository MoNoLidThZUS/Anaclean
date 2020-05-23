using System;
using System.IO;
using System.Collections.Generic;

namespace Anaclean
{
    class Program
    {
        const string defaultinput = "delscript.txt";
        const string defaultoutput = "fetusdeletus.log";
        const string BS = "\\";

        private static List<string> delQuery;
        private static List<string> allfiles;
        private static List<string> matchedfiles;
        private static string wd;
        static void Main(string[] args)
        {
            delQuery = new List<string>();
            allfiles = new List<string>();
            matchedfiles = new List<string>();
            wd = Directory.GetCurrentDirectory();
            Console.WriteLine("Starting AnaClean Skywalker");
            Console.WriteLine("Root directory: "+wd);
            try
            {
                //start read filescript
                if (args.Length == 0)
                {
                    Console.WriteLine("Using default "+defaultinput+" scriptfile");
                    readScript();
                }
                else
                {
                    //assume we can inพุทธ multiple DelScripts
                    foreach (string arg in args)
                    {
                        readScript(arg);
                    }
                }
                Console.WriteLine("ReadScript initialized: " + delQuery.Count+" query processed.");
                //populate dirdata
                readDir(wd);
                Console.WriteLine("Populate DirData: "+ allfiles.Count+" files in directory");
                //compare filenames
                checkFiles();
                Console.WriteLine("Matched files: "+ matchedfiles.Count+" entries");
                //confirm user action
                if (confirmAction())
                {
                    commitDelete();
                }
                else
                {
                    Console.WriteLine("User aborted operation.");
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }

            Console.WriteLine("AnaClean is complete.");
            Console.ReadKey();
        }

        static bool confirmAction()
        {
            Console.WriteLine("Warning: This software WILL REMOVE ANY FILES WITH MATCHING NAMES IN scriptfile");
            do
            {
                Console.Write("Confirm Action? <Y/N>: "); 
                char cfm = Console.ReadKey().KeyChar;
                Console.WriteLine();
                if (cfm.Equals('Y') || cfm.Equals('y')) return true;
                if (cfm.Equals('N') || cfm.Equals('n')) return false;
            } while (true);
            
        }
        static void readScript(string path = null)
        {
            if (path == null) path = defaultinput;
            if (File.Exists(path))
            {
                StreamReader fs = new StreamReader(path);
                string line;
                while ((line = fs.ReadLine()) != null)
                {
                    if (line.StartsWith('#')) continue; //skip if filename starts with # 
                    delQuery.Add(line);
                }
            }
            else
            {
                Console.WriteLine("Cannot locate scriptfile "+path);
            }
        }
        static void readDir(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                    {
                        foreach (string f in Directory.GetFiles(d))
                        {
                            allfiles.Add(f);
                        }
                        readDir(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.Error.WriteLine(excpt.Message);
            }
        }
        static void checkFiles()
        {
            foreach (string item in allfiles)
            {
                checkFile(item);
            }
        }
        static void checkFile(string fn)
        {
            foreach (string scd in delQuery)
            {
                if (fn.EndsWith(scd))
                {
                    matchedfiles.Add(fn);
                }
            }
        }
        static void commitDelete()
        {
            Console.WriteLine("*THANOS SNAP*");
            int i = 0;
            foreach (string nf in matchedfiles)
            {
                try
                {
                    Console.Write(nf);
                    File.Delete(nf);
                    Console.WriteLine("  ...DELETED!");
                    i++;
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine("Delete Error: "+e.Message);
                }
                
            }
            Console.WriteLine("Deleted " + i+" files");
        }
    }
}
