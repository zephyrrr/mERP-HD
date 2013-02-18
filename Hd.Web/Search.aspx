<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Hd.Web.Search" %>

<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head id="head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Content-language" content="zh-cn" />
    <meta http-equiv="imagetoolbar" content="false" />
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <link rel="shortcut icon" type="image/x-icon" href="http://www.componentart.com/favicon.ico" />
    <link rel="icon" type="image/x-icon" href="http://www.componentart.com/favicon.ico" />
    <title>Search</title>
    <link href="gridStyle.css" type="text/css" rel="stylesheet" />
    <link href="tableCss.css" type="text/css" rel="stylesheet" />
</head>
<%--<script type="text/javascript">

  function Grid1_webServiceBeforeInvoke(sender, eventArgs) {
      var request = eventArgs.get_request();
//      request.Filters[0] = { "DataFieldName": "ProductName", "DataFieldValue": "Chang", "Operand": 0 } 
  }
    </script>--%>
    <script language="javascript" type="text/javascript" src="DatePicker/WdatePicker.js"></script>
<script type="text/javascript">
    function select_HDId(HDId)
    {
        window.open("SearchXiang.aspx?winTab=网页查询_箱信息_进口&se= 货代自编号 = \"" + HDId + "\"");
    }
</script>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/SoaDataService.svc/json" />
                <asp:ServiceReference Path="~/Services/WebDataService.asmx" />
            </Services>
        </asp:ScriptManager>
        <div>
            <div style="padding: 20px;">
                <ComponentArt:Grid ID="Grid1" RunningMode="WebService"  SoaService="ComponentArt.SOA.UI.ISoaDataGridService"
                    PreHeaderClientTemplateId="PreHeaderTemplate" PostFooterClientTemplateId="PostFooterTemplate"
                    DataAreaCssClass="GridData" ShowHeader="true" HeaderCssClass="GridHeader" 
                    FooterCssClass="GridFooter" PageSize="16"
                    PagerStyle="Slider" PagerTextCssClass="GridFooterText" PagerButtonWidth="44"
                    PagerButtonHeight="26" PagerButtonHoverEnabled="true" SliderHeight="26" SliderWidth="150"
                    SliderGripWidth="9" SliderPopupOffsetX="80" SliderPopupClientTemplateId="SliderTemplate"
                    ImagesBaseUrl="images/" PagerImagesFolderUrl="images/pager/" TreeLineImagesFolderUrl="images/lines/"
        TreeLineImageWidth="22"
        TreeLineImageHeight="19"
        LoadingPanelFadeDuration="1000"
                    LoadingPanelFadeMaximumOpacity="60" LoadingPanelClientTemplateId="LoadingFeedbackTemplate"
                    LoadingPanelPosition="MiddleCenter" AllowGrouping="true" GroupingNotificationText="Drag a column to this area to group by it."
        GroupingNotificationTextCssClass="GridHeaderText"
        GroupBySortAscendingImageUrl="group_asc.gif"
        GroupBySortDescendingImageUrl="group_desc.gif"
        GroupBySortImageWidth="10"
        GroupBySortImageHeight="10"
        GroupingMode="ConstantRecords"
        PreExpandOnGroup="true"
        GroupByCssClass="GroupByCell"
        GroupByTextCssClass="GroupByText"
         Width="1200px" Height="440" PreloadLevels="false" ExpandImageUrl="lines/plus.gif"
        CollapseImageUrl="lines/minus.gif"
                    runat="server" AllowTextSelection="True" EmptyGridText="没有查询到相关数据...">
                    
<%--                    <ClientEvents>
          <WebServiceComplete EventHandler="Grid1_webServiceComplete" />
        </ClientEvents>--%>
                    <Levels>
                        <ComponentArt:GridLevel AllowGrouping="true" DataKeyField="货代自编号" ShowTableHeading="false"
                            TableHeadingCssClass="GridHeader" RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif"
                            DataCellCssClass="DataCell" HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" SortedDataCellCssClass="SortedDataCell"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="9"
                            SortImageHeight="5" TableHeadingClientTemplateId="TableHeadingTemplate" GroupHeadingCssClass="GroupHeading" ShowSelectorCells="False">
                             <Columns>
                          <ComponentArt:GridColumn DataField="货代自编号" HeadingText="货代自编号" DataCellClientTemplateId="货代自编号" />
                          <ComponentArt:GridColumn DataField="委托时间" HeadingText="委托时间"  FormatString="yyyy-M-dd" />
                          <ComponentArt:GridColumn DataField="委托人" HeadingText="委托人" />
                          <ComponentArt:GridColumn DataField="通关类别" HeadingText="通关类别" />
                          <ComponentArt:GridColumn DataField="委托分类" HeadingText="委托分类" />
                          <ComponentArt:GridColumn DataField="提单号" HeadingText="提单号" />
                          <ComponentArt:GridColumn DataField="船名航次" HeadingText="船名航次" />
                          <ComponentArt:GridColumn DataField="到港时间" HeadingText="到港时间"  FormatString="yyyy-M-dd" />
                          <ComponentArt:GridColumn DataField="停靠码头" HeadingText="停靠码头" />
                          <ComponentArt:GridColumn DataField="箱量" HeadingText="箱量" />
                          <ComponentArt:GridColumn DataField="报检状态" HeadingText="报检状态" />
                          <ComponentArt:GridColumn DataField="报关状态" HeadingText="报关状态" />
                          <ComponentArt:GridColumn DataField="承运状态" HeadingText="承运状态" />
                          <ComponentArt:GridColumn DataField="报关单号" HeadingText="报关单号" />
                     </Columns>  
                        </ComponentArt:GridLevel>
                    </Levels>
                    <ClientTemplates>
                        <ComponentArt:ClientTemplate ID="PreHeaderTemplate">
                            <img alt="" src="images/header_bga.gif" style="display: block;" />
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="PostFooterTemplate">
                            <img alt="" src="images/grid_postfooter.gif" style="display: block;" />
                        </ComponentArt:ClientTemplate>
                        <ComponentArt:ClientTemplate ID="TableHeadingTemplate">
                            Try paging, sorting, column resizing, and column reordering.
                        </ComponentArt:ClientTemplate>
                      
                        <ComponentArt:ClientTemplate ID="LoadingFeedbackTemplate">
                            <table height="340" width="692" bgcolor="#e0e0e0">
                                <tr>
                                    <td valign="middle" align="center">
                                        <table cellspacing="0" cellpadding="0" border="0">
                                            <tr>
                                                <td style="font-size: 10px; font-family: Verdana;">
                                                    Loading...&nbsp;
                                                </td>
                                                <td>
                                                    <img src="images/spinner.gif" alt="spinner" width="16" height="16" border="0" />
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:ClientTemplate>
                        
                        <ComponentArt:ClientTemplate ID="货代自编号">
                              <a href="javascript:select_HDId('## DataItem.GetMember('货代自编号').Value ##')" >## DataItem.GetMember("货代自编号").Value ##</a>                              
                        </ComponentArt:ClientTemplate>
                          
                        <ComponentArt:ClientTemplate ID="提单号">
                          <a href="SearchXiang.aspx?winTab=网页查询_箱信息_进口&se= 提单号 like ## DataItem.GetMember("提单号").Value ## ">## DataItem.GetMember("货代自编号").Value ##</a>
                        </ComponentArt:ClientTemplate>
                        
                        <ComponentArt:ClientTemplate ID="SliderTemplate">
                            <table class="SliderPopup" width="200" style="background-color: #ffffff" cellspacing="0"
                                cellpadding="0" border="0">
                                <tr>
                                    <td style="padding: 20px;" valign="middle" align="center">
                                        Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## Grid1.PageCount ##</b>
                                    </td>
                                </tr>
                            </table>
                        </ComponentArt:ClientTemplate>
                        
                        <ComponentArt:ClientTemplate Id="DetailTemplate">
                                <a style="color:#595959;" href="Search.aspx?">## DataItem.GetMemberAt(0).Value ##</a>
                        </ComponentArt:ClientTemplate>


                    </ClientTemplates>
                </ComponentArt:Grid>
            </div>
            <br />
            <br />
        </div>
    </div>
    <!-- /demo -->
    <div id="DivSearch" style="width:900px;">
    <b class="xtop">
	<b class="xb1"></b>
	<b class="xb2"></b>
	<b class="xb3"></b>
	<b class="xb4"></b>
	</b>
    <asp:Table runat="server" class="xboxcontent" Width="900px">
        <asp:TableRow Height="50px" Font-Size="X-Large"><asp:TableCell ColumnSpan="4"><b>客户委托信息查询<hr /></b></asp:TableCell></asp:TableRow>
        <asp:TableRow>
          <asp:TableCell>
               <asp:Label ID="Label1" runat="server" Text="货代自编号："></asp:Label>
            </asp:TableCell>
           <asp:TableCell>
               <asp:TextBox ID="id" runat="server"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
               <asp:Label ID="Label8" runat="server" Text="委托时间："></asp:Label>
            </asp:TableCell>
           <asp:TableCell><input class="Wdate" runat="server" type="text" id="weiTuoTime1" onFocus="WdatePicker({isShowClear:false,readOnly:true})"/>&nbsp; 
               至&nbsp; <input class="Wdate" runat="server" type="text" id="weiTuoTime2" onFocus="WdatePicker({isShowClear:false,readOnly:true})"/></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
           <asp:TableCell>
               <asp:Label ID="Label2" runat="server" Text="委托人："></asp:Label>
            </asp:TableCell>
           <asp:TableCell>
               <asp:DropDownList ID="weiTuoRen" runat="server" DataSourceID="SqlDataSource1" 
                   DataTextField="简称" DataValueField="编号">
               </asp:DropDownList>
               <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                   ConnectionString="<%$ ConnectionStrings:ConnectionString %>" SelectCommand="SELECT     编号, 简称
FROM         参数备案_人员单位
WHERE     (角色用途 LIKE '%01,%') AND (业务类型 LIKE '%11,%')
UNION
SELECT     '' AS 编号, '--请选择委托人--' AS 简称
ORDER BY 简称"></asp:SqlDataSource>
            </asp:TableCell>
            <asp:TableCell>
               <asp:Label ID="Label10" runat="server" Text="到港时间："></asp:Label>
            </asp:TableCell>
           <asp:TableCell><input class="Wdate" runat="server" type="text" id="daoGangTime1" onFocus="WdatePicker({isShowClear:false,readOnly:true})"/>&nbsp; 
               至&nbsp; <input class="Wdate" runat="server" type="text" id="daoGangTime2" onFocus="WdatePicker({isShowClear:false,readOnly:true})"/></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
           <asp:TableCell>
               <asp:Label ID="Label3" runat="server" Text="提单号："></asp:Label>
            </asp:TableCell>
          <asp:TableCell>
               <asp:TextBox ID="tiDanHao" runat="server"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
               <asp:Label ID="Label12" runat="server" Text="单证齐全时间："></asp:Label>
            </asp:TableCell>
           <asp:TableCell><input class="Wdate" runat="server" type="text" id="danZhengTime1" 
                   onFocus="WdatePicker({isShowClear:false,readOnly:true})"/>&nbsp; 至&nbsp; <input class="Wdate" runat="server" type="text" id="danZhengTime2" 
                   onFocus="WdatePicker({isShowClear:false,readOnly:true})"/></asp:TableCell>
        </asp:TableRow>
       <asp:TableRow>
           <asp:TableCell>
               <asp:Label ID="Label4" runat="server" Text="箱量："></asp:Label>
            </asp:TableCell>
           <asp:TableCell>
               <asp:TextBox ID="xiangLiang" runat="server"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
               <asp:Label ID="Label11" runat="server" Text="结关时间："></asp:Label>
            </asp:TableCell>
           <asp:TableCell><input class="Wdate" runat="server" type="text" id="jieGuanTime1" 
                   onFocus="WdatePicker({isShowClear:false,readOnly:true})"/>&nbsp; 至&nbsp; 
               <input class="Wdate" runat="server" type="text" id="jieGuanTime2" 
                   onFocus="WdatePicker({isShowClear:false,readOnly:true})"/></asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
           <asp:TableCell>
               <asp:Label ID="Label5" runat="server" Text="总重量："></asp:Label>
            </asp:TableCell>
           <asp:TableCell>
               <asp:TextBox ID="zongZhongLiang" runat="server"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
               <asp:Label ID="Label6" runat="server" Text="代表性箱号："></asp:Label>
            </asp:TableCell>
           <asp:TableCell>
               <asp:TextBox ID="daiBiaoXing" runat="server"></asp:TextBox>
            </asp:TableCell>
        </asp:TableRow>
        <asp:TableRow>
          <asp:TableCell>
               <asp:Label ID="Label7" runat="server" Text="品名："></asp:Label>
            </asp:TableCell>
           <asp:TableCell>
               <asp:TextBox ID="pinMing" runat="server"></asp:TextBox>
            </asp:TableCell>
            <asp:TableCell>
               <asp:Label ID="Label9" runat="server" Text="当前状态："></asp:Label>
            </asp:TableCell>
           <asp:TableCell><asp:DropDownList ID="state" runat="server">
                <asp:ListItem Value="">--请选择当前状态--</asp:ListItem><asp:ListItem Value="未到港">未到港</asp:ListItem><asp:ListItem Value="报关中">报关中</asp:ListItem><asp:ListItem Value="运输中">运输中</asp:ListItem><asp:ListItem Value="已完成">已完成</asp:ListItem></asp:DropDownList>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="Button1" runat="server" Text="查      询" onclick="Button1_Click" Height="25px" />
                </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <b class="xbottom">
	<b class="xb4"></b>
	<b class="xb3"></b>
	<b class="xb2"></b>
	<b class="xb1"></b>
	</b>
	</div>
    </form>
</body>
</html>
