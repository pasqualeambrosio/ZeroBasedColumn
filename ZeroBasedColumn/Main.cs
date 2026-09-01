using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET
{
    class Main
    {
        internal const string PluginName = "ZeroBasedColumn";
        static string iniFilePath = null;
        static bool someSetting = false;
        static frmMyDlg frmMyDlg = null;
        static int idMyDlg = -1;
        static Bitmap tbBmp = Properties.Resources.star;
        static Bitmap tbBmp_tbTab = Properties.Resources.star_bmp;
        static Icon tbIcon = null;

        // --- Zero-based column ---
        static int idZeroBasedCol = -1;
        static bool zeroBasedCol = false;
        static IntPtr statusBarHandle = IntPtr.Zero;
        static Timer zeroColTimer = null;
        

        const uint SB_GETTEXTLENGTHW = 0x40C;
        const uint SB_GETTEXTW = 0x40D;
        const uint SB_SETTEXTW = 0x40B;
        const int STATUSBAR_CUR_POS = 2; // parte "Ln:.. Col:.. Pos:.."

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessageGetText(IntPtr hWnd, uint msg, IntPtr wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessageSetText(IntPtr hWnd, uint msg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SendMessage")]
        static extern IntPtr SendMessageLen(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumChildWindows(IntPtr hWndParent, EnumChildProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        delegate bool EnumChildProc(IntPtr hWnd, IntPtr lParam);

        public static void OnNotification(ScNotification notification)
        {
            try
            {
                if (notification.Header.Code == (uint)SciMsg.SCN_UPDATEUI)
                {
                    UpdateColumnDisplay();
                }
            }
            catch
            {
            }
        }

        internal static void CommandMenuInit()
        {
            StringBuilder sbIniFilePath = new StringBuilder(Win32.MAX_PATH);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETPLUGINSCONFIGDIR, Win32.MAX_PATH, sbIniFilePath);
            iniFilePath = sbIniFilePath.ToString();
            if (!Directory.Exists(iniFilePath)) Directory.CreateDirectory(iniFilePath);
            iniFilePath = Path.Combine(iniFilePath, PluginName + ".ini");
            someSetting = (Win32.GetPrivateProfileInt("SomeSection", "SomeKey", 0, iniFilePath) != 0);

            zeroBasedCol = (Win32.GetPrivateProfileInt("Settings", "ZeroBasedCol", 0, iniFilePath) != 0);

            PluginBase.SetCommand(0, "Use zero-based column (Col-1)", ToggleZeroBasedCol,
                new ShortcutKey(false, false, false, Keys.None), zeroBasedCol);
            idZeroBasedCol = 0;

            PluginBase.SetCommand(1, "---", null); // separatore

            PluginBase.SetCommand(2, "About...", ShowAboutDialog,
                new ShortcutKey(false, false, false, Keys.None));

            zeroColTimer = new Timer();
            zeroColTimer.Interval = 86;
            zeroColTimer.Tick += (s, e) =>
            {
                try { UpdateColumnDisplay(); } catch { }
            };
            if (zeroBasedCol) zeroColTimer.Start();
        }

        internal static void ShowAboutDialog()
        {
            string message =                
                "Version 1.0\n\n" +
                "Displays the cursor column starting from 0 instead of 1.\n\n" +
                "Author: Pasquale Ambrosio\n\n\n" +
                "Click OK to open GitHub page on browser.";

            DialogResult result = MessageBox.Show(message, "About " + PluginName,
                MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            if (result == DialogResult.OK)
            {
                try
                {
                    System.Diagnostics.Process.Start("https://github.com/pasqualeambrosio/ZeroBasedColumn");
                }
                catch { /* browser non disponibile o link non valido, ignora silenziosamente */ }
            }
        }

        internal static void SetToolBarIcon()
        {
            if (idMyDlg < 0) return;

            toolbarIcons tbIcons = new toolbarIcons();
            tbIcons.hToolbarBmp = tbBmp.GetHbitmap();
            IntPtr pTbIcons = Marshal.AllocHGlobal(Marshal.SizeOf(tbIcons));
            Marshal.StructureToPtr(tbIcons, pTbIcons, false);
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_ADDTOOLBARICON, PluginBase._funcItems.Items[idMyDlg]._cmdID, pTbIcons);
            Marshal.FreeHGlobal(pTbIcons);
        }

        internal static void PluginCleanUp()
        {
            if (zeroColTimer != null) zeroColTimer.Stop();
            Win32.WritePrivateProfileString("SomeSection", "SomeKey", someSetting ? "1" : "0", iniFilePath);
            Win32.WritePrivateProfileString("Settings", "ZeroBasedCol", zeroBasedCol ? "1" : "0", iniFilePath);
        }

        internal static void myMenuFunction()
        {
            MessageBox.Show("Hello N++!");
        }

        internal static void myDockableDialog()
        {
            if (frmMyDlg == null)
            {
                frmMyDlg = new frmMyDlg();

                using (Bitmap newBmp = new Bitmap(16, 16))
                {
                    Graphics g = Graphics.FromImage(newBmp);
                    ColorMap[] colorMap = new ColorMap[1];
                    colorMap[0] = new ColorMap();
                    colorMap[0].OldColor = Color.Fuchsia;
                    colorMap[0].NewColor = Color.FromKnownColor(KnownColor.ButtonFace);
                    ImageAttributes attr = new ImageAttributes();
                    attr.SetRemapTable(colorMap);
                    g.DrawImage(tbBmp_tbTab, new Rectangle(0, 0, 16, 16), 0, 0, 16, 16, GraphicsUnit.Pixel, attr);
                    tbIcon = Icon.FromHandle(newBmp.GetHicon());
                }

                NppTbData _nppTbData = new NppTbData();
                _nppTbData.hClient = frmMyDlg.Handle;
                _nppTbData.pszName = "My dockable dialog";
                _nppTbData.dlgID = idMyDlg;
                _nppTbData.uMask = NppTbMsg.DWS_DF_CONT_RIGHT | NppTbMsg.DWS_ICONTAB | NppTbMsg.DWS_ICONBAR;
                _nppTbData.hIconTab = (uint)tbIcon.Handle;
                _nppTbData.pszModuleName = PluginName;
                IntPtr _ptrNppTbData = Marshal.AllocHGlobal(Marshal.SizeOf(_nppTbData));
                Marshal.StructureToPtr(_nppTbData, _ptrNppTbData, false);

                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMREGASDCKDLG, 0, _ptrNppTbData);
            }
            else
            {
                Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_DMMSHOW, 0, frmMyDlg.Handle);
            }
        }

        internal static void ToggleZeroBasedCol()
        {
            zeroBasedCol = !zeroBasedCol;

            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_SETMENUITEMCHECK,
                PluginBase._funcItems.Items[idZeroBasedCol]._cmdID, zeroBasedCol ? 1 : 0);

            if (zeroBasedCol)
            {
                zeroColTimer.Start();
                UpdateColumnDisplay();
            }
            else
            {
                zeroColTimer.Stop();
            }
        }

        

        static IntPtr FindStatusBarRecursive(IntPtr root)
        {
            IntPtr found = IntPtr.Zero;

            EnumChildProc callback = (hWnd, lParam) =>
            {
                StringBuilder cls = new StringBuilder(256);
                GetClassName(hWnd, cls, cls.Capacity);
                if (cls.ToString() == "msctls_statusbar32")
                {
                    found = hWnd;
                    return false;
                }
                return true;
            };

            EnumChildWindows(root, callback, IntPtr.Zero);
            return found;
        }

        static IntPtr GetStatusBarHandle()
        {
            if (statusBarHandle == IntPtr.Zero)
                statusBarHandle = FindStatusBarRecursive(PluginBase.nppData._nppHandle);
            return statusBarHandle;
        }

        
        static IntPtr GetCurrentScintilla()
        {
            int which = 0;
            Win32.SendMessage(PluginBase.nppData._nppHandle, (uint)NppMsg.NPPM_GETCURRENTSCINTILLA, 0, out which);
            return which == 0 ? PluginBase.nppData._scintillaMainHandle : PluginBase.nppData._scintillaSecondHandle;
        }

        static void UpdateColumnDisplay()
        {
            if (!zeroBasedCol) return;
            if (PluginBase.nppData._nppHandle == IntPtr.Zero) return;

            IntPtr hSb = GetStatusBarHandle();
            if (hSb == IntPtr.Zero) return;

            IntPtr sci = GetCurrentScintilla();
            if (sci == IntPtr.Zero) return;

            int pos = (int)Win32.SendMessage(sci, (uint)SciMsg.SCI_GETCURRENTPOS, 0, 0);
            int col = (int)Win32.SendMessage(sci, (uint)SciMsg.SCI_GETCOLUMN, pos, 0);

            int len = SendMessageLen(hSb, SB_GETTEXTLENGTHW, (IntPtr)STATUSBAR_CUR_POS, IntPtr.Zero).ToInt32() & 0xFFFF;
            if (len <= 0) return;

            StringBuilder sb = new StringBuilder(len + 1);
            SendMessageGetText(hSb, SB_GETTEXTW, (IntPtr)STATUSBAR_CUR_POS, sb);
            string text = sb.ToString();

            string newText = Regex.Replace(text, @"Col\s*:\s*\d+", "Col_ : " + col);

            if (newText != text)
                SendMessageSetText(hSb, SB_SETTEXTW, (IntPtr)STATUSBAR_CUR_POS, newText);
        }
    }
}