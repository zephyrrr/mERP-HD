<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimpleSearch.aspx.cs" Inherits="Hd.Web.SimpleSearch" %>

<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en" lang="en">
<head id="head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta http-equiv="Content-language" content="en" />
    <meta http-equiv="imagetoolbar" content="false" />
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <link rel="shortcut icon" type="image/x-icon" href="http://www.componentart.com/favicon.ico" />
    <link rel="icon" type="image/x-icon" href="http://www.componentart.com/favicon.ico" />
    <title>Search</title>
    <link href="gridStyle.css" type="text/css" rel="stylesheet" />
</head>
<%--<script type="text/javascript">

  function Grid1_webServiceBeforeInvoke(sender, eventArgs) {
      var request = eventArgs.get_request();
//      request.Filters[0] = { "DataFieldName": "ProductName", "DataFieldValue": "Chang", "Operand": 0 } 
  }
    </script>--%>
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
                        <ComponentArt:GridLevel AllowGrouping="true" DataKeyField="Id" ShowTableHeading="false"
                            TableHeadingCssClass="GridHeader" RowCssClass="Row" ColumnReorderIndicatorImageUrl="reorder.gif"
                            DataCellCssClass="DataCell" HeadingCellCssClass="HeadingCell" HeadingCellHoverCssClass="HeadingCellHover"
                            HeadingCellActiveCssClass="HeadingCellActive" HeadingRowCssClass="HeadingRow"
                            HeadingTextCssClass="HeadingCellText" SelectedRowCssClass="SelectedRow" SortedDataCellCssClass="SortedDataCell"
                            SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif" SortImageWidth="9"
                            SortImageHeight="5" TableHeadingClientTemplateId="TableHeadingTemplate" GroupHeadingCssClass="GroupHeading">
                            <Columns>
                                
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
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Clear" />
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    </form>
</body>
</html>
