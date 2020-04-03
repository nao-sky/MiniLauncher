using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace MiniLauncher4
{
    public class LaunchInfo
    {
        public readonly string Name;
        public readonly string ExecutePath;
        public readonly string Argument;
        public readonly string WorkingPath;

        public LaunchInfo(string name, string exepath, string args, string workpath)
        {
            this.Name = name;
            this.ExecutePath = exepath;
            this.Argument = args;
            this.WorkingPath = workpath;
        }
    }

    public class Definition
    {
        public Dictionary<string, LaunchInfo> LaunchInfoes { get; private set; }

        //public string ListenDomain { get; private set; }
        private string innerPath;

        private static string filePath;
        private static Definition _default;

        public static Definition Get(string path)
        {
                _default = LoadFile(path);

            return _default;
        }


        private static Definition LoadFile(string path)
        {
            var ret = new Definition(path);
            ret.Load();

            return ret;
        }

        private Definition(string path)
        {
            this.innerPath = path;
        }

        private void Load()
        {
            FileStream file = null;

            try
            {
                file = new FileStream(innerPath, FileMode.OpenOrCreate);

                var encFile = new EasyCryptStream(file);


                ReadStream(encFile);

            }
            catch(Exception ex)
            {
                //throw new Exception("unkoun erroe.",ex);
            }
            finally
            {
                file?.Close();
            }
        }

        private void ReadStream(Stream file)
        {
            try
            {
                using var sr = new StreamReader(file, new UTF8Encoding());
                var dic = new Dictionary<string, LaunchInfo>();

                var sections = Section.Split(sr.ReadToEnd());

                foreach(var sec in sections)
                {
                    string name = sec.Name;
                    string exepath = null;
                    string args = null;
                    string workpath = null;

                    foreach(var line in sec)
                    {
                        if(line.StartsWith("EXECUTEFILE"))
                        {
                            exepath = GetValue(line);
                        }
                        else if(line.StartsWith("ARGUMENT"))
                        {
                            args = GetValue(line);
                        }
                        else if(line.StartsWith("WORKINGPATH"))
                        {
                            workpath = GetValue(line);
                        }
                    }
                    if(name != null)
                        dic.Add(name, new LaunchInfo(name, exepath, args, workpath));
                }
                LaunchInfoes = dic;
            }
            catch(Exception ex)
            {
                LaunchInfoes = new Dictionary<string, LaunchInfo>();
                throw new Exception("unkoun error.", ex);
            }
        }

        private static string GetValue(string line)
        {
            return line[(line.IndexOf("=") + 1)..];
        }

        private class Section : IEnumerable<string>
        {
            public string Name { get; }

            private string Data { get; }

            public Section(string name, string data)
            {
                this.Name = name;
                this.Data = data;
            }

            public IEnumerator<string> GetEnumerator()
            {
                StringReader sr = new StringReader(Data);

                string line = sr.ReadLine();

                while(line != null)
                {
                    bool ign = line switch
                    {
                        { } x => string.IsNullOrWhiteSpace(x) || x[0] == '#',
                        _ => false
                    };

                    if (!ign)
                        yield return line;

                    line = sr.ReadLine();
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public static Section[] Split(string str)
            {
                StringReader sr = new StringReader(str);
                List<Section> ret = new List<Section>();

                string name = null;

                string line = sr.ReadLine();
                var dat = new StringBuilder();

                while(line != null)
                {
                    if(line.StartsWith("["))
                    {
                        ret.Add(new Section(name, dat.ToString()));

                        name = line;
                        dat = new StringBuilder(); 
                    }
                    else
                    {
                        dat.AppendLine(line);
                    }

                    line = sr.ReadLine();
                }

                if(dat.Length > 0)
                {
                    ret.Add(new Section(name, dat.ToString()));
                }

                return ret.ToArray();
            }
        }
    }
}
