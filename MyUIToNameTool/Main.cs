using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace MyUIToNameTool
{
    public class Main
    {
        private const string inStart = @"in=", outStart = @"out=", tableDir = @"tableData";
        private const string splitLine = "=====";

        public void Start()
        {
            ReadConfig();
        }


        public void ReadConfig()
        {
            string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.txt");

            StreamReader sr;
            if (File.Exists(configPath))
            {
                sr = new StreamReader(configPath);
            }
            else
            {
                var sw = File.CreateText(configPath);
                sw.WriteLine(inStart);
                sw.WriteLine(outStart);
                sw.Dispose();
                sr = new StreamReader(configPath);
            }

            try
            {
                string inString = sr.ReadLine().Trim();
                if (inString.Length > 0 && inString == inStart)
                {
                    Console.WriteLine("in= 是空");
                    return;
                }
                string inPath = inString.Substring(inStart.Length);

                string outString = sr.ReadLine();
                if (outString.Length > 0 && inString.Trim() == outStart)
                {
                    Console.WriteLine("out= 是空");
                    return;
                }
                string outPath = outString.Substring(outStart.Length);

                string dir = Path.Combine(Directory.GetCurrentDirectory(), tableDir);
                string tablePath = Path.Combine(dir, outPath.Substring(outPath.LastIndexOf('\\') + 1) + ".txt");
                if (tablePath.Length <= 0)
                {
                    Console.WriteLine("tablePath 出问题了");
                    return;
                }

                Directory.CreateDirectory(dir);

                var tableDic = ReadTxt(tablePath);

                int repeatNumber=0, noNameNumber = 0, chineseNameNumber = 0;

                var repeatList = tableDic.Values.ToList().GroupBy(x => x).Where(x => x.Count() > 1).ToList();
                repeatNumber = repeatList.Count;

                var deleteFiles = new DirectoryInfo(outPath).GetFiles();
                for (int i = 0; i < deleteFiles.Length; i++)
                {
                    File.Delete(deleteFiles[i].FullName);
                }


                var files = new DirectoryInfo(inPath).GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    string fileName = files[i].Name.Split('.')[0];
                    if (!tableDic.TryGetValue(fileName, out string value))
                    {
                        chineseNameNumber++;
                        tableDic.Add(fileName, null);
                        value = fileName;
                    }
                    if (string.IsNullOrEmpty(value))
                    {
                        noNameNumber++;
                        value = fileName;
                    }
                    string filePath = Path.Combine(outPath, value + files[i].Extension);
                    files[i].CopyTo(filePath, true);
                }

                SaveTxt(tablePath, tableDic);
                System.Diagnostics.Process.Start("explorer.exe", outPath);

                Console.WriteLine("完成了!!!重复的:{0}    没有名字的:{1}   中文的:{2}"
                    , repeatNumber , noNameNumber , chineseNameNumber );
                foreach(var item in repeatList)
                {
                    Console.WriteLine(item);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private Dictionary<string, string> ReadTxt(string filename)
        {
            Queue<string> queue = new Queue<string>();
            Dictionary<string, string> dic = new Dictionary<string, string>();

            if (!File.Exists(filename))
            {
                var sw = File.CreateText(filename);
                sw.WriteLine(splitLine);
                sw.Dispose();
            }

            var sr = File.OpenText(filename);
            string data;
            bool isStart = true;
            while ((data = sr.ReadLine()) != null)
            {
                data.Trim();
                if (data == splitLine)
                {
                    isStart = false;
                }
                else if (isStart)
                {
                    if (!string.IsNullOrEmpty(data))
                    {
                        queue.Enqueue(data.ToLower().Replace(" ", ""));
                    }
                }
                else
                {
                    var kd = data.Split('=');
                    if (kd.Length == 1)
                    {
                        dic.Add(kd[0], null);
                    }
                    else
                    {
                        dic.Add(kd[0], kd[1]);
                    }
                }
            }


            for (int i = 0; i < dic.Keys.Count; i++)
            {
                if (queue.Count == 0)
                {
                    break;
                }
                var value = queue.Dequeue();
                dic[dic.Keys.ElementAt(i)] = value;
            }
            sr.Dispose();
            return dic;
        }

        private void SaveTxt(string filename, Dictionary<string, string> dic)
        {
            var sw = File.CreateText(filename);
            sw.WriteLine(splitLine);
            foreach (var item in dic)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    sw.WriteLine(item.Key);
                }
                else
                {
                    sw.WriteLine(item.Key + "=" + item.Value);
                }
            }
            sw.Dispose();
        }
    }
}
