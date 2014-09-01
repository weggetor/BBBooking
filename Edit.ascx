<%@ Control language="C#" Inherits="Bitboxx.DNNModules.BBBooking.Edit" AutoEventWireup="true" Codebehind="Edit.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<div class="dnnForm bbbooking_edit dnnClear">
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblYear" />
            <asp:DropDownList runat="server" ID="ddlYear" AutoPostBack="true" OnSelectedIndexChanged="ddlYear_OnSelectedIndexChanged" CssClass="bbbooking" />
        </div>
        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblSchedule" />
            <asp:Image runat="server" ID="imgSchedule" />
            <br/> <br/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label runat="server" ID="lblBookings" />
            <asp:DataGrid ID="grdBookings" runat="server" AllowSorting="True" DataKeyNames="BookingId"
	            AutoGenerateColumns="False" 
                CellPadding="5"
                Width="486"
	            OnItemCommand="grdBookings_ItemCommand">
                <SelectedItemStyle BackColor="lightgreen" />
	            <Columns>
		            <asp:templatecolumn>
			            <HeaderTemplate>
			                <asp:ImageButton ID="cmdNew" runat="server" CommandName="New" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"BookingId") %>' IconKey="Add" IconStyle="Gray" ToolTip='<%#LocalizeString("cmdNew.Tooltip") %>'/>
			            </HeaderTemplate>
                        <itemtemplate>
				            <asp:ImageButton ID="cmdEdit" runat="server" CommandName="Edit" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"BookingId") %>' IconKey="Edit" IconStyle="Gray" ToolTip='<%#LocalizeString("cmdEdit.Tooltip") %>'/>
			            </itemtemplate>
		            </asp:templatecolumn>
		            <asp:templatecolumn>
			            <itemtemplate>
				            <asp:ImageButton ID="cmdDelete" runat="server" CommandName="Delete" CommandArgument='<%# DataBinder.Eval(Container.DataItem,"BookingId") %>' IconKey="Delete" IconStyle="Gray" ToolTip='<%#LocalizeString("cmdDelete.Tooltip") %>'/>
			            </itemtemplate>
		            </asp:templatecolumn>
		            <asp:BoundColumn  DataField="Startdate" HeaderText="Start" DataFormatString="{0:d}" />
                    <asp:BoundColumn  DataField="Enddate" HeaderText="End" DataFormatString="{0:d}" />
		            <asp:TemplateColumn  HeaderText="State">
			            <itemtemplate>
				            <asp:Label runat="server"  ID="lblState" Text='<%# GetStateName((int)DataBinder.Eval(Container.DataItem,"State")) %>' />
			            </itemtemplate>
		            </asp:TemplateColumn>
                    <asp:TemplateColumn  HeaderText="Client">
			            <itemtemplate>
				            <dnn:Label runat="server" ID="lblUserName" Text='<%# GetUserName((int)DataBinder.Eval(Container.DataItem,"UserId")) %>'  
                                HelpText='<%# GetUserInfo((int)DataBinder.Eval(Container.DataItem,"UserId")) %>' />
			            </itemtemplate>
		            </asp:TemplateColumn>
	            </Columns>
            </asp:DataGrid>
            <br/>
        </div>
        <asp:Panel runat="server" ID="pnlEdit" CssClass="dnnFormItem" Visible="false" >
            <dnn:Label runat="server" ID="lblEdit" />
            <div class="dnnLeft">
                <asp:HiddenField runat="server" ID="hidBookingId"/>
                <asp:PlaceHolder runat="server" ID="phMessage" />
                <table>
                    <tr>
                        <td><asp:Label runat="server" ID="lblStartdate" Resourcekey="lblStartdate.Text" /></td>
                        <td><dnn:DnnDatePicker runat="server" CssClass="dnnFormInput" ID="dtpStartdate" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label runat="server" ID="lblEnddate" Resourcekey="lblEnddate.Text" /></td>
                        <td><dnn:DnnDatePicker runat="server" CssClass="dnnFormInput" ID="dtpEnddate" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label runat="server" ID="lblUser" Resourcekey="lblUser.Text" /></td>
                        <td><asp:DropDownList runat="server" ID="ddlUser" Width="150"/></td>
                    </tr>
                    <tr>
                        <td><asp:Label runat="server" ID="lblState" Resourcekey="lblState.Text" /></td>
                        <td><asp:DropDownList runat="server" ID="ddlState" Width="150"/></td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td><asp:Button ID="cmdCheckSave" runat="server" CssClass="dnnPrimaryAction" OnClick="cmdCheckSave_OnClick" Resourcekey="cmdCheckSave.Text"/>
                            <asp:Button ID="cmdSave" runat="server" CssClass="dnnPrimaryAction" OnClick="cmdSave_OnClick" Resourcekey="cmdSave.Text" Enabled="false"/>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </fieldset>
    <ul class="dnnActions dnnClear">
        <li><asp:LinkButton runat="server" CssClass="dnnPrimaryAction" ResourceKey="Cancel" OnClick="cmdCancel_Click" /></li>
    </ul>
</div>
