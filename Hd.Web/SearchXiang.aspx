<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SearchXiang.aspx.cs" Inherits="Hd.Web.SearchXiang" %>

<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head id="head3" runat="server">
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
  
<body>
    <form id="form3" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/Services/SoaDataService.svc/json" />
            </Services>
        </asp:ScriptManager>
        
    <div>
    <div id="ShowWeiTuoInfoDiv" style="width:800px;">
    <b class="xtop">
	<b class="xb1"></b>
	<b class="xb2"></b>
	<b class="xb3"></b>
	<b class="xb4"></b>
	</b>
        <asp:Table ID="Table1" runat="server" class="xboxcontent" Width="800px" >
           <asp:TableRow Height="50px" Font-Size="X-Large"><asp:TableCell ColumnSpan="4"><b>客户委托信息<hr /></b></asp:TableCell></asp:TableRow>
           <asp:TableRow>
               <asp:TableCell><asp:Label ID="Label15" runat="server" Text="货代自编号："></asp:Label></asp:TableCell>
               <asp:TableCell><asp:TextBox ReadOnly="true" ID="txtHDId" runat="server"></asp:TextBox></asp:TableCell>
           </asp:TableRow>
           <asp:TableRow>
               <asp:TableCell><asp:Label ID="Label1" runat="server" Text="委托人："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtWeiTuoRen" runat="server"></asp:TextBox>
               </asp:TableCell>
               <asp:TableCell><asp:Label ID="Label2" runat="server" Text="委托时间："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtWeiTuoTime" runat="server"></asp:TextBox></asp:TableCell>
           </asp:TableRow>
           <asp:TableRow>
               <asp:TableCell><asp:Label ID="Label3" runat="server" Text="通关类别："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtTgId" runat="server"></asp:TextBox></asp:TableCell>
               <asp:TableCell><asp:Label ID="Label4" runat="server" Text="委托分类："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtWtfl" runat="server"></asp:TextBox></asp:TableCell>
           </asp:TableRow>
           <asp:TableRow>
               <asp:TableCell><asp:Label ID="Label5" runat="server" Text="提单号："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtTiDanHao" runat="server"></asp:TextBox></asp:TableCell>
               <asp:TableCell><asp:Label ID="Label6" runat="server" Text="箱量："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtXiangLiang" runat="server"></asp:TextBox></asp:TableCell>
           </asp:TableRow>
           <asp:TableRow>
               <asp:TableCell><asp:Label ID="Label7" runat="server" Text="到港时间："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtDaoGangTime" runat="server"></asp:TextBox></asp:TableCell>
               <asp:TableCell><asp:Label ID="Label8" runat="server" Text="船名/航次："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtChuanMing" runat="server"></asp:TextBox></asp:TableCell>
           </asp:TableRow>
           <asp:TableRow>
               <asp:TableCell><asp:Label ID="Label9" runat="server" Text="停靠码头："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtTkmt" runat="server"></asp:TextBox></asp:TableCell>
               <asp:TableCell><asp:Label ID="Label10" runat="server" Text="报关单号："></asp:Label></asp:TableCell>
               <asp:TableCell>
                   <asp:TextBox ReadOnly="true" ID="txtBaoGuanHao" runat="server"></asp:TextBox></asp:TableCell>
           </asp:TableRow>
           
           <asp:TableRow><asp:TableCell>&nbsp;</asp:TableCell></asp:TableRow>
           
           <asp:TableRow>
               <asp:TableCell ColumnSpan="4">
               <center>
                  <asp:Table runat="server" Width="100%">
                      <asp:TableRow>
                           <asp:TableCell><asp:Label ID="Label11" runat="server" Text="报检状态："></asp:Label></asp:TableCell>
                           <asp:TableCell>
                               <asp:TextBox ReadOnly="true" ID="txtBjState" runat="server"></asp:TextBox></asp:TableCell>
                           <asp:TableCell><asp:Label ID="Label12" runat="server" Text="报关状态："></asp:Label></asp:TableCell>
                           <asp:TableCell>
                               <asp:TextBox ReadOnly="true" ID="txtBgState" runat="server"></asp:TextBox>
                           </asp:TableCell>
                           <asp:TableCell><asp:Label ID="Label13" runat="server" Text="承运状态："></asp:Label></asp:TableCell>
                           <asp:TableCell>
                               <asp:TextBox ReadOnly="true" ID="txtCyState" runat="server"></asp:TextBox>
                           </asp:TableCell>
                      </asp:TableRow>
                  </asp:Table>
                  </center>
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
    <br /><br /><br />
    
        
<ComponentArt:Grid ID="Grid3" RunningMode="WebService"  SoaService="ComponentArt.SOA.UI.ISoaDataGridService"
                    PreHeaderClientTemplateId="PreHeaderTemplate" PostFooterClientTemplateId="PostFooterTemplate"
                    DataAreaCssClass="GridData" ShowHeader="true" HeaderCssClass="GridHeader" FooterCssClass="GridFooter" PageSize="16"
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
         Width="1200" Height="440" PreloadLevels="false" ExpandImageUrl="lines/plus.gif"
        CollapseImageUrl="lines/minus.gif"
                    runat="server">
                    
<%--                    <ClientEvents>
          <WebServiceComplete EventHandler="Grid1_webServiceComplete" />
        </ClientEvents>--%>
                    <Levels>
                        <ComponentArt:GridLevel AllowGrouping="true" DataKeyField="箱号" ShowTableHeading="false"
                            TableHeadingCssClass="GridHeader" RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif"
                            DataCellCssClass="DataCell" HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" SortedDataCellCssClass="SortedDataCell"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="9"
                            SortImageHeight="5" TableHeadingClientTemplateId="TableHeadingTemplate" GroupHeadingCssClass="GroupHeading">
                            <Columns>
                                <ComponentArt:GridColumn DataField="箱号" HeadingText="箱号" />
                                <ComponentArt:GridColumn DataField="封志号" HeadingText="封志号" />
                                <ComponentArt:GridColumn DataField="箱型" HeadingText="箱型" />
                                <ComponentArt:GridColumn DataField="重量" HeadingText="重量(千克)" />
                                <ComponentArt:GridColumn DataField="提箱地" HeadingText="提箱地" />
                                <ComponentArt:GridColumn DataField="卸货地" HeadingText="卸货地" />
                                <ComponentArt:GridColumn DataField="当前状态" HeadingText="当期状态" />
                                <ComponentArt:GridColumn DataField="还箱时间" HeadingText="还箱时间" FormatString="yyyy-M-dd" />
                                <ComponentArt:GridColumn DataField="备注" HeadingText="备注" />
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
                        
                        <ComponentArt:ClientTemplate Id="DetailTemplate3">
                                <a style="color:#595959;" href="Search.aspx?">## DataItem.GetMemberAt(0).Value ##</a>
                        </ComponentArt:ClientTemplate>


                    </ClientTemplates>
                </ComponentArt:Grid>
    </div>
    </form>
</body>
</html>
