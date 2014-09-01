using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;

namespace Bitboxx.DNNModules.BBBooking
{

    /// <summary>
    /// The Edit class lets the administrator edit the bookings
    /// </summary>

    [DNNtc.PackageProperties("BBBooking")]
    [DNNtc.ModuleProperties("BBBooking")]
    [DNNtc.ModuleControlProperties("Edit", "BBBooking", DNNtc.ControlType.Edit, "", false, false)]
    public partial class Edit : PortalModuleBase
    {
        private readonly BBBookingController Controller = new BBBookingController();
        private List<int> _allowedDays; 

        public int SelectedYear
        {
            get { return Convert.ToInt32(ddlYear.SelectedItem.Value); }
        }

        public int MinDays
        {
            get { return Convert.ToInt32(Settings["MinDays"] ?? "1"); }
        }
        
        public int MinOffset
        {
            get { return Convert.ToInt32(Settings["MinOffset"] ?? "0"); }
        }

        public List<int> AllowedDays
        {
            get
            {
                if (_allowedDays == null)
                {
                    _allowedDays = new List<int>();
                    string[] days;
                    if (String.IsNullOrWhiteSpace((string) Settings["AllowedDays"]))
                        days = "0,1,2,3,4,5,6".Split(',');
                    else
                        days = ((string) Settings["AllowedDays"]).Split(',');

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
                BookingInfo booking;

                int bookingId = Convert.ToInt32(hidBookingId.Value);
                if (bookingId > 0)
                {
                    booking = Controller.GetBooking(bookingId);
                }
                else
                {
                    booking = new BookingInfo()
                              {
                                  CreatedByUserID = UserId,
                                  LastModifiedByUserID = UserId,
                                  CreatedOnDate = DateTime.Now,
                                  LastModifiedOnDate = DateTime.Now,
                                  ModuleID = ModuleId,
                                  PortalID = PortalId
                              };
                }
                booking.Startdate = dtpStartdate.SelectedDate ?? new DateTime(1900, 1, 1);
                booking.Enddate = dtpEnddate.SelectedDate ?? new DateTime(1900, 1, 1);
                booking.State = Convert.ToInt32(ddlState.SelectedValue);
                booking.UserID = Convert.ToInt32(ddlUser.SelectedValue);

                return booking;
            }
        }

        #region Event Handlers

        /// <summary>
        /// Runs when the control is initialized
        /// </summary>
        private void Page_Init(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    int year = DateTime.Now.Year;
                    for (int i = year - 5; i < year + 5; i++)
                    {
                        ddlYear.Items.Add(i.ToString());
                    }
                    ddlYear.SelectedIndex = 5;

                    UserInfo activeUser = UserController.GetCurrentUserInfo();
                    ArrayList alUsers = UserController.GetUsers(PortalId);
                    List<UserInfo> users = alUsers.Cast<UserInfo>().ToList();

                    var sortedUsers = (from u in users orderby u.Username select u);

                    if (!users.Contains(activeUser))
                        ddlUser.Items.Add(new ListItem(activeUser.DisplayName, activeUser.UserID.ToString()));

                    foreach (UserInfo user in sortedUsers)
                    {
                        ddlUser.Items.Add(new ListItem(user.DisplayName,user.UserID.ToString()));
                    }

                    ddlState.Items.Clear();
                    //ddlState.Items.Add(new ListItem(LocalizeString("State.Free"),"1"));
                    ddlState.Items.Add(new ListItem(LocalizeString("State.Reserved"),"2"));
                    ddlState.Items.Add(new ListItem(LocalizeString("State.Occupied"),"3"));
                    //ddlState.Items.Add(new ListItem(LocalizeString("State.Selected"),"4"));
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
        /// <summary>
        /// Runs when the control is loaded
        /// </summary>
        private void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    BindData();
                }

            }
            catch (Exception exc) //Module failed to load
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void ddlYear_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            BindData();
        }

        protected void grdBookings_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            int bookingId; 
            BookingInfo booking;
            grdBookings.SelectedIndex = -1;
            phMessage.Controls.Clear();
            switch (e.CommandName)
            {
                case "New":
                    pnlEdit.Visible = true;
                    hidBookingId.Value = "-1";

                    DateTime date = (SelectedYear == DateTime.Now.Year) ? DateTime.Now.Date : new DateTime(SelectedYear, DateTime.Now.Month, DateTime.Now.Day);
                    while (!AllowedDays.Contains((int) date.DayOfWeek) || date < DateTime.Now.Date.AddDays(MinOffset))
                    {
                        date = date.AddDays(1);
                    }
                    dtpStartdate.SelectedDate = date;
                    dtpEnddate.SelectedDate = date.AddDays(MinDays);
                    ddlState.SelectedValue = "2";
                    BindData();
                    break;
                
                case "Edit":
                    pnlEdit.Visible = true;
                    bookingId = Convert.ToInt32(e.CommandArgument);
                    booking = Controller.GetBooking(bookingId);
                    grdBookings.SelectedIndex = e.Item.ItemIndex;

                    hidBookingId.Value = booking.BookingId.ToString();
                    dtpStartdate.SelectedDate = booking.Startdate;
                    dtpEnddate.SelectedDate = booking.Enddate;
                    if (ddlUser.Items.FindByValue(booking.UserID.ToString()) != null)
                        ddlUser.SelectedValue = booking.UserID.ToString();
                    if (ddlState.Items.FindByValue(booking.State.ToString()) != null)
                        ddlState.SelectedValue = booking.State.ToString();
                    BindData();
                    break;

                case "Delete":
                    pnlEdit.Visible = false;
                    bookingId = Convert.ToInt32(e.CommandArgument);
                    booking = Controller.GetBooking(bookingId);
                    try
                    {
                        Controller.DeleteBooking(booking);
                    }
                    catch (Exception)
                    {
                        string message = LocalizeString("DeleteBooking.Error");
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, message, ModuleMessage.ModuleMessageType.YellowWarning);
                    }
                    BindData();
                    break;
            }
        }

        protected void cmdCheckSave_OnClick(object sender, EventArgs e)
        {
            phMessage.Controls.Clear();
            string valid = Controller.ValidateBooking(ModuleId,SelectedBooking, AllowedDays,MinOffset, MinDays);

            if (!String.IsNullOrEmpty(valid))
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "dnnFormMessage dnnFormWarning");
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.InnerHtml = valid;
                div.Controls.Add(span);
                phMessage.Controls.Add(div);
            }
            else
            {
                HtmlGenericControl div = new HtmlGenericControl("div");
                div.Attributes.Add("class", "dnnFormMessage dnnFormSuccess");
                HtmlGenericControl span = new HtmlGenericControl("span");
                span.InnerHtml = LocalizeString("NoError.Error");
                div.Controls.Add(span);
                phMessage.Controls.Add(div);
            }
            cmdSave.Enabled = true;
            BindData();
        }

        protected void cmdSave_OnClick(object sender, EventArgs e)
        {
            if (SelectedBooking.BookingId > 0)
                Controller.UpdateBooking(SelectedBooking);
            else
                Controller.InsertBooking(SelectedBooking);

            pnlEdit.Visible = false;
            grdBookings.SelectedIndex = -1;
            BindData();
            cmdSave.Enabled = false;
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId));
        }

        #endregion

        #region Helper methods

        
        
        public void BindData()
        {
            Localization.LocalizeDataGrid(ref grdBookings, this.LocalResourceFile);
            grdBookings.DataSource = Controller.GetBookingsByYear(ModuleId, Convert.ToInt32(SelectedYear));
            grdBookings.DataBind();
            imgSchedule.ImageUrl = "/bbimagehandler.ashx?schedule=year&culture=" + CultureInfo.CurrentUICulture.Name + "&matrix=" + Controller.GetYearMatrix(ModuleId, SelectedYear, false);
        }

        public string GetStateName(int state)
        {
            switch (state)
            {
                case 0:
                    return "<span style=\"background-color:darkgrey;color:white;padding:5px;display:block;margin:0:\">" + LocalizeString("State.NotValid") + "</span>";
                case 1:
                    return "<span style=\"background-color:darkgreen;color:white;padding:5px;display:block;margin:0:\">" + LocalizeString("State.Free") + "</span>";
                case 2:
                    return "<span style=\"background-color:orange;color:black;padding:5px;display:block;margin:0:\">" + LocalizeString("State.Reserved") + "</span>";
                case 3:
                    return "<span style=\"background-color:red;color:white;padding:5px;display:block;margin:0:\">" + LocalizeString("State.Occupied") + "</span>";
                case 4:
                    return "<span style=\"background-color:blue;color:white;padding:5px;display:block;margin:0:\">" + LocalizeString("State.Selected") + "</span>";
                default:
                    return LocalizeString("State.Unknown");

            }
        }

        public string GetUserName(int userId)
        {
            return UserController.GetUserById(PortalId, userId).DisplayName;
        }

        public string GetUserInfo(int userId)
        {
            UserInfo user =  UserController.GetUserById(PortalId, userId);
            StringBuilder sb = new StringBuilder("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\">");
            sb.AppendFormat("<tr><td><b>{0}</b></td><td>&nbsp;</td><td>{1}</td></tr>", LocalizeString("TipName.Text"), user.DisplayName);
            sb.AppendFormat("<tr><td><b>{0}</b></td><td>&nbsp;</td><td>{1}</td></tr>", LocalizeString("TipStreet"), user.Profile.Street);
            sb.AppendFormat("<tr><td><b>{0}</b></td><td>&nbsp;</td><td>{1}</td></tr>", LocalizeString("TipCity"), user.Profile.City);
            sb.AppendFormat("<tr><td><b>{0}</b></td><td>&nbsp;</td><td>{1}</td></tr>", LocalizeString("TipPostalcode"), user.Profile.PostalCode);
            sb.AppendFormat("<tr><td><b>{0}</b></td><td>&nbsp;</td><td>{1}</td></tr>", LocalizeString("TipPhone"), user.Profile.Telephone);
            sb.AppendFormat("<tr><td><b>{0}</b></td><td>&nbsp;</td><td>{1}</td></tr>", LocalizeString("TipEmail"), user.Email);
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        #endregion
    }
}
