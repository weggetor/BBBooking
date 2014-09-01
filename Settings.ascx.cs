using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Host;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;


namespace Bitboxx.DNNModules.BBBooking
{

    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Settings class manages Module Settings
    /// </summary>
    /// -----------------------------------------------------------------------------
    [DNNtc.PackageProperties("BBBooking")]
    [DNNtc.ModuleProperties("BBBooking")]
    [DNNtc.ModuleControlProperties("Settings", "Configure settings", DNNtc.ControlType.Edit, "", true, true)]
    public partial class Settings : ModuleSettingsBase
    {

        #region Base Method Implementations

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// LoadSettings loads the settings from the Database and displays them
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void LoadSettings()
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    int i = 0;
                    string[] dayNums;
                    if (ModuleSettings["AllowedDays"] != null)
                        dayNums = ((string) ModuleSettings["AllowedDays"]).Split(',');
                    else
                        dayNums = "0,1,2,3,4,5,6".Split(',');

                    foreach (string dayName in CultureInfo.CurrentCulture.DateTimeFormat.DayNames)
                    {
                        ListItem dayItem = new ListItem(dayName, i.ToString());
                        dayItem.Selected = dayNums.Contains(i.ToString());
                        chkArrivalDays.Items.Add(dayItem);
                        i++;
                    }
                    
                    txtMinDays.Text = (string)ModuleSettings["MinDays"] ?? "1";
                    txtMinOffset.Text = (string)ModuleSettings["MinOffset"] ?? "0";
                    chkOnlyFutureDays.Checked = Convert.ToBoolean(ModuleSettings["OnlyFutureDays"] ?? "false");

                    if (Settings["SenderName"] != null)
                        txtSenderName.Text = (string)Settings["SenderName"];

                    if (Settings["SenderEmail"] != null)
                        txtSenderEmail.Text = (string)Settings["SenderEmail"];
                    else
                        txtSenderEmail.Text = Host.HostEmail;

                    if (Settings["RecipientEmail"] != null)
                        txtRecipientEmail.Text = (string)Settings["RecipientEmail"];

                    if (Settings["Subject"] != null)
                        txtSubject.Text = (string)Settings["Subject"];
                    else
                        txtSubject.Text = "Booking form submission from [EMAIL]";

                    if (Settings["SendToUser"] != null)
                        chkFirstnameVisible.Checked = Convert.ToBoolean(Settings["SendToUser"]);

                    if (Settings["Bodytext"] != null)
                        txtBodytext.Text = (string)Settings["Bodytext"];
                    else
                        txtBodytext.Text = "Booking form submission :\r\n\r\n" +
                                          "Startdate    : [STARTDATE]\r\n" +
                                          "Enddate      : [ENDDATE]\r\n" +
                                          "Firstname    : [FIRSTNAME]\r\n" +
                                          "Lastname     : [LASTNAME]\r\n" +
                                          "Organization : [ORGANIZATION]\r\n" +
                                          "Street       : [STREET]\r\n" +
                                          "City         : [CITY]\r\n" +
                                          "Postalcode   : [POSTALCODE]\r\n" +
                                          "Phone        : [PHONE]\r\n" +
                                          "Fax          : [FAX]\r\n" +
                                          "Email        : [EMAIL]\r\n" +
                                          "Remarks :\r\n" +
                                          "[REMARK]";
                    
                    chkFirstnameVisible.Checked = Convert.ToBoolean(Settings["VisibleFirstname"] ?? "false");
                    chkLastnameVisible.Checked = Convert.ToBoolean(Settings["VisibleLastname"] ?? "false");
                    chkOrganizationVisible.Checked = Convert.ToBoolean(Settings["VisibleOrganization"] ?? "false");
                    chkStreetVisible.Checked = Convert.ToBoolean(Settings["VisibleStreet"] ?? "false");
                    chkCityVisible.Checked = Convert.ToBoolean(Settings["VisibleCity"] ?? "false");
                    chkPostalCodeVisible.Checked = Convert.ToBoolean(Settings["VisiblePostalCode"] ?? "false");
                    chkPhoneVisible.Checked = Convert.ToBoolean(Settings["VisiblePhone"] ?? "false");
                    chkFaxVisible.Checked = Convert.ToBoolean(Settings["VisibleFax"] ?? "false");
                    chkEmailVisible.Checked = Convert.ToBoolean(Settings["VisibleEmail"] ?? "false");
                    chkRemarkVisible.Checked = Convert.ToBoolean(Settings["VisibleRemark"] ?? "false");

                    chkFirstnameMandatory.Checked = Convert.ToBoolean(Settings["EnsureFirstname"] ?? "false");
                    chkLastnameMandatory.Checked = Convert.ToBoolean(Settings["EnsureLastname"] ?? "false");
                    chkOrganizationMandatory.Checked = Convert.ToBoolean(Settings["EnsureOrganization"] ?? "false");
                    chkStreetMandatory.Checked = Convert.ToBoolean(Settings["EnsureStreet"] ?? "false");
                    chkCityMandatory.Checked = Convert.ToBoolean(Settings["EnsureCity"] ?? "false");
                    chkPostalCodeMandatory.Checked = Convert.ToBoolean(Settings["EnsurePostalCode"] ?? "false");
                    chkPhoneMandatory.Checked = Convert.ToBoolean(Settings["EnsurePhone"] ?? "false");
                    chkFaxMandatory.Checked = Convert.ToBoolean(Settings["EnsureFax"] ?? "false");
                    chkEmailMandatory.Checked = Convert.ToBoolean(Settings["EnsureEmail"] ?? "false");
                    chkRemarkMandatory.Checked = Convert.ToBoolean(Settings["EnsureRemark"] ?? "false");
                }
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpdateSettings saves the modified settings to the Database
        /// </summary>
        /// -----------------------------------------------------------------------------
        public override void UpdateSettings()
        {
            try
            {
                ModuleController module = new ModuleController();
                StringBuilder sb = new StringBuilder();
                foreach (ListItem dayItem in chkArrivalDays.Items)
                {
                    if (dayItem.Selected)
                        sb.Append(dayItem.Value + ",");
                }

                string allowedDays = sb.ToString();
                if (allowedDays.Length > 0)
                    allowedDays = allowedDays.Substring(0, allowedDays.Length - 1);

                module.UpdateModuleSetting(this.ModuleId, "AllowedDays", allowedDays);
                module.UpdateModuleSetting(this.ModuleId, "MinDays", txtMinDays.Text);
                module.UpdateModuleSetting(this.ModuleId, "MinOffset", txtMinOffset.Text);
                module.UpdateModuleSetting(this.ModuleId, "OnlyFutureDays", chkOnlyFutureDays.Checked.ToString());

                module.UpdateModuleSetting(this.ModuleId, "SenderName", txtSenderName.Text.Trim());
                module.UpdateModuleSetting(this.ModuleId, "SenderEmail", txtSenderEmail.Text.Trim());
                module.UpdateModuleSetting(this.ModuleId, "RecipientEmail", txtRecipientEmail.Text.Trim());
                module.UpdateModuleSetting(this.ModuleId, "Subject", txtSubject.Text.Trim());
                module.UpdateModuleSetting(this.ModuleId, "SendToUser", chkSendToUser.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "Bodytext", txtBodytext.Text.Trim());

                module.UpdateModuleSetting(this.ModuleId, "VisibleFirstname", chkFirstnameVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisibleLastname", chkLastnameVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisibleOrganization", chkOrganizationVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisibleStreet", chkStreetVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisibleCity", chkCityVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisiblePostalCode", chkPostalCodeVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisiblePhone", chkPhoneVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisibleFax", chkFaxVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisibleEmail", chkEmailVisible.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "VisibleRemark", chkRemarkVisible.Checked.ToString());

                module.UpdateModuleSetting(this.ModuleId, "EnsureFirstname", chkFirstnameMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsureLastname", chkLastnameMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsureOrganization", chkOrganizationMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsureStreet", chkStreetMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsureCity", chkCityMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsurePostalCode", chkPostalCodeMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsurePhone", chkPhoneMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsureFax", chkFaxMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsureEmail", chkEmailMandatory.Checked.ToString());
                module.UpdateModuleSetting(this.ModuleId, "EnsureRemark", chkRemarkMandatory.Checked.ToString());
            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

    }

}

