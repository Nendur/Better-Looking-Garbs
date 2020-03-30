using System;
using System.Drawing;
using System.Windows.Forms;

namespace BetterLookingGarbsInstall
{
    class BLGWindow : Form
    {
        public BLGWindow(LogViewer log)
        {
            Text = "Better Looking Garbs installation";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(600, 700);
            Padding = new Padding(20);
            Controls.Add(log);
            Show();
        }
    }
}
