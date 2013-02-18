using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public partial class LoginPage : System.Web.UI.Page
{
	protected void Page_Load(object sender, EventArgs e)
	{
        Login1.LoggedIn += new EventHandler(Login1_LoggedIn);
	}

    void Login1_LoggedIn(object sender, EventArgs e)
    {
        System.Threading.Thread.CurrentPrincipal = HttpContext.Current.User;
    }

}
