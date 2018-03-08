﻿using System;
using System.Diagnostics;

namespace Nager.PublicSuffix.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("WebTldRuleProvider");
            Console.WriteLine("------------------------------");
            LoadFromWeb();

            Console.WriteLine();
            Console.WriteLine("FileTldRuleProvider");
            Console.WriteLine("------------------------------");
            LoadFromFile();

            Console.WriteLine();
            Console.WriteLine("Performance");
            Console.WriteLine("------------------------------");
            Performance();

            Console.WriteLine();
            Console.WriteLine("----------- DONE -------------");
            Console.ReadLine();
        }

        public static void LoadFromWeb()
        {
            var webTldRuleProvider = new WebTldRuleProvider(cacheProvider: new FileCacheProvider(cacheTimeToLive: new TimeSpan(10, 0, 0)));

            var domainParser = new DomainParser(webTldRuleProvider);
            for (var i = 0; i < 1000; i++)
            {
                var isValid = webTldRuleProvider.CacheProvider.IsCacheValid();
                if (!isValid)
                {
                    webTldRuleProvider.BuildAsync().GetAwaiter(); //Reload data
                }

                var domainInfo = domainParser.Get($"https://heb.spitball.co/item/university%20of%20michigan/113537/econ%20101/610562/econlecture6.docx/");

                Console.WriteLine("RegistrableDomain:{0}", domainInfo.RegistrableDomain);
                Console.WriteLine("SubDomain:{0}", domainInfo.SubDomain);
            }
        }

        public static void LoadFromFile()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));
            var domainInfo = domainParser.Get("sub.test.co.uk");

            Console.WriteLine("RegistrableDomain:{0}", domainInfo.RegistrableDomain);
            Console.WriteLine("SubDomain:{0}", domainInfo.SubDomain);
        }

        public static void Performance()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i< 100000; i++)
            {
                var domainInfo = domainParser.Get($"sub{i}.test.co.uk");
            }
            sw.Stop();

            Console.WriteLine("Elapsed:{0}ms", sw.Elapsed.TotalMilliseconds);
        }
    }
}
