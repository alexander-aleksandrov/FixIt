using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace FixIt
{
    public partial class FixItForm : Form
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private static DateTime _lastShiftPressTime = DateTime.MinValue;

        private Language _language;

        private StringBuilder _strBuilder = new StringBuilder();
        private int _keyPressedCounter = 0;
        private InputSimulator simulator = new InputSimulator();
        private bool wasTranscribed = false;
        private Lang _keybLayout;
        private readonly string ruKeyboard = "00000419";
        private readonly string enKeyboard = "00000409";
        private readonly string uaKeyboard = "00000422";

        private CultureInfo _currentLanaguge;

        public FixItForm()
        {
            InitializeComponent();
            _proc = HookCallback; // сохраняем делегат HookCallback в поле _proc
            _hookID = SetHook(_proc); // устанавливаем глобальный перехват клавиатуры
            _language = new Language();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    HandleCurrentLanguage();
                    Thread.Sleep(500);
                }
            });
        }


        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {


            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) // если нажата клавиша на клавиатуре
            {
                Keys key = (Keys)Marshal.ReadInt32(lParam);

                bool shift = IsKeyDown(Keys.ShiftKey);
                bool capsLock = IsCapsLockOn();

                if (wasTranscribed && (key != Keys.RShiftKey || key != Keys.LShiftKey))
                {
                    wasTranscribed = false;
                    _strBuilder.Clear();
                }

                if (char.IsLetterOrDigit((char)key) || key == Keys.Space)
                {
                    char keyChar = GetCharFromKey(key, shift, capsLock);
                    _strBuilder.Append(keyChar);
                    _keyPressedCounter++;
                }


                if (key == Keys.RShiftKey || key == Keys.LShiftKey)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan interval = now - _lastShiftPressTime;

                    if (interval.TotalMilliseconds < 500 && _strBuilder.Length != 0)
                    {
                        string toTranscribe = _strBuilder.ToString();
                        string transcribedText = _language.Transcribe(toTranscribe);

                        //Удаление старой и вывод новой строки
                        for (int i = 0; i < transcribedText.Length; i++)
                        {
                            simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                        }
                        simulator.Keyboard.TextEntry(transcribedText);

                        //Зацикливание транскрибции
                        _strBuilder.Clear();
                        _strBuilder.Append(transcribedText);

                        _lastShiftPressTime = DateTime.MinValue;

                        // Флаг о выполнении транскрибции
                        wasTranscribed = true;
                        return (IntPtr)1; // Отменяем нажатие Shift, чтобы не обрабатывалось дальше
                    }
                    else
                    {
                        _lastShiftPressTime = now; // Сохраняем время нажатия Shift
                    }
                }

                label1.Text = _strBuilder.ToString();
            }


            return CallNextHookEx(_hookID, nCode, wParam, lParam); // передаем управление следующему перехватчику
        }

        private static CultureInfo GetCurrentCulture()
        {
            var l = GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
            return new CultureInfo((short)l.ToInt64());
        }

        private void HandleCurrentLanguage()
        {
            var currentCulture = GetCurrentCulture();
            if (_currentLanaguge == null || _currentLanaguge.LCID != currentCulture.LCID)
            {
                _currentLanaguge = currentCulture;
            }
        }

        private static bool IsKeyDown(Keys key)
        {
            short state = GetKeyState((int)key);
            return (state & 0x8000) != 0;
        }

        private static bool IsCapsLockOn()
        {
            return (Control.IsKeyLocked(Keys.CapsLock));
        }


        private char GetCharFromKey(Keys key, bool shift, bool capsLock)
        {

            char ch;
            string res = String.Empty;

            char keyChar = (char)key;
            string charToAdd = (Char.IsLetterOrDigit(keyChar) || key == Keys.Space) ? keyChar.ToString() : string.Empty;
            charToAdd = (key == Keys.Space) ? " " : charToAdd;
            if (key >= Keys.A && key <= Keys.Z)
            {
                ch = (char)((int)'a' + (int)(key - Keys.A));
                if (shift ^ capsLock)
                {
                    ch = char.ToUpper(ch);
                }
                if (_currentLanaguge.Name == "ru-RU")
                {
                    res = _language.Transcribe(ch.ToString());
                    return res[0];
                }
                else
                {
                    return ch;
                }


            }
            return charToAdd.ToCharArray()[0];
        }

        public static bool IsInputActive()
        {
            IntPtr activeWindow = GetForegroundWindow();
            IntPtr activeInput = GetFocus();
            return (activeWindow == activeInput);
        }
        private void FixItForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnhookWindowsHookEx(_hookID); // отключаем глобальный перехват клавиатуры
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess()) // получаем текущий процесс
            using (ProcessModule curModule = curProcess.MainModule) // получаем основной модуль процесса
            {
                // устанавливаем глобальный перехват клавиатуры и возвращаем идентификатор перехватчика
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutName([Out] StringBuilder pwszKLID);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, nuint wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpWindowText, int nMaxCount);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

        [DllImport("user32.dll")]
        static extern IntPtr GetFocus();

        [DllImport("user32.dll")]
        static extern bool GetKeyboardState(byte[] lpKeyState);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern short GetKeyState(int key);
    }
}