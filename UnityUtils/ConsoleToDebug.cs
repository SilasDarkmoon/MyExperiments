using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Mod.Unity
{
    public class DebugWriter : System.IO.TextWriter
    {
        private System.Text.StringBuilder sb = new System.Text.StringBuilder();
        private int _LastReceiveTick;

        private static DebugWriter _Instance;
        public static DebugWriter Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DebugWriter();
                }
                return _Instance;
            }
        }
        private DebugWriter()
        {
            _Instance = this;
            _LastReceiveTick = unchecked(Environment.TickCount - 11);

            if (!IsPlayerLoopRegistered)
            {
                RegisterPlayerLoop();
            }
        }

        public override void Write(char value)
        {
            var tick = Environment.TickCount;
            if (sb.Length > 0)
            {
                if (unchecked(tick - _LastReceiveTick) > 10)
                {
                    if (sb[sb.Length - 1] == '\n')
                    {
                        WriteToDebug();
                    }
                }
            }
            _LastReceiveTick = tick;
            sb.Append(value);
        }
        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;

        public void RedirectConsole()
        {
            Console.SetOut(this);
        }

        private void Update()
        {
            if (sb.Length > 0)
            {
                WriteToDebug();
            }
        }
        private void WriteToDebug()
        {
            Debug.Log(GetAndClear());
        }
        private string GetAndClear()
        {
            for (int i = sb.Length - 1; i >= 0; --i)
            {
                if (sb[i] == '\n' || sb[i] == '\r')
                {
                    sb.Remove(i, 1);
                }
                else
                {
                    break;
                }
            }
            var str = sb.ToString();
            sb.Clear();
            return str;
        }

        private static bool IsPlayerLoopRegistered
        {
            get
            {
                var loop = PlayerLoop.GetCurrentPlayerLoop();
                if (loop.subSystemList != null)
                {
                    foreach (var subSystem in loop.subSystemList)
                    {
                        if (subSystem.type == typeof(DebugWriter))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private static void RegisterPlayerLoop()
        {
            var loop = PlayerLoop.GetCurrentPlayerLoop();
            var newLoop = new PlayerLoopSystem
            {
                type = typeof(DebugWriter),
                updateDelegate = () =>
                {
                    _Instance?.Update();
                }
            };
            var subSystems = new List<PlayerLoopSystem>(loop.subSystemList ?? Array.Empty<PlayerLoopSystem>());
            subSystems.Add(newLoop);
            loop.subSystemList = subSystems.ToArray();
            PlayerLoop.SetPlayerLoop(loop);
        }

        private class HookedLogger : UnityEngine.ILogHandler
        {
            private static ILogHandler _UnityDefaultLogHandler;
            static HookedLogger()
            {
                _UnityDefaultLogHandler = Debug.unityLogger.logHandler;
            }

            public HookedLogger()
            {
                Debug.unityLogger.logHandler = this;
            }

            private void TryFlushDebugWriter()
            {
                var cached = DebugWriter.Instance.GetAndClear();
                if (cached != null && cached != "")
                {
                    _UnityDefaultLogHandler.LogFormat(LogType.Log, null, "{0}", cached);
                }
            }

            public void LogException(Exception exception, UnityEngine.Object context)
            {
                TryFlushDebugWriter();
                _UnityDefaultLogHandler.LogException(exception, context);
            }

            public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
            {
                TryFlushDebugWriter();
                _UnityDefaultLogHandler.LogFormat(logType, context, format, args);
            }
        }
        private HookedLogger _Logger = new HookedLogger();
    }

    public static class SystemConsoleToDebugLog
    {
        public static void Setup()
        {
            DebugWriter.Instance.RedirectConsole();
        }
    }
}
