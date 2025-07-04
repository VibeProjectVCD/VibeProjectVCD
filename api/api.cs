using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

// i like how people think dragging ware-x's api into a .net disassembler makes them think they are a reverse engineer 😱
// open source commented

namespace WareXAPI
{
    internal class Pipe
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);
    

        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        private const int INVALID_HANDLE_VALUE = -1;

        public static bool NamedPipeExists(string pipe) // shit didnt really work 💔
        {
            IntPtr handle = CreateFile(
                pipe,
                GENERIC_READ | GENERIC_WRITE,
                0,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);

            if (handle.ToInt64() != INVALID_HANDLE_VALUE)
            {
                CloseHandle(handle);
                return true;
            }
            return false;
        }

        public static void NamedPipeSendData(string script) // will send data to namedpipe
        {
            try
            {
                using (NamedPipeClientStream pipeclient = new NamedPipeClientStream(".", "WareX", PipeDirection.Out))
                {
                    pipeclient.Connect();

                    using (StreamWriter writer = new StreamWriter(pipeclient))
                    {
                        writer.AutoFlush = true;
                        writer.WriteLine(script);
                    }
                }
            }
            catch (Exception ex)
            {
                WareX msgbox = new WareX();
                msgbox.Message(3, "Exception: " + ex.Message.ToString());
                Environment.Exit(-1);
            }
        }

        public string name = @"\\\\.\\pipe\\WareX"; // pipe name
    };

    public class WareX
    {
       
    };
}
