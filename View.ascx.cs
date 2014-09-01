using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mail;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security.Membership;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Authentication;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Mail;
using DotNetNuke.UI.Skins.Controls;

namespace Bitboxx.DNNModules.BBBooking
{

    /// <summary>
    /// The View class displays the content
    /// </summary>

    [DNNtc.PackageProperties("BBBooking", 1, "BBBooking", "BBBooking", "", "Torsten Weggen", "bitboxx solutions", "http://www.bitboxx.net", "info@bitboxx.net", true)]
    [DNNtc.ModuleProperties("BBBooking", "BBBooking", 0)]
    [DNNtc.ModuleDependencies(DNNtc.ModuleDependency.CoreVersion, "07.00.01")]
    [DNNtc.ModuleControlProperties("", "BBBooking", DNNtc.ControlType.View, "", false, false)]
    public partial class View : PortalModuleBase, IActionable
    {
        private readonly BBBookingController Controller = new BBBookingController();

        private List<int> _allowedDays;

        public int SelectedYear
        {
            get
            {
                if (ViewState["SelectedYear"] != null)
                    return Convert.ToInt32(ViewState["SelectedYear"]);
                return DateTime.Now.Year;
            }
            set
            {
                ViewState["SelectedYear"] = value;
            }
        }

        public int MinDays
        {
            get { return Convert.ToInt32(Settings["MinDays"] ?? "1"); }
        }

        public int MinOffset
        {
            get { return Convert.ToInt32(Settings["MinOffset"] ?? "0"); }
        }

        public bool OnlyFutureDays
        {
            get { return Convert.ToBoolean(Settings["OnlyFutureDays"] ?? "false"); }
        }

        public List<int> AllowedDays
        {
            get
            {
                if (_allowedDays == null)
                {
                    _allowedDays = new List<int>();
                    string[] days;
                    if (String.IsNullOrWhiteSpace((string)Settings["AllowedDays"]))
                        days = "0,1,2,3,4,5,6".Split(',');
                    else
                        days = ((string)Settings["AllowedDays"]).Split(',');

                    foreach (string day in days)
                    {
                        _allowedDays.Add(Convert.ToInt32(day));
                    }
                }
                return _allowedDays;
            }
        }

        public BookingInfo SelectedBooking
        {
            get
            {
                return new BookingInfo()
                {
                    CreatedByUserID = UserId,
                    LastModifiedByUserID = UserId,
                    CreatedOnDate = DateTime.Now,
                    LastModifiedOnDate = DateTime.Now,
                    ModuleID = ModuleId,
                    PortalID = PortalId,
                    Startdate = dtpStartdate.SelectedDate ?? new DateTime(1900, 1, 1),
                    Enddate = dtpEnddate.SelectedDate ?? new DateTime(1900, 1, 1),
                    State = 0,
                    UserID = UserId
                };
            }
        }

        #region Event Handlers

        /// <summary>
        /// Runs when the control is loaded
        /// </summary>
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    DateTime date = (SelectedYear == DateTime.Now.Year) ? DateTime.Now.Date : new DateTime(SelectedYear, DateTime.Now.Month, DateTime.Now.Day);
                    while (!AllowedDays.Contains((int)date.DayOfWeek) || date < DateTime.Now.Date.AddDays(MinOffset))
                    {
                        date = date.AddDays(1);
                    }
                    dtpStartdate.SelectedDate = date;
                    dtpEnddate.SelectedDate = date.AddDays(MinDays);

                    valEmailOK.ValidationExpression = Globals.glbEmailRegEx;

                    SetVisibility("Firstname");
                    SetVisibility("Lastname");
                    SetVisibility("Organization");
                    SetVisibility("Street");
                    SetVisibility("City");
                    SetVisibility("PostalCode");
                    SetVisibility("Phone");
                    SetVisibility("Fax");
                    SetVisibility("Email");
                    SetVisibility("Remark");
                    
                    BindData();
                    ShowInfo();
                    mvBooking.SetActiveView(vwCheck);
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void lnkPrevYear_OnClick(object sender, EventArgs e)
        {
            SelectedYear = SelectedYear - 1;
            BindData();
        }

        protected void lnkNextYear_OnClick(object sender, EventArgs e)
        {
            SelectedYear = SelectedYear + 1;
            BindData();
        }

        protected void cmdCheck_OnClick(object sender, EventArgs e)
        {
            string valid = Controller.ValidateBooking(ModuleId, SelectedBooking, AllowedDays, MinOffset, MinDays);
            if (!String.IsNullOrWhiteSpace(valid))
            {
                ShowMessage(phMessage,valid,ModuleMessage.ModuleMessageType.RedError);
            }
            else
            {
                mvBooking.SetActiveView(vwUserdata);
            }
            BindData();
        }

        protected void cmdChangeSelection_OnClick(object sender, EventArgs e)
        {
            mvBooking.SetActiveView(vwCheck);
            BindData();
        }

        protected void cmdSend_Click(object sender, EventArgs e)
        {
            try
            {
                UserInfo user = UserController.GetUserByName(txtEmail.Text.Trim()) ?? CreateUser();
                SaveBooking(user);
                SendBookingMail();
                ShowMessage(phConfirm, LocalizeString("ResultOK.Text"), ModuleMessage.ModuleMessageType.GreenSuccess);
                txtFirstname.Text = "";
                txtLastname.Text = "";
                txtOrganization.Text = "";
                txtStreet.Text = "";
                txtCity.Text = "";
                txtPostalCode.Text = "";
                txtPhone.Text = "";
                txtFax.Text = "";
                txtEmail.Text = "";
                txtRemark.Text = "";
                BindData();
            }
            catch (Exception ex)
            {
                ShowMessage(phConfirm, LocalizeString("ResultError.Text"), ModuleMessage.ModuleMessageType.RedError);
            }
        }

        #endregion

        public void BindData()
        {
            if (!OnlyFutureDays || SelectedYear - 1 >= DateTime.Now.Year)
            {
                lnkPrevYear.Text = (SelectedYear - 1).ToString();
                lnkPrevYear.Visible = true;
            }
            else
            {
                lnkPrevYear.Text = "";
                lnkPrevYear.Visible = false;
            }
            lblYear.Text = (SelectedYear).ToString();
            lnkNextYear.Text = (SelectedYear + 1).ToString();
            if (mvBooking.ActiveViewIndex != 1)
                imgSchedule.ImageUrl = "/bbimagehandler.ashx?schedule=year&culture=" + CultureInfo.CurrentUICulture.Name + "&matrix=" + Controller.GetYearMatrix(ModuleId, SelectedYear, OnlyFutureDays);
            else
                imgSchedule.ImageUrl = "/bbimagehandler.ashx?schedule=year&culture=" + CultureInfo.CurrentUICulture.Name + "&matrix=" + Controller.GetYearMatrix(ModuleId, SelectedYear,OnlyFutureDays,dtpStartdate.SelectedDate,dtpEnddate.SelectedDate);
            lblSelectedTime.Text = String.Format("{0:D} -<br/> {1:D}", SelectedBooking.Startdate, SelectedBooking.Enddate);
        }

        private void SetVisibility(string fieldName)
        {
            Panel pnl = this.FindControl("pnl" + fieldName) as Panel;
            TextBox txt = this.FindControl("txt" + fieldName) as TextBox;
            RequiredFieldValidator val = this.FindControl("val" + fieldName) as RequiredFieldValidator;
            
            if (txt != null && pnl != null && val != null)
            {
                if (!String.IsNullOrEmpty((string)Settings["Visible" + fieldName]) && Convert.ToBoolean(Settings["Visible" + fieldName]))
                {
                    if (!String.IsNullOrEmpty((string)Settings["Ensure" + fieldName]) && Convert.ToBoolean(Settings["Ensure" + fieldName]))
                    {
                        pnl.CssClass += " dnnFormRequired";
                        txt.CssClass += " dnnFormRequired";
                        val.Visible = true;
                    }
                }
                else
                {
                    pnl.Visible = false;
                }
            }
        }

        private void ShowInfo()
        {
            StringBuilder infoText = new StringBuilder();
            if (AllowedDays.Count < 7)
            {
                StringBuilder sb = new StringBuilder();
                foreach (int allowedDay in AllowedDays)
                {
                    sb.Append(CultureInfo.CurrentCulture.DateTimeFormat.DayNames[allowedDay] + ",");
                }
                string ad = sb.ToString();
                if (ad.Length > 0)
                    ad = ad.Substring(0, ad.Length - 1);
                infoText.AppendFormat(LocalizeString("AllowedDays.Info") + "<br/>", ad);
            }
            if (MinDays > 0)
                infoText.AppendFormat(LocalizeString("MinDays.Info") + "<br/>", MinDays);

            infoText.AppendFormat(LocalizeString("MinOffset.Info") + "<br/>", DateTime.Now.AddDays(MinOffset));

            if (infoText.Length > 5)
                infoText = infoText.Remove(infoText.Length - 5,5);

            ShowMessage(phInfo,infoText.ToString(),ModuleMessage.ModuleMessageType.BlueInfo);
        }

        private void ShowMessage(PlaceHolder phMessage, string message, ModuleMessage.ModuleMessageType messageType)
        {
            HtmlGenericControl div = new HtmlGenericControl("div");
            switch (messageType)
            {
                case ModuleMessage.ModuleMessageType.GreenSuccess:
                    div.Attributes.Add("class", "dnnFormMessage dnnFormSuccess");
                    break;
                case ModuleMessage.ModuleMessageType.YellowWarning:
                    div.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                    break;
                case ModuleMessage.ModuleMessageType.RedError:
                    div.Attributes.Add("class", "dnnFormMessage dnnFormError");
                    break;
                case ModuleMessage.ModuleMessageType.BlueInfo:
                    div.Attributes.Add("class", "dnnFormMessage dnnFormInfo");
                    break;
                default:
                    throw new ArgumentOutOfRangeException("messageType");
            }
            
            HtmlGenericControl span = new HtmlGenericControl("span");
            span.InnerHtml = message;
            div.Controls.Add(span);
            phMessage.Controls.Add(div);
        }

        private UserInfo CreateUser()
        {
            UserInfo user = new UserInfo();
            user.Username = txtEmail.Text.Trim();
            user.FirstName = txtFirstname.Text.Trim();
            user.LastName = txtLastname.Text.Trim();
            user.PortalID = PortalId;
            user.Email = txtEmail.Text.Trim();
            user.DisplayName = user.FirstName + " " + user.LastName;
            string password = UserController.GeneratePassword(10);
            user.Membership.Password = password;
            user.IsSuperUser = false;
            user.Membership.Approved = false;

            user.Profile.InitialiseProfile(PortalId);
            user.Profile.PreferredLocale = PortalSettings.DefaultLanguage;
            user.Profile.PreferredTimeZone = PortalSettings.TimeZone;
            user.Profile.FirstName = user.FirstName;
            user.Profile.LastName = user.LastName;
            user.Profile.City = txtCity.Text.Trim();
            user.Profile.PostalCode = txtPostalCode.Text.Trim();
            user.Profile.Street = txtStreet.Text.Trim();

            UserCreateStatus status = MembershipProvider.Instance().CreateUser(ref user);

            if (status == UserCreateStatus.Success || status == UserCreateStatus.UsernameAlreadyExists)
            {
                if (status == UserCreateStatus.UsernameAlreadyExists)
                    UserController.AddUserPortal(PortalId, user.UserID);

                //// Add User to Standard Roles
                //RoleController roleController = new RoleController();
                //ArrayList roles = roleController.GetPortalRoles(PortalId);
                //for (int i = 0; i < roles.Count - 1; i++)
                //{
                //    RoleInfo role = (RoleInfo) roles[i];
                //    if (role.AutoAssignment == true)
                //        roleController.AddUserRole(PortalId, user.UserID, role.RoleID, Null.NullDate, Null.NullDate);
                //}
            }
            return user;
        }

        private void SaveBooking(UserInfo user)
        {
            BookingInfo booking = SelectedBooking;
            booking.UserID = user.UserID;
            booking.CreatedByUserID = user.UserID;
            booking.LastModifiedByUserID = user.UserID;
            booking.State = 2;
            Controller.InsertBooking(booking);
            mvBooking.SetActiveView(vwConfirm);
        }
        private void SendBookingMail()
        {
            string senderMail = (string)Settings["SenderEmail"] ?? "";
            string senderName = (string)Settings["SenderName"] ?? "";
            string subject = (string)Settings["Subject"] ?? "Booking form submission from [EMAIL]";

            string recipientEmail = (string)Settings["RecipientEmail"] ?? "";

            try
            {
                // http://www.systemnetmail.com

                MailMessage mail = new MailMessage();

                //set the addresses
                string smtpServer = DotNetNuke.Entities.Host.Host.SMTPServer;
                string smtpAuthentication = DotNetNuke.Entities.Host.Host.SMTPAuthentication;
                string smtpUsername = DotNetNuke.Entities.Host.Host.SMTPUsername;
                string smtpPassword = DotNetNuke.Entities.Host.Host.SMTPPassword;

                mail.From = new MailAddress("\"" + senderName.Trim() + "\" <" + senderMail.Trim() + ">");
                mail.To.Add(recipientEmail);
                if (Settings["SendToUser"] != null && Convert.ToBoolean(Settings["SendToUser"]))
                    mail.To.Add(txtEmail.Text.Trim());

                mail.Subject = subject.Replace("[FIRSTNAME]", txtFirstname.Text.Trim())
                    .Replace("[LASTNAME]", txtLastname.Text.Trim())
                    .Replace("[ORGANIZATION]", txtOrganization.Text.Trim())
                    .Replace("[STREET]", txtStreet.Text.Trim())
                    .Replace("[CITY]", txtCity.Text.Trim())
                    .Replace("[POSTALCODE]", txtPostalCode.Text.Trim())
                    .Replace("[PHONE]", txtPhone.Text.Trim())
                    .Replace("[FAX]", txtFax.Text.Trim())
                    .Replace("[EMAIL]", txtEmail.Text.Trim())
                    .Replace("[REMARK]", txtRemark.Text.Trim())
                    .Replace("[STARTDATE]", SelectedBooking.Startdate.ToString("d"))
                    .Replace("[ENDDATE]", SelectedBooking.Enddate.ToString("d"));

                mail.Body = ((string)Settings["Bodytext"])
                    .Replace("[FIRSTNAME]", txtFirstname.Text.Trim())
                    .Replace("[LASTNAME]", txtLastname.Text.Trim())
                    .Replace("[ORGANIZATION]", txtOrganization.Text.Trim())
                    .Replace("[STREET]", txtStreet.Text.Trim())
                    .Replace("[CITY]", txtCity.Text.Trim())
                    .Replace("[POSTALCODE]", txtPostalCode.Text.Trim())
                    .Replace("[PHONE]", txtPhone.Text.Trim())
                    .Replace("[FAX]", txtFax.Text.Trim())
                    .Replace("[EMAIL]", txtEmail.Text.Trim())
                    .Replace("[REMARK]", txtRemark.Text.Trim())
                    .Replace("[STARTDATE]", SelectedBooking.Startdate.ToString("d"))
                    .Replace("[ENDDATE]", SelectedBooking.Enddate.ToString("d"));


                SmtpClient emailClient = new SmtpClient(smtpServer);
                if (smtpAuthentication == "1")
                {
                    System.Net.NetworkCredential SMTPUserInfo = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                    emailClient.UseDefaultCredentials = false;
                    emailClient.Credentials = SMTPUserInfo;
                }
                emailClient.Send(mail);
            }
            catch (SmtpException sex)
            {
                ShowMessage(phConfirm,sex.Message,ModuleMessage.ModuleMessageType.RedError);
                Exceptions.LogException(sex);
            }
            catch (Exception ex)
            {
                ShowMessage(phConfirm, ex.Message, ModuleMessage.ModuleMessageType.RedError);
                Exceptions.LogException(ex);
            }
        }

        #region Implementation of IActionable

        /// ----------------------------------------------------------------------------- 
        /// <summary> 
        /// Registers the module actions required for interfacing with the portal framework 
        /// </summary> 
        /// <value></value> 
        /// <returns></returns> 
        /// <remarks></remarks> 
        /// <history> 
        /// </history> 
        /// ----------------------------------------------------------------------------- 
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection Actions = new ModuleActionCollection();
                Actions.Add(
                    GetNextActionID(),
                    Localization.GetString(ModuleActionType.EditContent, this.LocalResourceFile),
                    ModuleActionType.EditContent,
                    "",
                    "Edit.gif",
                    EditUrl(),
                    false,
                    DotNetNuke.Security.SecurityAccessLevel.Edit,
                    true,
                    false);
                return Actions;
            }
        }


        #endregion
    }

}
