using Microsoft.Win32;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DarkColorWatcher
{
    public partial class MainForm : Form
    {
        const string REGNAME = @"SOFTWARE\Microsoft\Windows\DWM";
        const string REGVAL = "AccentColor";
        const string REGVAL2 = "AccentColorInactive";
        const string REGVAL3 = "ColorPrevalence";
        uint Color1 = 0x010101;
        uint Color2 = 0x2b2b2b;

        bool listen = true;

        Icon fallbackIcon;

        public MainForm()
        {
            InitializeComponent();

            Opacity = 0;

            Load += Form1_Load;
            Shown += Form1_Shown;

            using (var fbIcoBm = new Bitmap(16, 16))
            {
                using (var g = Graphics.FromImage(fbIcoBm))
                    g.Clear(Color.DimGray);
                var hIco = fbIcoBm.GetHicon();
                fallbackIcon = Icon.FromHandle(hIco);
            }

            Icon ico;
            if (Native.GetScalingFactor() > 1f)
                ico = SysIcons.GetSystemIcon(SysIcons.SHSTOCKICONID.SIID_FIND, SysIcons.IconSize.Large);
            else
                ico = SysIcons.GetSystemIcon(SysIcons.SHSTOCKICONID.SIID_FIND, SysIcons.IconSize.Small);

            if (ico != null)
                notifyIcon1.Icon = ico;
            else
                notifyIcon1.Icon = fallbackIcon;

            LoadSettings();

            if (!ReadColorize() && MessageBox.Show("To make this tool work, you need to enable \"Show color on Title bars\". Click \"Yes\" if you want to enable this setting now.", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                WriteColorize(true);
        }

        private void LoadSettings()
        {
            SettingsManager.Load();

            Color1 = ColorToInt(SettingsManager.Color1);
            Color2 = ColorToInt(SettingsManager.Color2);
        }

        private void SetDarkMode()
        {
            Native.SetPrefferDarkMode(true);
            Native.UseImmersiveDarkModeColors(Handle, true);
            Native.UseImmersiveDarkMode(Handle, true);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            Hide();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var m = new RegistryMonitor(RegistryHive.CurrentUser, REGNAME);
            m.RegChanged += M_RegChanged;
            m.Start();
            SetVal();

            SetDarkMode();

            notifyIcon1.ContextMenu = contextMenu1;
            ContextMenu = contextMenu1;
        }

        private void M_RegChanged(object sender, EventArgs e)
        {
            if (listen)
                SetVal();
        }

        private void WriteColorize(bool value)
        {
            using (var b = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var k = b.OpenSubKey(REGNAME, true))
            {
                k.SetValue(REGVAL3, value ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        private bool ReadColorize()
        {
            using (var b = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var k = b.OpenSubKey(REGNAME))
            {
                var v = k.GetValue(REGVAL3, 0);
                if (!(v is int))
                    return false;
                var val = (int)v;
                return val == 1;
            }
        }

        private void SetVal()
        {
            listen = false;
            using (var b = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64))
            using (var k = b.OpenSubKey(REGNAME, true))
            {
                k.SetValue(REGVAL, Color1, RegistryValueKind.DWord);
                k.SetValue(REGVAL2, Color2, RegistryValueKind.DWord);
            }
            listen = true;
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            SetVal();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            var afs = Application.OpenForms.OfType<AboutBox>();
            if (afs.Any())
                afs.ElementAt(0).ShowMe();
            else
                using (var a = new AboutBox())
                    a.ShowDialog();
        }

        private void menuItem4_Click(object sender, EventArgs e)
        {
            Close();
        }

        private uint ColorToInt(Color color)
        {
            return (uint)(((color.B & 0xff) << 16) + ((color.G & 0xff) << 8) + (color.R & 0xff));
        }

        private void menuItem5_Click(object sender, EventArgs e)
        {
            LoadSettings();
            SetVal();
        }
    }
}
