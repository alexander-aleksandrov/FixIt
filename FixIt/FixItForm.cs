using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using WindowsInput;

namespace FixIt
{
    public partial class FixItForm : Form
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private static DateTime _lastShiftPressTime = DateTime.MinValue;
        private int _delayInMs = 500;

        private KeyboardLayoutDictionary _dictionary;

        private StringBuilder _initialText = new();
        private StringBuilder _mappedText = new();
        private InputSimulator simulator = new();
        private bool wasMapped = false;

        private CultureInfo _currentLanaguge;
        private KeyboardLayout _keybLayout = new();

        private List<Letter> _lettersBuffer = new();
        public FixItForm()
        {
            InitializeComponent();
            _proc = HookCallback; // сохраняем делегат HookCallback в поле _proc
            _hookID = SetHook(_proc); // устанавливаем глобальный перехват клавиатуры
            _dictionary = new KeyboardLayoutDictionary();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    HandleCurrentLanguage();
                    Thread.Sleep(500);
                }
            });
        }

        //reading a key in a hook, performs on every key press
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) // если нажата клавиша на клавиатуре
            {
                Keys pressedKey = (Keys)Marshal.ReadInt32(lParam);

                //Clearing buuffer when new input started
                bool keyIsAllowed = _dictionary.allowedKeys.Contains(pressedKey);
                bool keyIsShift = pressedKey == Keys.RShiftKey || pressedKey == Keys.LShiftKey;
                CaseFlag caseflag = IsKeyDown(Keys.ShiftKey) || IsCapsLockOn() ? CaseFlag.UpperCase : CaseFlag.LowerCase;

                if (wasMapped)
                {
                    if (keyIsAllowed)
                    {
                        if (keyIsShift)
                        {
                            DateTime now = DateTime.Now;
                            TimeSpan interval = now - _lastShiftPressTime;

                            if (interval.TotalMilliseconds < _delayInMs && _lettersBuffer.Count != 0)
                            {
                                ProcessLetterBuffer();

                                _lastShiftPressTime = DateTime.MinValue;

                                // Флаг о выполнении транскрибции
                                wasMapped = true;

                                return (IntPtr)1; // Отменяем нажатие Shift, чтобы не обрабатывалось дальше
                            }
                            else
                            {
                                _lastShiftPressTime = now; // Сохраняем время нажатия Shift
                            }
                        }
                        else
                        {
                            _lettersBuffer.Clear();
                            wasMapped = false;
                            _lettersBuffer.Add(new Letter(pressedKey, _currentLanaguge.Name, caseflag));
                        }
                    }
                    else
                    {
                        wasMapped = false;
                        _lettersBuffer.Clear();
                    }
                }
                else //not mapped
                {
                    if (keyIsAllowed)
                    {
                        if (keyIsShift)
                        {
                            DateTime now = DateTime.Now;
                            TimeSpan interval = now - _lastShiftPressTime;

                            if (interval.TotalMilliseconds < _delayInMs && _lettersBuffer.Count != 0)
                            {
                                ProcessLetterBuffer();

                                _lastShiftPressTime = DateTime.MinValue;

                                // Флаг о выполнении транскрибции
                                wasMapped = true;

                                return (IntPtr)1; // Отменяем нажатие Shift, чтобы не обрабатывалось дальше
                            }
                            else
                            {
                                _lastShiftPressTime = now; // Сохраняем время нажатия Shift
                            }
                        }
                        else // not shift key
                        {
                            //add letter
                            _lettersBuffer.Add(new Letter(pressedKey, _currentLanaguge.Name, caseflag));
                        }
                    }
                    else //not keyIsAllowed
                    {
                        _lettersBuffer.Clear();
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam); // передаем управление следующему перехватчику
        }

        private void ProcessLetterBuffer()
        {
            string txt = _dictionary.Map(_lettersBuffer);
            _mappedText.Append(txt);

            ReplaceOnScreen(txt);
            label1.Text = _mappedText.ToString();
            ChangeCurrentLanguage();

            _mappedText.Clear();
        }

        private void ReplaceOnScreen(string txt)
        {
            //Удаление старой и вывод новой строки
            for (int i = 0; i < txt.Length; i++)
            {
                //simulator.Keyboard.KeyPress(VirtualKeyCode.BACK);
                PressBackspaceKey();
            }
            simulator.Keyboard.TextEntry(txt);

        }

        private void PressBackspaceKey()
        {
            const byte VK_BACK = 0x08;
            keybd_event(VK_BACK, 0, 0, 0); // эмуляция нажатия клавиши
            keybd_event(VK_BACK, 0, 2, 0); // эмуляция отпускания клавиши
        }

        private void ChangeCurrentLanguage()
        {
            string newLang = KeyboardLayout.NextLanguage(_currentLanaguge.Name);
            foreach (Letter l in _lettersBuffer)
            {
                l.Lang = newLang;
            }
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

        //imports---------------------------------------------------------------------------

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);
        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll")]
        static extern int GetFocus();

        [DllImport("user32.dll")]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(int hWnd, int ProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern int SendMessage(int hWnd, int Msg, int wParam, StringBuilder lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern int GetKeyboardLayoutName([Out] StringBuilder pwszKLID);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, nuint wParam, StringBuilder lParam);

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
        static extern short GetKeyState(int key);
    }
}