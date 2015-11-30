using System;
using System.Collections.Generic;
using Mono.Options;

namespace lhm.net.commands
{
    class Program
    {
        static void Main(string[] args)
        {
            var showHelp = false;
            var options = new LhmCommandOptions();

            var p = new OptionSet {
                { "cleanup", "Run cleanup command", v => options.RunCleanUp = v != null},
                { "whatif", "Actually run the cleanup command or just output what would happen", v => options.WhatIf = v == null},
                { "con|connection=", "Database connection to execute command for", v => options.DatabaseConnection = v},
                { "h|help",  "Show help information about the lhm.net.exe", v => showHelp = v != null }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);

                ValidateOptions(options);

                if (options.RunCleanUp)
                {
                    Lhm.Setup(options.DatabaseConnection);
                    Lhm.CleanUp(!options.WhatIf);
                }
            }
            catch (OptionException e)
            {
                Console.Write("Lhm.net: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `Lhm.net --help' for more information.");
                return;
            }
            catch (Exception e)
            {
                Console.Write("Lhm.net: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `Lhm.net --help' for more information.");
                return;
            }

            if (!showHelp) return;
            ShowHelp(p);
        }

        private static void ValidateOptions(LhmCommandOptions lhmCommandOptions)
        {
            if(string.IsNullOrWhiteSpace(lhmCommandOptions.DatabaseConnection))
                throw new Exception("Lhm.net connection required");
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: Lhm.net [OPTIONS]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }

    public class LhmCommandOptions
    {
        public bool RunCleanUp { get; set; }

        public string DatabaseConnection { get; set; }

        public bool WhatIf { get; set; }
    }
}
