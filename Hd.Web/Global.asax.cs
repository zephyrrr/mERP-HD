using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Spring.Context;
using Spring.Context.Support;
using Feng;

namespace Hd.Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            IApplicationContext ctx = ContextRegistry.GetContext();
            Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(new Microsoft.Practices.ServiceLocation.ServiceLocatorProvider(
                delegate()
                {
                    return new Microsoft.Practices.ServiceLocation.SpringAdapter.SpringServiceLocatorAdapter(ctx);
                }));

            // Spring.net 默认是Singleton的
            IDataBuffer db = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IDataBuffer>();
            if (db != null)
            {
                db.LoadData();
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}