<%@ Control Language="C#" AutoEventWireup="false" Inherits="Bitboxx.DNNModules.BBBooking.Settings" Codebehind="Settings.ascx.cs" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
 
<div class="dnnForm dnnBBBookingSettings dnnClear">
    <div class="dnnFormItem">
		<dnn:Label id="lblMinDays" runat="server" controlname="txtMinDays" suffix=":" />
		<asp:Textbox ID="txtMinDays" CssClass="dnnFormInput" runat="server" />
	</div>
    <div class="dnnFormItem">
		<dnn:Label id="lblMinOffset" runat="server" controlname="txtMinOffset" suffix=":" />
		<asp:Textbox ID="txtMinOffset" CssClass="dnnFormInput" runat="server" />
	</div>
    <div class="dnnFormItem">
		<dnn:Label id="lblOnlyFutureDays" runat="server" controlname="chkOnlyFutureDays" suffix=":" />
        <asp:CheckBox ID="chkOnlyFutureDays" CssClass="dnnFormInput" runat="server"  />
	</div>
    <div class="dnnFormItem">
		<dnn:Label id="lblArrivalDays" runat="server" controlname="chkArrivalDays" suffix=":" />
		<asp:CheckBoxList ID="chkArrivalDays" CssClass="dnnFormInput" runat="server"  RepeatDirection="Vertical">
        </asp:CheckBoxList>
	</div>
    <fieldset>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSenderName" runat="server" ControlName="txtSenderName" Suffix=":"/>
            <asp:TextBox runat="server" ID="txtSenderName"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSenderEmail" runat="server" ControlName="txtSenderEmail" Suffix=":"/>
            <asp:TextBox runat="server" ID="txtSenderEmail"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblRecipientEmail" runat="server" ControlName="txtRecipientEmail" Suffix=":"/>
            <asp:TextBox runat="server" ID="txtRecipientEmail"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSendToUser" runat="server" ControlName="chkSendToUser" Suffix=":"/>
            <asp:CheckBox runat="server" ID="chkSendToUser"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblSubject" runat="server" ControlName="txtSubject" Suffix=":"/>
            <asp:TextBox runat="server" ID="txtSubject"/>
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblBodytext" runat="server" ControlName="txtBodytext" Suffix=":"/>
            <asp:TextBox runat="server" ID="txtBodytext" Rows="10" TextMode="MultiLine" Font-Names='courier, "courier new", monospace' />
        </div>
        <div class="dnnFormItem">
            <dnn:Label ID="lblFields" runat="server" ControlName="chkFirstnameVisible" Suffix=":"/>
            <table>
                <tr>
                    <th>&nbsp;</th>
                    <th><asp:Label runat="server" ID="lblVisible" Text="visible"/></th> 
                    <th><asp:Label runat="server" ID="lblMandatory" Text="mandatory"/></th> 
                </tr>
                <tr>
                    <td><asp:Label ID="lblFirstname" runat="server" ResourceKey="lblFirstname.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkFirstnameVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkFirstnameMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblLastname" runat="server" ResourceKey="lblLastname.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkLastnameVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkLastnameMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblOrganization" runat="server" ResourceKey="lblOrganization.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkOrganizationVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkOrganizationMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblStreet" runat="server" ResourceKey="lblStreet.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkStreetVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkStreetMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblCity" runat="server" ResourceKey="lblCity.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkCityVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkCityMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblPostalCode" runat="server" ResourceKey="lblPostalCode.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkPostalCodeVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkPostalCodeMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblPhone" runat="server" ResourceKey="lblPhone.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkPhoneVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkPhoneMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblFax" runat="server" ResourceKey="lblFax.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkFaxVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkFaxMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblEmail" runat="server" ResourceKey="lblEmail.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkEmailVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkEmailMandatory"/></td>
                </tr>
                <tr>
                    <td><asp:Label ID="lblRemark" runat="server" ResourceKey="lblRemark.Text"/></td>
                    <td><asp:CheckBox runat="server" ID="chkRemarkVisible"/></td>
                    <td><asp:CheckBox runat="server" ID="chkRemarkMandatory"/></td>
                </tr>
            </table>
    </fieldset>
</div>

