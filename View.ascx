<%@ Control language="C#" Inherits="Bitboxx.DNNModules.BBBooking.View" AutoEventWireup="true" Codebehind="View.ascx.cs" %>
<%@ Register Assembly="DotNetNuke.Web" Namespace="DotNetNuke.Web.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<div style="width:486px" class="dnnForm bbbooking_view">
    <asp:Image runat="server" ID="imgSchedule" />
    <div style="float:left" class="legend">
        <asp:Label runat="server" ID="lblOccupied" CssClass="legend" BackColor="#9A0103" ForeColor="#FFFFFF" ResourceKey="lblOccupied.Text" />
        <asp:Label runat="server" ID="lblFree" CssClass="legend" BackColor="#009800" ForeColor="#FFFFFF"  ResourceKey="lblFree.Text" />
        <asp:Label runat="server" ID="lblReserved" CssClass="legend" BackColor="#FCCE00" ForeColor="000000"  ResourceKey="lblReserved.Text" />
    </div>
    <div style="float:right;" class="legend">
        <asp:Label ID="lblYearCap" runat="server" ResourceKey="lblYearCap.Text" />
        <asp:LinkButton runat="server" ID="lnkPrevYear" OnClick="lnkPrevYear_OnClick"/> | 
        <asp:Label runat="server" ID="lblYear"/> | 
        <asp:LinkButton runat="server" ID="lnkNextYear" OnClick="lnkNextYear_OnClick"/>
    </div>
    <div style="clear:both;">&nbsp;</div>
    
    <asp:MultiView runat="server" ID="mvBooking" ActiveViewIndex="0">
        <asp:View runat="server" ID="vwCheck">
            <h3><asp:Label runat="server" ID="lblCheck" Resourcekey="lblCheck.Text"/></h3>
            <asp:PlaceHolder runat="server" ID="phInfo" />
            <asp:PlaceHolder runat="server" ID="phMessage" />
            <table style="border-width:0;width:100%; border-collapse: collapse;">
                <tr>
                    <td><asp:Label runat="server" ID="lblStartdate" Resourcekey="lblStartdate.Text" /></td>
                    <td><asp:Label runat="server" ID="lblEnddate" Resourcekey="lblEnddate.Text" /></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td style="width:150px"><dnn:DnnDatePicker runat="server" ID="dtpStartdate" /></td>
                    <td style="width:150px"><dnn:DnnDatePicker runat="server" ID="dtpEnddate" /></td>
                    <td><asp:Button runat="server" ID="cmdCheck" ResourceKey="cmdCheck.Text" OnClick="cmdCheck_OnClick"/></td>
                </tr>
            </table>
        </asp:View>
        <asp:View runat="server" ID="vwUserdata">
            <h3><asp:Label runat="server" ID="lblUserdata" Resourcekey="lblUserdata.Text"/></h3>
            <fieldset>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblSelectedTimeCap" runat="server" ControlName="lblSelectedTime" Suffix=":"/>
                    <asp:Label runat="server" ID="lblSelectedTime" CssClass="legend" BackColor="#034DFE" ForeColor="#FFFFFF" Font-Size="10"/> 
                    <asp:Button runat="server" ID="cmdChangeSelection" ResourceKey="cmdChangeSelection.Text" OnClick="cmdChangeSelection_OnClick" CausesValidation="false"/>
                </div>
                <asp:Panel CssClass="dnnFormItem" id="pnlFirstname" runat="server">
                    <dnn:Label ID="lblFirstname" runat="server" ControlName="txtFirstname" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtFirstname" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valFirstname" ControlToValidate="txtFirstname" Display="Dynamic" ResourceKey="valFirstname.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlLastname" runat="server">
                    <dnn:Label ID="lblLastname" runat="server" ControlName="txtLastname" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtLastname" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valLastname" ControlToValidate="txtLastname" Display="Dynamic" ResourceKey="valLastname.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlOrganization" runat="server">
                    <dnn:Label ID="lblOrganization" runat="server" ControlName="txtOrganization" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtOrganization" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valOrganization" ControlToValidate="txtOrganization" Display="Dynamic" ResourceKey="valOrganization.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlStreet" runat="server">
                    <dnn:Label ID="lblStreet" runat="server" ControlName="txtAddress" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtStreet" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valStreet" ControlToValidate="txtStreet" Display="Dynamic" ResourceKey="valStreet.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlCity" runat="server">
                    <dnn:Label ID="lblCity" runat="server" ControlName="txtCity" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtCity" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valCity" ControlToValidate="txtCity" Display="Dynamic" ResourceKey="valCity.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlPostalCode" runat="server">
                    <dnn:Label ID="lblPostalCode" runat="server" ControlName="txtPostalCode" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtPostalCode" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valPostalCode" ControlToValidate="txtPostalCode" Display="Dynamic" ResourceKey="valPostalCode.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlPhone" runat="server">
                    <dnn:Label ID="lblPhone" runat="server" ControlName="txtPhone" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtPhone" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valPhone" ControlToValidate="txtPhone" Display="Dynamic" ResourceKey="valPhone.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                 <asp:Panel CssClass="dnnFormItem" id="pnlFax" runat="server">
                    <dnn:Label ID="lblFax" runat="server" ControlName="txtFax" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtFax" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valFax" ControlToValidate="txtFax" Display="Dynamic" ResourceKey="valFax.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlEmail" runat="server">
                    <dnn:Label ID="lblEmail" runat="server" ControlName="txtEmail" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtEmail" CssClass="dnnFormInput"/>
                    <asp:RequiredFieldValidator runat="server" ID="valEmail" ControlToValidate="txtEmail" Display="Dynamic" ResourceKey="valEmail.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                    <asp:RegularExpressionValidator runat="server" ID="valEmailOK" ControlToValidate="txtEmail" Display="Dynamic" ResourceKey="valEmailOK.Text" CssClass="dnnFormMessage dnnFormError"  />
                </asp:Panel>
                <asp:Panel CssClass="dnnFormItem" id="pnlRemark" runat="server">
                    <dnn:Label ID="lblRemark" runat="server" ControlName="txtRemark" Suffix=":"/>
                    <asp:TextBox runat="server" ID="txtRemark" Rows="5" TextMode="MultiLine" />
                    <asp:RequiredFieldValidator runat="server" ID="valRemark" ControlToValidate="txtRemark" Display="Dynamic" ResourceKey="valRemark.Text" CssClass="dnnFormMessage dnnFormError" Visible="False"/>
                </asp:Panel>
                <div class="dnnFormItem">
                    <dnn:Label ID="lblDummy" runat="server" ControlName="cmdSend" />
                    <ul class="dnnActions dnnClear">
                        <li><asp:LinkButton CssClass="dnnPrimaryAction" ID="cmdSend" runat="server" OnClick="cmdSend_Click" ResourceKey="cmdSend" /></li>
                    </ul>
                </div>
            </fieldset>
        </asp:View>
        <asp:View runat="server" ID="vwConfirm">
            <h3><asp:Label runat="server" ID="lblConfirm" Resourcekey="lblConfirm.Text"/></h3>
            <asp:PlaceHolder runat="server" ID="phConfirm" />
        </asp:View>
    </asp:MultiView>
</div>
