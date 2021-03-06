﻿using System;
using System.Text;

namespace DotGame.Utils
{
    /// <summary>
    /// Stellt verschiedene Funktionen für den Log bereit.
    /// </summary>
    public static class Log
    {
        private static object locker = new object();
        private static StringBuilder buffer = new StringBuilder();

        private static bool allowBuffer = true;
        /// <summary>
        /// Erlaubt ob die Log-Klasse Einträge buffern darf.
        /// </summary>
        public static bool AllowBuffer
        {
            get { return allowBuffer; }
            set
            {
                lock (locker)
                {
                    if (allowBuffer != value)
                    {
                        allowBuffer = value;
                        if (!allowBuffer)
                            FlushBuffer();
                    }
                }
            }
        }

        private static int bufferCapacity = 5000;
        /// <summary>
        /// Gibt an wie viele Zeichen gebuffert werden sollen bis sie zur Konsole oder Debug-Output geschrieben werden soll.
        /// </summary>
        public static int BufferCapacity
        {
            get { return bufferCapacity; }
            set
            {
                lock (locker)
                {
                    if (value != bufferCapacity)
                    {
                        bufferCapacity = value;
                        if (buffer.Length >= bufferCapacity)
                            FlushBuffer();
                    }
                }
            }
        }

        /// <summary>
        /// Das Level ab dem Nachrichten im Log angezeigt werden.
        /// Standard: LogLevel.Verbose
        /// </summary>
        public static LogLevel LevelMinimum { get; set; }

        /// <summary>
        /// Sorgt dafür das der Buffer in die Konsole oder Debug-Output geschrieben wird.
        /// </summary>
        public static void FlushBuffer()
        {
            lock (locker)
            {
                string bufferString = buffer.ToString();
                Console.Write(bufferString);
                System.Diagnostics.Debug.Write(bufferString);
                buffer.Clear();
            }
        }

        /// <summary>
        /// Schreibt eine Nachricht in den Log.
        /// </summary>
        /// <param name="level">Das Level für die Nachricht</param>
        /// <param name="message">Die Nachricht</param>
        public static void Write(LogLevel level, string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (level >= LevelMinimum)// alle Einträge die unter dem Minimum fallen ignoreren
            {
                lock (locker)
                {
                    string[] messageLines = message.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string str in messageLines)
                    {
                        string formattedMessage = string.Format("[{0}] {1} {2}\r\n", DateTime.Now.ToLongTimeString(), ("(" + level.ToString() + ")").PadRight(10), str);
                        buffer.Append(formattedMessage);
                    }

                    if (!allowBuffer || buffer.Length >= bufferCapacity || level == LogLevel.Error)
                        FlushBuffer();
                }
            }
        }

        /// <summary>
        /// Schreibt eine formattierte Nachricht in den Log.
        /// </summary>
        /// <param name="level">Das Level für die Nachricht</param>
        /// <param name="format">Das Format der Nachricht</param>
        /// <param name="args">Die Argumente für das Format</param>
        public static void Write(LogLevel level, string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            if (args == null)
                throw new ArgumentNullException("args");

            Write(level, string.Format(format, args));
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Verbose, message) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Verbose(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Write(LogLevel.Verbose, message);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Debug, message) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Debug(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Write(LogLevel.Debug, message);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Info, message) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Info(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Write(LogLevel.Info, message);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Warning, message) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Warning(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Write(LogLevel.Warning, message);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Error, message) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Error(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            Write(LogLevel.Error, message);
        }

        public static void Error(Exception exception)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            Error(exception.Message);

            if (exception.StackTrace != null)
                Error(exception.StackTrace);
            else
                Error("No stacktrace");
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Verbose, format, args) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Verbose(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            if (args == null)
                throw new ArgumentNullException("args");

            Write(LogLevel.Verbose, format, args);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Debug, format, args) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Debug(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            if (args == null)
                throw new ArgumentNullException("args");

            Write(LogLevel.Debug, format, args);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Info, format, args) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Info(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            if (args == null)
                throw new ArgumentNullException("args");

            Write(LogLevel.Info, format, args);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Warning, format, args) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Warning(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            if (args == null)
                throw new ArgumentNullException("args");

            Write(LogLevel.Warning, format, args);
        }

        /// <summary>
        /// Ruft Log.Write(LogLevel.Error, format, args) auf.
        /// </summary>
        /// <param name="message">Die Nachricht</param>
        public static void Error(string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");
            if (args == null)
                throw new ArgumentNullException("args");

            Write(LogLevel.Error, format, args);
        }


        /// <summary>
        /// Schreibt alle Felder des Objekts obj in den Log.
        /// </summary>
        /// <param name="level">Das LogLevel welches benutzt werden soll.</param>
        /// <param name="obj">Das Objekt mit den Feldern die in den Log geschrieben werden sollen.</param>
        public static void WriteFields(LogLevel level, object obj)
        {
            var fields = obj.GetType().GetFields();
            Log.Write(level, "All fields for {0} [n: {1}]:", obj.GetType().FullName, fields.Length);
            foreach (var field in fields)
                Log.Write(level, "\t{0} = {1}", field.Name, field.GetValue(obj));
        }
    }
}
