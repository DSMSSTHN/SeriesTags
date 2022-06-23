using SeriesTags.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Drawing;
using System.IO;

namespace SeriesTags {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public static MSEpisode? CurrentEpisode { get; set; }
        
        public MainWindow() {
            STOptions.LoadRecents();
            InitializeComponent();
        }



        #region keys
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        private const int SCREENSHOR_ID = 9000;

        //Modifiers:
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CTRL = 0x0002; //Ctrl
        //CAPS LOCK:
        private const uint VK_CAPITAL = 0x14;
        //[`~] key
        private const uint MADDE = 0xC0;
        //X_Win key
        private const uint XKey = 0x58;
        //period key
        private const uint Period = 0xBE;

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, SCREENSHOR_ID, MOD_ALT, MADDE); //Alt + `
        }
        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY && wParam.ToInt32() == SCREENSHOR_ID) {
                AddScreenshot();
            }
            return IntPtr.Zero;
        }

        protected override void OnClosed(EventArgs e) {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, SCREENSHOR_ID);
            base.OnClosed(e);
        }


        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr onj);

        private void AddScreenshot() {
            if (CurrentEpisode == null) return;

            using (Bitmap bitmap = new Bitmap(3840, 2160, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
                using (Graphics g = Graphics.FromImage(bitmap)) {
                    g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

                }
                var size = 400;
                var width = bitmap.Width;
                var height = bitmap.Height;
                //handle = bitmap.GetHbitmap();
                if (bitmap.Width > bitmap.Height) {
                    if (bitmap.Width > size) {
                        width = size;
                        var h = height * (width / (double)bitmap.Width);
                        height = (int)Math.Round(h);
                    }
                } else {
                    if (bitmap.Height > size) {
                        height = size;
                        var w = width * (height / (double)bitmap.Height);
                        width = (int)Math.Round(w);
                    }
                }
                using (var thumnailImage = bitmap.GetThumbnailImage(width, height, delegate () { return false; }, IntPtr.Zero)) {
                    ImageConverter imageConverter = new ImageConverter();
                    byte[]? bytes = imageConverter.ConvertTo(thumnailImage, typeof(byte[])) as byte[];
                    if (bytes == null) return;
                    var path = CurrentEpisode.PhotosPath;
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    var files = Directory.GetFiles(path).Where(p => p.EndsWith(".jpg") || p.EndsWith(".png"));
                    var max = files.Count() == 0 ? 0 : files.Select(f => {
                        if (int.TryParse(Path.GetFileNameWithoutExtension(f), out int i)) return i;
                        return 0;
                    }).Max();
                    var imagePath = Path.Combine(path, $"{max + 1}.png");
                    thumnailImage.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
                    CurrentEpisode.AddPhoto(imagePath);
                }

            }
        }
        #endregion
    }
}
