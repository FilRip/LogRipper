using System.Windows.Media;

using LogRipper.Helpers;

namespace LogRipper.Constants
{
    internal static class Icons
    {
        public static ImageSource HelpImage { get; set; }
        public static ImageSource RecycleImage { get; set; }
        public static ImageSource CancelImage { get; set; }
        public static ImageSource OkImage { get; set; }
        public static ImageSource SaveImage { get; set; }
        public static ImageSource RefreshImage { get; set; }
        public static ImageSource SearchImage { get; set; }
        public static ImageSource EditImage { get; set; }
        public static ImageSource WindowImage { get; set; }

        internal static void Init()
        {
            HelpImage = NativeMethods.GetIconFromFile("shell32.dll", 221);
            RecycleImage = NativeMethods.GetIconFromFile("shell32.dll", 31);
            CancelImage = NativeMethods.GetIconFromFile("imageres.dll", 100);
            OkImage = NativeMethods.GetIconFromFile("imageres.dll", 101);
            SaveImage = NativeMethods.GetIconFromFile("shell32.dll", 258);
            RefreshImage = NativeMethods.GetIconFromFile("imageres.dll", 228);
            SearchImage = NativeMethods.GetIconFromFile("imageres.dll", 168);
            EditImage = NativeMethods.GetIconFromFile("imageres.dll", 247);
            WindowImage = NativeMethods.GetIconFromFile("shell32.dll", 98);
        }
    }
}
