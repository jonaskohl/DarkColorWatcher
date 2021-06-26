using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DarkColorWatcher
{
    static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);
        private const int IDC_HAND = 32649;
        private static Cursor SystemHandCursor;

        private static void ApplyHandCursorFix()
        {
            try
            {
                SystemHandCursor = new Cursor(LoadCursor(IntPtr.Zero, IDC_HAND));

                typeof(Cursors).GetField("hand", BindingFlags.Static | BindingFlags.NonPublic)
                               .SetValue(null, SystemHandCursor);
            }
            catch { }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            using (var mutex = new Mutex(false, "9e3d0b10-43ed-49fb-b7a3-929ce399a17e"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("Another instance of DarkColorWatcher is already open", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ApplyHandCursorFix();

                Application.Run(new MainForm());
            }
        }
    }
}
