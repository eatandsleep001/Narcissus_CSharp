using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NarcissusNamespace
{
    class Narcissus
    {
        private Uri uri = null;
        private int totalView = 0;
        private int success = 0;
        private int countView = 0;
        private Mutex mutex = null;

        public static string ReadAllTextInFile(string Filename)
        {
            string result = null;
            FileStream fileStream = new FileStream(Filename, FileMode.Open, FileAccess.Read);

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                result = streamReader.ReadToEnd();

            return result;
        }

        public static string RandomString(string Characters, int Length)
        {
            Random random = new Random();
            return new string(Enumerable.Repeat(Characters, Length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public Narcissus(string Url, int TotalView)
        {
            if (!Uri.TryCreate(Url, UriKind.Absolute, out this.uri))
            {
                Console.WriteLine("Cannot create Uri");
            }

            this.totalView = Math.Abs(TotalView);
            this.success = 0;
            this.countView = 0;

            this.mutex = new Mutex();
        }

        private CookieContainer GetCookieContainer()
        {
            CookieContainer result = new CookieContainer();

            result.Add(this.uri, new Cookie(@"314159265358979", @"314159265358979"));
            result.Add(this.uri, new Cookie(@"__cfduid",
                Narcissus.RandomString(@"abcdefghijklmnopqrstuvwxyz0123456789", 43)));
            result.Add(this.uri, new Cookie(@"location.href", @"1"));

            return result;
        }

        private HttpStatusCode Get(CookieContainer CookieContainer, int Index)
        {
            HttpStatusCode result = 0;
            HttpWebRequest httpWebRequest = null;
            HttpWebResponse httpWebResponse = null;

            try
            {
                httpWebRequest = (HttpWebRequest)WebRequest.Create(this.uri);
                httpWebRequest.Accept = @"text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                httpWebRequest.CookieContainer = CookieContainer;
                httpWebRequest.Headers.Add(@"Accept-Language", @"vi-VN,vi;q=0.8,fr-FR;q=0.6,fr;q=0.4,en-US;q=0.2,en;q=0.2");
                httpWebRequest.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.82 Safari/537.36 " + Index;

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            catch (Exception ex) { }

            if (httpWebResponse != null)
            {
                result = httpWebResponse.StatusCode;
            }

            if (httpWebResponse != null)
            {
                httpWebResponse.Close();
            }

            return result;
        }

        private void Do(int threadID)
        {
            HttpStatusCode httpStatusCode;
            CookieContainer cookieContainer = this.GetCookieContainer();
            Random random = new Random();

            while (true)
            {
                httpStatusCode = this.Get(cookieContainer, random.Next(1000000000));

                this.mutex.WaitOne();

                this.countView++;

                if (httpStatusCode == HttpStatusCode.OK)
                {
                    this.success++;
                }

                Console.WriteLine(
                    string.Format("Thread {0,3}:", threadID).PadRight(15, ' ') +
                    string.Format("{0,5}|{1,0}|{2,0}", this.success, this.countView, httpStatusCode));

                if (this.countView >= this.totalView)
                {
                    this.mutex.ReleaseMutex();
                    break;
                }

                this.mutex.ReleaseMutex();
            }
        }

        public void Run()
        {
            List<Thread> threads = new List<Thread>();
            int threadCount = 1;
            IniFile iniFile = new IniFile(@"Settings.ini");

            int.TryParse(iniFile.Read(@"Threads", @"Settings"), out threadCount);

            if (threadCount > this.totalView)
            {
                threadCount = this.totalView;
            }

            Console.Title += @" " + this.uri.AbsoluteUri;

            for (int i = 0; i < threadCount; i++)
            {
                threads.Add(new Thread(delegate () { this.Do(i); }));
                threads[i].Start();
            }
        }
    }
}
