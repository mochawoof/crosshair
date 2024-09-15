using System.Diagnostics;
using System.Runtime.InteropServices;

namespace crosshair
{
    public partial class Form : System.Windows.Forms.Form
    {
        [DllImport("user32.dll", SetLastError = true)]

        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]

        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public Form()
        {
            InitializeComponent();
        }

        void setCrosshair(string path)
        {
            Properties.Settings.Default.crosshair = path;
            Properties.Settings.Default.Save();
        }

        void loadCrosshair()
        {
            try
            {
                pictureBox.Image = new Bitmap(Properties.Settings.Default.crosshair);
            } catch
            {
                setCrosshair(null);
            }
        }

        void keyPressed(object sender, KeyPressedEventArgs e)
        {
            if (e.Key == Keys.Q)
            {
                Application.Exit();
            }
            else if (e.Key == Keys.C)
            {
                openFileDialog.InitialDirectory = Properties.Settings.Default.crosshair;

                pictureBox.Image = null;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    setCrosshair(openFileDialog.FileName);
                }

                loadCrosshair();
            }
            else if (e.Key == Keys.H)
            {
                instructions.Visible = !instructions.Visible;
            }
        }

        private void Form_Load(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.Location = new Point(0, 0);

            pictureBox.Size = this.Size;
            pictureBox.Location = new Point(0, 0);

            instructions.Text = $@"v{Application.ProductVersion}

Press Ctrl+Shift+Q to quit.

Press Ctrl+Shift+C to change your crosshair.

Press Ctrl+Shift+H to hide this.";

            loadCrosshair();
            this.TopMost = true;

            int initialstyle = GetWindowLong(this.Handle, -20);

            SetWindowLong(this.Handle, -20, initialstyle | 0x80000 | 0x20);

            KeyboardHook hook = new KeyboardHook();
            hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(keyPressed);

            ModifierKeys triggerKeys = global::ModifierKeys.Control | global::ModifierKeys.Shift;
            hook.RegisterHotKey(triggerKeys, Keys.Q);
            hook.RegisterHotKey(triggerKeys, Keys.C);
            hook.RegisterHotKey(triggerKeys, Keys.H);
        }
    }
}
