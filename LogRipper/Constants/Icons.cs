using System.Windows.Media;

using LogRipper.Helpers;

namespace LogRipper.Constants;

internal static class Icons
{
    private const string Shell32Dll = "shell32.dll";
    private const string ImageResDll = "imageres.dll";

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
        HelpImage = NativeMethods.GetIconFromFile(Shell32Dll, 221);
        RecycleImage = NativeMethods.GetIconFromFile(Shell32Dll, 31);
        CancelImage = NativeMethods.GetIconFromFile(ImageResDll, 100);
        OkImage = NativeMethods.GetIconFromFile(ImageResDll, 101);
        SaveImage = NativeMethods.GetIconFromFile(Shell32Dll, 258);
        RefreshImage = NativeMethods.GetIconFromFile(ImageResDll, 228);
        SearchImage = NativeMethods.GetIconFromFile(ImageResDll, 168);
        EditImage = NativeMethods.GetIconFromFile(ImageResDll, 247);
        WindowImage = NativeMethods.GetIconFromFile(Shell32Dll, 98);
    }
}
