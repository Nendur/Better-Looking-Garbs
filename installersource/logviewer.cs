using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;

namespace BetterLookingGarbsInstall
{
    class LogViewer : RichTextBox
    {
        public LogViewer()
        {
            ReadOnly = true;
            Dock = DockStyle.Fill;
            BorderStyle = BorderStyle.None;
        }

        public void Open(string file)
        {
            try
            {
                StreamWriter filestream = new StreamWriter(file, false)
                {
                    AutoFlush = true
                };
                filestream.WriteLine("Running " + Process.GetCurrentProcess().MainModule.FileName + " on " + DateTime.Now);
                logfile = filestream;
            }
            catch (Exception e)
            {
                logfile = new StringWriter();
                Error(e.Message);
            }
        }

        public void Write(string text)
        {
            AppendText(text);
            AppendText("\n");
            if (logfile != null)
                logfile.WriteLine(text);
            Update();
        }

        public void WriteDebug(string text)
        {
            logfile.WriteLine(text);
        }

        public void Error(string text)
        {
            Color old = SelectionColor;
            SelectionColor = Color.Red;
            Write(text);
            SelectionColor = old;
        }

        public void WriteDLCCheck(string text, bool check)
        {
            logfile.Write(text);
            logfile.WriteLine(check ? " present." : " absent.");

            Color old = SelectionColor;
            Font font = SelectionFont;
            SelectionColor = check ? Color.Green : Color.Red;
            AppendText(check ? "\u2714 " : "\u2718 ");
            SelectionColor = old;
            SelectionFont = font;
            AppendText(text);
            AppendText(check ? " present.\n" : " absent.\n");
            Update();
        }

        public TextWriter LogStream()
        {
            return logfile;
        }

        private TextWriter logfile = null;
    }
}