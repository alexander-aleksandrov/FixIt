using System.Runtime.InteropServices;

namespace FixIt
{
    public class KeyboardLayout
    {
        private static uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private static int HWND_BROADCAST = 0xffff;
        private static string en_US = "00000409";
        private static string ru_RU = "00000419";
        private static string uk_UA = "00000422";
        private static uint KLF_ACTIVATE = 1;

        public static string NextLanguage(string currentLayout)
        {
            if (currentLayout == "ru-RU")
            {
                PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(en_US, KLF_ACTIVATE));
                return "en-US";
            }
            else if (currentLayout == "en-US")
            {
                PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(ru_RU, KLF_ACTIVATE));
                return "ru-RU";
            }
            else
            {
                PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(ru_RU, KLF_ACTIVATE));
                return "ru-RU";
            }
        }


        private DateTime lastShiftTime = DateTime.MinValue; // время последнего нажатия клавиши Shift

        public bool IsShiftDoublePressed()
        {
            if ((DateTime.Now - lastShiftTime).TotalMilliseconds < 500) // проверяем, что это второе нажатие Shift
            {
                lastShiftTime = DateTime.MinValue; // сбрасываем время первого нажатия Shift
                return true;
            }
            else
            {
                lastShiftTime = DateTime.Now; // запоминаем время первого нажатия Shift
                return false;
            }

        }

        [DllImport("user32.dll")]
        private static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutList(int nBuff, [Out] IntPtr[] lpList);

    }
}
