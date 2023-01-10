using System;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using DailyProject_221204.Scripts;

namespace DailyProject_221204
{
    public class GlobalHotKey : IDisposable
    {
        const int WM_HOTKEY = 0x0312;

        const int APPLICATION_MIN_HOTKEY_ID = 0x0000;
        const int APPLICATION_MAX_HOTKEY_ID = 0xbfff;

        const int DLL_MIN_HOTKEY_ID = 0xc000;
        const int DLL_MAX_HOTKEY_ID = 0xffff;

        readonly IntPtr _windowHandle = default;
        readonly Dictionary<int, GlobalHotKeyModel> _globalHotKeys = new Dictionary<int, GlobalHotKeyModel>();
        readonly Random _idGenerator = new Random();

        /// <summary>
        /// グローバルホットキーを登録する。
        /// 失敗の場合は0が返される。
        /// </summary>
        [DllImport("user32.dll")]
        static extern int RegisterHotKey(IntPtr hWnd, int id, int modKey, int key);
        /// <summary>
        /// グローバルホットキーの登録を解除する。
        /// 失敗の場合は0が返される。
        /// </summary>
        [DllImport("user32.dll")]
        static extern int UnregisterHotKey(IntPtr hWnd, int id);

        public GlobalHotKey(IntPtr windowHandle)
        {
            ComponentDispatcher.ThreadPreprocessMessage += _onThreadPreprocessMessage;

            _windowHandle = windowHandle;
        }

        public IDisposable SubscribeGlobalHotKey(Action action, Key key, ModifierKeys modifierKey = ModifierKeys.None)
        {
            var model = new GlobalHotKeyModel(key, modifierKey, action);

            var result = 0;
            var hotKeyId = _generateHotKeyId();
            for (int i = 0; i < 16; i++)
            {
                result = RegisterHotKey(_windowHandle, hotKeyId, (int)modifierKey, KeyInterop.VirtualKeyFromKey(key));
                if (result != 0)
                {
                    _globalHotKeys[hotKeyId] = model;
                    break;
                }

                hotKeyId = _generateHotKeyId();
            }

            var unsubscribe = new ActionDisposer(() => _unsbscribeGlobalHotkey(hotKeyId));

            if (result != 0)
            {
                Debug.WriteLine($"[key: {key}][modifierKey: {modifierKey}][hotKeyId: {hotKeyId}]グローバルホットキーを設定します。");

                return unsubscribe;
            }
            else
            {
                Debug.Assert(false, "GlobalHotKeyに登録するために適したIDが見つかりませんでした。");

                unsubscribe = new ActionDisposer(() => Debug.WriteLine($"[key: {key}][modifierKey: {modifierKey}][hotKeyId: {hotKeyId}]登録に失敗したグローバルホットキーのDisposeが行われました。"));
                return unsubscribe;
            }
        }

        public void Dispose()
        {
            var hotKeyIds = _globalHotKeys.Keys.ToArray();
            foreach (var id in hotKeyIds)
            {
                UnregisterHotKey(_windowHandle, id);
            }

            ComponentDispatcher.ThreadPreprocessMessage -= _onThreadPreprocessMessage;
        }

        void _unsbscribeGlobalHotkey(int hotKeyId)
        {
            _globalHotKeys.Remove(hotKeyId);
            UnregisterHotKey(_windowHandle, hotKeyId);
        }

        int _generateHotKeyId()
        {
            return _idGenerator.Next(APPLICATION_MIN_HOTKEY_ID, APPLICATION_MAX_HOTKEY_ID);
        }

        void _onThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            // HotKeyが押されたかどうかを判定。
            if (msg.message != WM_HOTKEY)
            {
                return;
            }

            var hotKeyId = msg.wParam.ToInt32();
            if (_globalHotKeys.TryGetValue(hotKeyId, out var hotKey))
            {
                var action = hotKey.Action;
                action?.Invoke();
            }
        }

        class GlobalHotKeyModel : AbstractModel
        {
            public Key Key { get; } = Key.None;
            public ModifierKeys ModifierKey { get; } = ModifierKeys.None;
            public Action Action { get; } = null!;

            public GlobalHotKeyModel(Key key, ModifierKeys modifierKey, Action action)
            {
                Key = key;
                ModifierKey = modifierKey;
                Action = action;
            }
        }
    }
}
