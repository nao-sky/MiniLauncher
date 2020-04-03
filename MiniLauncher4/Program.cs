using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace MiniLauncher4
{
    static class Program
    {
        static NotifyIcon notify;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            EasyCryptStream.Salt = EdefName;

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new DummyForm();

            notify = new NotifyIcon(form.components);
            notify.Text = "test";
            notify.Icon = new System.Drawing.Icon("app.ico");
            notify.Visible = true;

            //notify.Click += ShowMenu;

            RootMenuCreate(notify);

            Application.Run();

        }
        static void ShowMenu(object sender, EventArgs args)
        {
            if (sender is NotifyIcon notify)
            {
                notify.ContextMenuStrip.Show();
            }
        }
        static void DefFileChanged(object sender, EventArgs args)
        {

        }


        private static ToolStripMenuItem folder1;
        private static ToolStripMenuItem folder2;

        static void RootMenuCreate(NotifyIcon notify)
        {
            notify.ContextMenuStrip = new ContextMenuStrip();

            var root = notify.ContextMenuStrip;

            root.SuspendLayout();

            root.Items.Clear();

            folder1 = new ToolStripMenuItem();

            folder1.Text = "Execute Pronpt";

            root.Items.Add(folder1);

            root.Items.Add("-");

            folder2 = new ToolStripMenuItem();

            folder2.Text = "Execute";

            root.Items.Add(folder2);

            root.Items.Add("-");

            var m1 = new ToolStripMenuItem();

            m1.Text = "Open Settings";
            m1.Click += settings_Click;

            root.Items.Add(m1);

            var m2 = new ToolStripMenuItem();
            m2.Text = "Exit";

            m2.Click += Exit_Click;

            root.Items.Add(m2);

            LoadMenuItems();


            root.ResumeLayout();

        }

        static void CommandExecute(LaunchInfo info, bool useWindow)
        {
            if (useWindow)
            {
                try
                {
                    Process process = new Process();
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\CmdExecuter.exe";
                    processStartInfo.Arguments = string.Join(' ', info.ExecutePath, info.Argument);
                    processStartInfo.UseShellExecute = true;
                    processStartInfo.CreateNoWindow = false;
                    processStartInfo.WorkingDirectory = info.WorkingPath;

                    process.StartInfo = processStartInfo;
                    process.Start();
                }
                catch(Exception ex)
                {
                }


            }
            else
            {
                try
                {
                    Process process = new Process();
                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                    processStartInfo.FileName = info.ExecutePath;
                    processStartInfo.Arguments = info.Argument;
                    processStartInfo.UseShellExecute = true;
                    processStartInfo.CreateNoWindow = !useWindow;
                    processStartInfo.WorkingDirectory = info.WorkingPath;

                    process.StartInfo = processStartInfo;
                    process.Start();
                }
                catch(Exception ex)
                {
                    try
                    {
                        var err = new ErrorForm();
                        err.ShowMessage(info, ex);
                    }
                    catch { }
                }
            }
        }


        static void settings_Click(object sender, EventArgs args)
        {
            using var st = new SettingForm();

            {
                using FileStream fs = new FileStream(EdefName, FileMode.OpenOrCreate);
                using var es = new EasyCryptStream(fs);

                StreamReader sr = new StreamReader(es, new UTF8Encoding());

                st.TextValue = sr.ReadToEnd();

                st.ShowDialog();
                
                es.Position = 0;

                var buf = new UTF8Encoding().GetBytes(st.TextValue);

                if(buf.Length == 0)
                {
                    buf = new UTF8Encoding().GetBytes("#\r\n"+
                                                        "# exsample\r\n" +
                                                        "#\r\n" +
                                                        "#[name]\r\n" +
                                                        "#EXECUTEFILE=\r\n" +
                                                        "#ARGUMENT=\r\n" +
                                                        "#WORKINGPATH=\r\n"
                                                        );
                }
                es.Write(buf);
                es.SetLength(buf.LongLength);

                es.Flush();
            }

            LoadMenuItems();
        }

        static void Exit_Click(object sender, EventArgs args)
        {
            notify.Dispose();
            Application.Exit();
        }

        static void LoadMenuItems()
        {

            var def = Definition.Get(EdefName);
            
            if(def != null)
            {
                folder1.DropDownItems.Clear();
                folder2.DropDownItems.Clear();

                foreach (var key in def.LaunchInfoes.Keys)
                {
                    var info = def.LaunchInfoes[key];

                    var menu1 = new ToolStripMenuItem();
                    menu1.Text = info.Name;
                    menu1.Tag = info;
                    menu1.Click += MenuItem1_Click;

                    folder1.DropDownItems.Add(menu1);

                    var menu2 = new ToolStripMenuItem();
                    menu2.Text = info.Name;
                    menu2.Tag = info;
                    menu2.Click += MenuItem2_Click;

                    folder2.DropDownItems.Add(menu2);
                }


            }
            if(folder1.DropDownItems.Count< 1)
            {
                folder1.DropDownItems.Add("setting error");
                folder2.DropDownItems.Add("setting error");
            }
        }

        static void MenuItem1_Click(object sender, EventArgs args)
        {
            if (sender is ToolStripMenuItem tool)
            {
                if (tool.Tag is LaunchInfo info)
                    CommandExecute(info, true);
            }


        }
        static void MenuItem2_Click(object sender, EventArgs args)
        {
            if (sender is ToolStripMenuItem tool)
            {
                if (tool.Tag is LaunchInfo info)
                    CommandExecute(info, false);
            }


        }

        static string EdefName
        {
            get
            {
                var exeFile = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
                var extFile = exeFile + ".edef";

                return extFile;
            }
        }
    }


}
