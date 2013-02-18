<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Report.aspx.cs" Inherits="Hd.Web.Report" %>

<%@ Register assembly="CrystalDecisions.Web, Version=10.5.3700.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" namespace="CrystalDecisions.Web" tagprefix="CR" %>
<%@ Register assembly="Feng.Report" namespace="Feng.Web" tagprefix="FW" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>报表</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <FW:MyCrystalReportViewer ID="CrystalReportViewer1" runat="server" 
            AutoDataBind="true" />
    
    </div>
    
    </form>
</body>
</html>
