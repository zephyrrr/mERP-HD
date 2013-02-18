using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Reflection;
using System.Security.Principal;
using System.Threading;

namespace Hd.Utils
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Feng.Run.ProgramHelper.InitProgram();

            IIdentity identity = new GenericIdentity("200001", "ownset");
            IPrincipal principal = new GenericPrincipal(identity, new string[0]);
            Thread.CurrentPrincipal = principal;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormUpload());
        }
    }
}
