using System;
using System.Collections;
using System.Globalization;
using System.Text;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Data;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Search;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Bitboxx.DNNModules.BBBooking
{

    /// ----------------------------------------------------------------------------- 
    /// <summary> 
    /// The Controller class for BBBooking 
    /// </summary> 
    /// <remarks> 
    /// </remarks> 
    /// <history> 
    /// </history> 
    /// ----------------------------------------------------------------------------- 

    // [DNNtc.UpgradeEventMessage("01.01.01,04.00.02,04.01.00")]
    [DNNtc.BusinessControllerClass()]
    public class BBBookingController // : IPortable
    {
        #region Booking

        public IEnumerable<BookingInfo> GetBookings(int moduleID)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<BookingInfo>();
                return repository.Find("WHERE moduleId = @0", moduleID);
            }
        }

        public IEnumerable<BookingInfo> GetBookingsByYear(int moduleID, int year)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<BookingInfo>();
                return repository.Find("WHERE moduleId = @0 AND (YEAR(startdate) = @1 OR YEAR(Enddate) = @1) ", moduleID, year).OrderBy(b => b.Startdate);
            }
        }

        public BookingInfo GetBooking(int bookingId)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<BookingInfo>();
                return repository.Find("WHERE BookingId = @0", bookingId).FirstOrDefault();
            }
        }

        public int InsertBooking(BookingInfo booking)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<BookingInfo>();
                repository.Insert(booking);
                return booking.BookingId;
            }
        }
        public void UpdateBooking(BookingInfo booking)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<BookingInfo>();
                repository.Update(booking);
            }
        }
        public void DeleteBooking(BookingInfo booking)
        {
            using (IDataContext context = DataContext.Instance())
            {
                var repository = context.GetRepository<BookingInfo>();
                repository.Delete(booking);
            }
        }

        public string ValidateBooking(int moduleID, BookingInfo booking, List<int> allowedDays, int minOffset, int minDays)
        {
            string resourceFile = Globals.ResolveUrl("~/DesktopModules/bbbooking/App_LocalResources/Edit.ascx.resx");
            string result = "";

            if (booking.Startdate.Date > booking.Enddate.Date)
                result += Localization.GetString("EndBeforeStart.Error", resourceFile) + "<br/>";

            IEnumerable<BookingInfo> savedBookings = GetBookings(moduleID)
                .Where(b => ((b.Startdate.Date <= booking.Startdate.Date && b.Enddate.Date > booking.Startdate.Date) ||
                            (b.Startdate.Date < booking.Enddate.Date && b.Enddate.Date >= booking.Enddate.Date) ||
                            (b.Startdate.Date >= booking.Startdate.Date && b.Enddate.Date <= booking.Enddate.Date)) &&
                             b.BookingId != booking.BookingId);

            if (savedBookings.Any())
                result += String.Format(Localization.GetString("Overlapping.Error", resourceFile)) + "<br/>";

            if (!allowedDays.Contains((int)booking.Startdate.DayOfWeek))
            {
                StringBuilder sb = new StringBuilder();
                foreach (int allowedDay in allowedDays)
                {
                    sb.Append(CultureInfo.CurrentCulture.DateTimeFormat.DayNames[allowedDay] + ",");
                }
                string ad = sb.ToString();
                if (ad.Length > 0)
                    ad = ad.Substring(0, ad.Length - 1);
                result += String.Format(Localization.GetString("AllowedDays.Error", resourceFile), ad) + "<br/>";
            }

            if (booking.Startdate.Date - DateTime.Now.Date < new TimeSpan(minOffset, 0, 0, 0))
                result += String.Format(Localization.GetString("MinOffset.Error", resourceFile), DateTime.Now.Date + new TimeSpan(minOffset, 0, 0, 0)) + "<br/>";

            if (booking.Enddate.Date - booking.Startdate.Date < new TimeSpan(minDays, 0, 0, 0))
                result += String.Format(Localization.GetString("MinDays.Error", resourceFile), minDays) + "<br/>";

            if (result.Length > 5)
                result = result.Substring(0, result.Length - 5);

            return result;
        }

        #endregion

        public string GetYearMatrix(int moduleId, int year, bool showOnlyFutureDates, DateTime? selectStart = null, DateTime? selectEnd = null )
        {
            List<BookingInfo> bookings = GetBookingsByYear(moduleId, year).ToList();

            if (selectStart != null && selectEnd != null)
            {
                bookings.Add(new BookingInfo()
                             {
                                 BookingId = -1,
                                 ModuleID = moduleId,
                                 PortalID = PortalSettings.Current.PortalId,
                                 Startdate = (DateTime)selectStart,
                                 Enddate = (DateTime)selectEnd,
                                 State = 4,
                                 UserID = PortalSettings.Current.UserId
                             });
            }

            StringBuilder sb = new StringBuilder();

            for (int month = 1; month < 13; month++)
            {
                for (int day = 1; day < 32; day++)
                {
                    try
                    {
                        int state = 0;
                        DateTime date = new DateTime(year, month, day);
                        if (!showOnlyFutureDates || date >= DateTime.Now.Date)
                        {
                            state = 1;
                            BookingInfo hit = (from b in bookings
                                where b.Startdate <= date && b.Enddate > date
                                select b).FirstOrDefault();
                            if (hit != null)
                                state = hit.State;

                            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                                state += 5;
                        }

                        sb.Append(state.ToString());
                    }
                    catch (Exception)
                    {
                        sb.Append("0");
                    }
                }
            }
            return sb.ToString();
        }


        //#region Implementation of IPortable

        ///// ----------------------------------------------------------------------------- 
        ///// <summary> 
        ///// ExportModule implements the IPortable ExportModule Interface 
        ///// </summary> 
        ///// <remarks> 
        ///// </remarks> 
        ///// <param name="moduleID">The Id of the module to be exported</param> 
        ///// <history> 
        ///// </history> 
        ///// ----------------------------------------------------------------------------- 
        //public string ExportModule(int moduleID) 
        //{ 
        //    IEnumerable<BookingInfo> colBBBookings = GetAllBBBookings(moduleID); 
        //    string strXML = "<BBBookings>"; 
        //    foreach ( BookingInfo objBBBooking in colBBBookings) 
        //    { 
        //        strXML += "<BBBooking>"; 
        //        strXML += "<name>" +XmlUtils.XMLEncode(objBBBooking.Name) + "</name>"; 
        //        strXML += "</BBBooking>"; 
        //    } 
        //    strXML += "</BBBookings>"; 
        //    return strXML; 
        //}

        ///// ----------------------------------------------------------------------------- 
        ///// <summary> 
        ///// ImportModule implements the IPortable ImportModule Interface 
        ///// </summary> 
        ///// <remarks> 
        ///// </remarks> 
        ///// <param name="moduleID">The Id of the module to be imported</param> 
        ///// <param name="name">The name to be imported</param> 
        ///// <param name="version">The version of the module to be imported</param> 
        ///// <param name="userId">The Id of the user performing the import</param> 
        ///// <history> 
        ///// </history> 
        ///// ----------------------------------------------------------------------------- 
        //public void ImportModule(int moduleID, string name, string version, int userId) 
        //{ 
        //    
        //    XmlNode xmlBBBookings = Globals.GetContent(name, "BBBookings"); 
        //    foreach ( XmlNode xmlBBBooking in xmlBBBookings.SelectNodes("BBBooking")) 
        //    { 
        //        BookingInfo objBBBooking = new BookingInfo(); 
        //        objBBBooking.ModuleId = moduleID; 
        //        objBBBooking.Name = xmlBBBooking.SelectSingleNode("name").InnerText; 
        //        objBBBooking.CreatedByUserID = userId; 
        //        InsertBBBooking(objBBBooking); 
        //    } 
        //}

        //#endregion

    }
}