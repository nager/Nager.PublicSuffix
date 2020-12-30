using System;
using System.Diagnostics;
using System.Threading;

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
            Console.WriteLine("Performance (UriNormalization)");
            Console.WriteLine("------------------------------");
            Performance(true);

            Console.WriteLine();
            Console.WriteLine("Performance (IdnMappingNormalization)");
            Console.WriteLine("------------------------------");
            Performance(false);

            Console.WriteLine();
            Console.WriteLine("----------- DONE -------------");
            Console.ReadLine();
        }

        public static void LoadFromWeb()
        {
            var webTldRuleProvider = new WebTldRuleProvider(cacheProvider: new FileCacheProvider(cacheTimeToLive: new TimeSpan(0, 0, 10)));

            var domainParser = new DomainParser(webTldRuleProvider);
            for (var i = 0; i < 20; i++)
            {
                var isValid = webTldRuleProvider.CacheProvider.IsCacheValid();
                if (!isValid)
                {
                    webTldRuleProvider.BuildAsync().GetAwaiter().GetResult(); //Reload data
                    Console.WriteLine("Reload data");
                }

                var domainInfo = domainParser.Parse($"sub{i}.test.co.uk");

                Console.WriteLine("RegistrableDomain:{0}", domainInfo.RegistrableDomain);
                Console.WriteLine("SubDomain:{0}", domainInfo.SubDomain);
                Thread.Sleep(1000);
            }
        }

        public static void LoadFromFile()
        {
            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"));
            var domainInfo = domainParser.Parse("sub.test.co.uk");

            Console.WriteLine("RegistrableDomain:{0}", domainInfo.RegistrableDomain);
            Console.WriteLine("SubDomain:{0}", domainInfo.SubDomain);
        }

        public static void Performance(bool useUriNormalization)
        {
            IDomainNormalizer normalizer;

            if (useUriNormalization)
            {
                normalizer = new UriNormalizer();
            }
            else
            {
                normalizer = new IdnMappingNormalizer();
            }

            var domainParser = new DomainParser(new FileTldRuleProvider("effective_tld_names.dat"), normalizer);

            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i< 100000; i++)
            {
                var domainInfo = domainParser.Parse($"sub{i}.test.co.uk");
            }
            sw.Stop();

            Console.WriteLine("Elapsed:{0}ms", sw.Elapsed.TotalMilliseconds);
        }
    }
}
