using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;

using Insights.Models;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

using Insights.DB_Helper;

namespace Insights.Controllers
{
    public class HomeController : Controller
    {

        static string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private DateTime currentDate;

        //Employee landing page view
        public ActionResult Dashboard()
        {
            if (Session["refNo"] != null)
            {
                return View();
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Manager landing page view
        public ActionResult ManagerDashboard()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    DateTime now = DateTime.Now;
                    var startDate = new DateTime(now.Year, now.Month, 1);
                    DateTime newDate = Convert.ToDateTime(startDate);

                    JsonResult result = DASHBOARDBAR(newDate, now);

                    ViewBag.Details = result;

                    return View();
                }
                else
                    return RedirectToAction("logout", "home");

            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

       

        public ActionResult ShiftManagerDashboard()
        {
            if (Session["refNo"] != null)
            {
                return View();
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Monthly chart page view
        public ActionResult MonthlyChart()
        {
                if (Session["refNo"] != null)
                {
                    return View();
                }
                else
                {
                    ViewBag.Message = "Session Time Out";
                    return RedirectToAction("logout", "home");
                }
        }

        public ActionResult ManageMLS()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        public ActionResult Manageplatforms()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        public ActionResult MLSMap()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }else
                    return RedirectToAction("logout", "home");
                
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        public ActionResult ZipcodeMap()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        public ActionResult SearchByAddress()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Form submit - Search Address (Search button)
        [HttpPost]
        public ActionResult SearchByAddress(string address)
        {

            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    JsonResult Details = getAddressDetails(address);

                    ViewBag.AddressData = Details;

                    Session["address"] = address;

                    ViewBag.address = address;

                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }



        public ActionResult Login2()
        {
            ViewBag.Message = "Your application description page.";

            return View();


        }

        //Orderlimitdetails page view
        public ActionResult OrderLimitDetails()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }else
                    return RedirectToAction("logout", "home");
                
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Onclick function - Employee landing page menu bar (Completed,Pending order count)
        public ActionResult StatusWiseOrders()
        {
            if (Session["refNo"] != null)
            {
                return RedirectToAction("MyOrders", "home");
            }
            else
                return RedirectToAction("logout", "home");

        }

        public ActionResult DayWiseOrderCount()
        {
            if (Session["refNo"] != null)
            {
                Int64 refNo = Convert.ToInt64(Session["refNo"]);

                JsonResult Details = new JsonResult();

                if (Convert.ToInt64(Session["role"]) != 2 && Convert.ToInt64(Session["role"]) != 1)
                {

                    string CurrentMonth = DateTime.Now.Month.ToString();

                    string CurrentYear = DateTime.Now.Year.ToString();

                    Details = getDayWiseOrders(refNo, CurrentMonth, CurrentYear);

                    ViewBag.month = CurrentMonth;

                    ViewBag.year = CurrentYear;
                }
                else
                {
                    Details = null;

                    TempData["employee"] = 0;
                }

                ViewBag.DayWiseData = Details;

                return View();
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        [HttpPost]
        public ActionResult DayWiseOrderCount(Int64? empRef,string month,string year)
        {

            if (Session["refNo"] != null)
            {
                Int64 refNo = Convert.ToInt64(Session["refNo"]);

                JsonResult Details = new JsonResult();

                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {

                    //string CurrentMonth = DateTime.Now.Month.ToString();

                    //string CurrentYear = DateTime.Now.Year.ToString();

                    Details = getDayWiseOrders(empRef, month, year);

                    ViewBag.month = month;

                    ViewBag.year = year;

                    TempData["employee"] = empRef;
                }
                else
                {
                    //string CurrentMonth = DateTime.Now.Month.ToString();

                    //string CurrentYear = DateTime.Now.Year.ToString();


                    Details = getDayWiseOrders(refNo, month, year);

                    ViewBag.month = month;

                    ViewBag.year = year;
                }

                ViewBag.DayWiseData = Details;

                return View();
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        public ActionResult ResearchTraineeBatch()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
                //JsonResult Details = getBatchWiseDetails(1);
                //ViewBag.Data = Details;

                
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        public ActionResult BatchInfo()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
                
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //My Orders page view
        public ActionResult MyOrders()
        {
            if (Session["refNo"] != null)
            {
                if(TempData["details"] == "Revision"){
                    TempData["details"] = null;
                }
                
                //DateTime Date = DateTime.Now.AddDays(1).Date.AddSeconds(-1);
                ////DateTime Date = DateTime.Now.AddDays(1);
                //var date = Date.ToString();
                //DateTime DateNow = DateTime.Now.Date;
                //currentDate = DateNow.AddDays(-30);
                //var newDate = currentDate.ToString();
                //TempData["StartDate"] = newDate;
                //TempData["EndDate"] = date;
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                DateTime newDate = Convert.ToDateTime(startDate);

                TempData["StartDate"] = startDate;
                TempData["EndDate"] = now;

                //DateTime endDate = DateTime.Parse(date);
                //DateTime startDate = DateTime.Parse(newDate);

                if (TempData["details"] == null)
                {
                    JsonResult Details = getemployeeincentivedetails(startDate, now, 1);
                    ViewBag.Data = Details;
                }
                else
                {
                    ViewBag.Data = TempData["details"];
                }

                if (Session["crStatus"]==null)
                {
                    Session["crStatus"] = 1;
                }

                if (Session["StartDate"] != null && Session["EndDate"] != null)
                {
                    TempData["StartDate"] = Session["StartDate"];
                    TempData["EndDate"] = Session["EndDate"];
                }
                

                return View();
            }
            else
                return RedirectToAction("logout", "home");
        }

        //Form submit - My Orders (Generate button)
        [HttpPost]
        public ActionResult MyOrders(DateTime StartDate, DateTime EndDate, int currentStatus)
        {
            if (Session["refNo"] != null)
            {
                if (currentStatus == 3)
                {
                    DateTime EndDates = EndDate.AddDays(1).Date.AddSeconds(-1);
                    TempData["StartDate"] = StartDate;
                    Session["StartDate"] = StartDate;

                    TempData["EndDate"] = EndDates;
                    Session["EndDate"] = EndDates;

                    Session["crStatus"] = currentStatus;

                    ViewBag.Data = "Revision";
                    TempData["details"] = ViewBag.Data;
                }
                else
                {
                    DateTime EndDates = EndDate.AddDays(1).Date.AddSeconds(-1);

                    JsonResult empData = getemployeeincentivedetails(StartDate, EndDates, currentStatus);
                    ViewBag.Data = empData;
                    TempData["details"] = ViewBag.Data;

                    TempData["StartDate"] = StartDate;
                    Session["StartDate"] = StartDate;

                    TempData["EndDate"] = EndDates;
                    Session["EndDate"] = EndDates;

                    //TempData["crStatus"] = currentStatus;
                    Session["crStatus"] = currentStatus;

                    
                }
                return View();

            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //My Incentive Page View
        public ActionResult MyIncentive()
        {
            if (Session["refNo"] != null)
            {
                ViewBag.Message = "Your contact page.";
                DateTime Date = DateTime.Now.AddDays(1).Date.AddSeconds(-1);
                var date = Date.ToString();
                DateTime DateNow = DateTime.Now.Date;
                currentDate = DateNow.AddDays(-30);
                var newDate = currentDate.ToString();
                TempData["StartDate"] = newDate;
                TempData["EndDate"] = date;

                return View();
            }
            else
                return RedirectToAction("logout", "home");

        }

        //Form submit - My Incentive (Generate button)
        [HttpPost]
        public ActionResult MyIncentive(DateTime StartDate, DateTime EndDate)
        {
            if (Session["refNo"] != null)
            {
                DateTime EndDates = EndDate.AddDays(1).Date.AddSeconds(-1);

                JsonResult empData = employeeIncentivedetails(StartDate, EndDates);
                ViewBag.Data = empData;

                    TempData["StartDate"] = StartDate;
                    TempData["EndDate"] = EndDates;
                    return View();
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }

        }


        //Manager Login
        //Incentive Report Page view
        public ActionResult IncentiveReport()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    ViewBag.Message = "Your incentive report page.";

                    DateTime now = DateTime.Now;

                    var startDate = new DateTime(now.Year, now.Month, 1);

                    DateTime newDate = Convert.ToDateTime(startDate);

                    TempData["StartDate"] = startDate;
                    TempData["EndDate"] = now;

                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
                
            }
            else
                return RedirectToAction("logout", "home");
        }

        ////Form submit - Incentive Report (Generate button)
        [HttpPost]
        public ActionResult IncentiveReport(DateTime StartDate, DateTime EndDate)
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    if (EndDate > StartDate)
                    {
                        DateTime EndDates = EndDate.AddDays(1).Date.AddSeconds(-1);

                        JsonResult empData = getincentiveamount(StartDate, EndDates);

                        ViewBag.Data = empData;
                        TempData["StartDate"] = StartDate;
                        TempData["EndDate"] = EndDate;
                        return View();

                    }
                    else
                    {
                        TempData["StartDate"] = StartDate;
                        TempData["EndDate"] = EndDate;
                        TempData["Result"] = "To Date should be greater than From date";
                        return View();
                    }
                }
                else
                    return RedirectToAction("logout", "home");
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Manager login
        //Monthly report page view
        public ActionResult MonthlyReport()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 3 || Convert.ToInt64(Session["role"]) == 1)
                {
                    ViewBag.Message = "Your contact page.";
                    DateTime Date = DateTime.Now;
                    var date = Date.ToString();
                    currentDate = Date.AddDays(-30);
                    var newDate = currentDate.ToString();
                    TempData["StartDate"] = newDate;
                    TempData["EndDate"] = date;

                    return View();
                }
                else
                    return RedirectToAction("logout", "home");  
            }
            else
                return RedirectToAction("logout", "home");  
        }

        //Form submit - Monthly report (Generate button)
        [HttpPost]
        public ActionResult MonthlyReport(Int64 empRef,DateTime StartDate, DateTime EndDate)
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 3 || Convert.ToInt64(Session["role"]) == 1)
                {
                    if (empRef != null && empRef != -1)
                    {
                        JsonResult empData = getemployeedetails(empRef, StartDate, EndDate);

                        ViewBag.Data = empData;
                        ViewBag.empRef = empRef;
                        TempData["StartDate"] = StartDate;
                        TempData["EndDate"] = EndDate;
                        return View();
                    }
                    else
                    {
                        JsonResult empData = getorderdetails(StartDate, EndDate);
                        ViewBag.Data = empData;
                        TempData["StartDate"] = StartDate;
                        TempData["EndDate"] = EndDate;
                        return View();
                    }
                }
                else
                    return RedirectToAction("logout", "home");

            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }   
        }

        public ActionResult EmployeeReport()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 3 || Convert.ToInt64(Session["role"]) == 1)
                {
                    DateTime now = DateTime.Now;
                    var startDate = new DateTime(now.Year, now.Month, 1);
                    //var endDate = startDate.AddMonths(1).AddDays(-1);


                    //DateTime Date = DateTime.Now;
                    //var date = Date.ToString();
                    //currentDate = Date.AddDays(-30);
                    //var newDate = currentDate.ToString();
                    //DateTime currentDate = Convert.ToDateTime(date);
                    DateTime newDate = Convert.ToDateTime(startDate);

                    DateTime EndDates = now.AddDays(1).Date.AddSeconds(-1);

                    TempData["StartDate"] = startDate;
                    TempData["EndDate"] = now;
                    JsonResult Details = getorderdetails_shift_manager(-1, newDate, EndDates);

                    ViewBag.Data = Details;

                    return View();
                }
                else
                    return RedirectToAction("logout", "home");

            }
            else
                return RedirectToAction("logout", "home");
        }

        //Form submit - Monthly report (Generate button)
        [HttpPost]
        public ActionResult EmployeeReport(Int64? empRef, DateTime StartDate, DateTime EndDate)
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 3 || Convert.ToInt64(Session["role"]) == 1)
                {

                    DateTime EndDates = EndDate.AddDays(1).Date.AddSeconds(-1);
                    JsonResult empData = getorderdetails_shift_manager(empRef, StartDate, EndDates);
                    ViewBag.Data = empData;
                    TempData["StartDate"] = StartDate;
                    TempData["EndDate"] = EndDate;
                    return View();
                }else
                    return RedirectToAction("logout", "home");


            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Weightage page view
        public ActionResult Weightage()
        {
            if (Session["refNo"] != null) 
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    JsonResult Details = getalldetails(5);

                    ViewBag.WeightageData = Details;

                    ViewBag.role = 5;

                    //TempData["curRole"] = Session["curRole"];

                    return View();
                }else
                    return RedirectToAction("logout", "home");  
                
            }
            else
                return RedirectToAction("logout", "home");  
        }

        //Form submit - Weightage (Generate button)
        [HttpPost]
        public ActionResult Weightage(int role)
        {

            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    JsonResult Details = getalldetails(role);

                    ViewBag.WeightageData = Details;
                    //Session["curRole"] = role;
                    //TempData["curRole"] = Session["curRole"];

                    Session["curRole"] = role;

                    ViewBag.role = role;

                    return View();
                }
                else
                    return RedirectToAction("logout", "home");  
                
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }



            
        }


        public ActionResult IncentiveVerification()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    int month = DateTime.Now.AddMonths(-1).Month;

                    string currentYear = DateTime.Now.Year.ToString();

                    int year = Int32.Parse(currentYear);

                    if (month == 12)
                    {
                        year = year - 1;
                    }
                        
                    JsonResult Details = getVerifyList(month, year);

                    ViewBag.VerifyData = Details;

                    ViewBag.month = month;

                    ViewBag.year = year;

                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
                
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        [HttpPost]
        public ActionResult IncentiveVerification(int month,int year)
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    JsonResult Details = getVerifyList(month, year);

                    ViewBag.VerifyData = Details;

                    ViewBag.month = month;

                    ViewBag.year = year;


                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
                
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Monthly invoice page view
        public ActionResult MonthlyInvoice()
        {
            if (Session["refNo"] != null)
            {
                return View();
            }
            else
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
        }

        public ActionResult IncentiveByMonth()
        {
            if (Session["refNo"] != null)
            {

                return View();
            }
            else
            {
                ViewBag.Message = "Session Time Out";
                return RedirectToAction("logout", "home");
            }
        }

        //Login page view
        public ActionResult Login()
        {
            return View();
        }

        

        [HttpPost]
        [AllowAnonymous]


        //Login page
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            model.Password = Encrypt_Password(model.Password); // Encrypt Password

            Int64 iResult = 0;

            if (!Regex.Match(model.Username.ToString(), @"^[a-zA-Z0-9/]+(?:[\\w -]*[a-zA-Z0-9/]+)*$").Success)
            {
                iResult = -2;
            }
            else
            {
                iResult = SqlProvider.CheckLogin(model);
            }

            if (iResult > 0)//Successful login
            {
                Users users = new Users();

                users = GetUserDetailsByUserID(iResult);

                if (users.role_id != 5 || users.role_id != 6)
                {
                    iResult = users.emp_ref_no;
                }
                else
                {
                    iResult = -1000;

                    string mailBody = "<html><body><p> Hello Admin, Employee code <b> " + model.Username + "</b> tries to login in Manager Count application at " + DateTime.Now + ". Please do the needfull.</p></body></html>";

                    // SendMail("anoop.a@ecesistech.com", "ramkfaizi97@gmail.com", "Manager Count Unauthorized login", mailBody);
                }
            }

            ////CommonMethods.SendMail("anu.vivinwlts@gmail.com", "Error Notify", ConfigurationManager.ConnectionStrings["BG_ConnectionString"].ConnectionString.ToString() + "," + iResult.ToString());
            if (iResult > 0)//Successful login
            {
                //FormsAuthentication.SetAuthCookie(iResult.ToString(), model.RememberMe);
                Session["userid"] = iResult.ToString();
                Users user = new Users();

                user = GetUserDetailsByUserID(iResult);

                //Session["username"] = users.Username;
                Session["role"] = user.role_id;
                Session["refNo"] = user.emp_ref_no;

                Session["emp_ref_code"] = user.emp_ref_code;

                Session["emp_full_name"] = user.emp_full_name;

                //Session["file_path"] = users.file_path;

                //Session["email"] = users.Email;
                //Session["availablestatus"] = users.Status;

                Session["loginRole"] = "";

                if (!string.IsNullOrEmpty(returnUrl) )
                    return RedirectToLocal(returnUrl);
                else if ((Convert.ToInt64(Session["role"]) == 2) || (Convert.ToInt64(Session["role"]) == 1))
                {
                    return RedirectToAction("ManagerDashboard", "Home");
                }
                else if ((Convert.ToInt64(Session["role"]) == 3))
                {
                    return RedirectToAction("ShiftManagerDashboard", "Home");
                }
                else
                    return RedirectToAction("Dashboard", "Home");

            }
            else if (iResult == -1)
                ViewBag.Message = "Password does not match";
            else if (iResult == -2)
                ViewBag.Message = "Invalid Entry Detected";
            else if (iResult == -4)
                ViewBag.Message = "Deleted";
            else if (iResult == -5)
                return RedirectToAction("forgotpassword", "home"); //login attempts fails";
            else if (iResult == -1000)
                ViewBag.Message = "You are not authorized to view this page. Unauthorized Access mail send to administrator.";

            ViewBag.ReturnUrl = returnUrl;

            //return RedirectToAction("Login", "Home");

  

            return View();
        }

        //Login Page
        //Checking the existance of user
        public Int64 CheckLogin(LoginViewModel model)
        {
            Int64 Result = SqlProvider.CheckLogin(model);

            return (Result);
        }

        public Users GetUserDetailsByUserID(Int64 iUserID)
        {
            Users getUser = SqlProvider.GetUserDetailsByUserID(iUserID);

            return (getUser);
            
        }

        //Login Page
        //Method to decrypt the password from database
        public static string Encrypt_Password(string password)
        {
            string pswstr = string.Empty;
            byte[] psw_encode = new byte[password.Length];
            psw_encode = System.Text.Encoding.UTF8.GetBytes(password);
            pswstr = Convert.ToBase64String(psw_encode);
            return pswstr;
        }

        //Method to implement session timeout
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        //Logout function
        public ActionResult Logout()
        {
            Session.Clear();//Clear all sessions

            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Home");
        }



        //All Modules
        //Method for tracking exception occured while functioning
        public string trackException(string controller, string exception)
        {
            Int64 refNo = Convert.ToInt64(Session["refNo"]);

            string Result = SqlProvider.trackException(controller, exception, refNo);

            return (Result);
        }


        //Method for appending drop down values
        //All Modules
        public JsonResult LoadTableData()
        {
            Int64 refNo = Convert.ToInt64(Session["refNo"]);

            
            if (Session["refNo"] != null)
            {
                object Result = SqlProvider.LoadTableData(refNo);

                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        public ActionResult StaticWeightage()
        {
            if (Session["refNo"] != null)
            {
                if (Convert.ToInt64(Session["role"]) == 2 || Convert.ToInt64(Session["role"]) == 1)
                {
                    return View();
                }
                else
                    return RedirectToAction("logout", "home");
            }
            else
                ViewBag.Message = "Session Time Out";
            return RedirectToAction("logout", "home");
        }

        //public JsonResult GetEmployeeData()
        //{
        //    var AjaxReturn = new
        //    {
        //        Message = "Error"
        //    };
        //    try
        //    {

        //        Int64 refNo = Convert.ToInt64(Session["refNo"]);

        //        string strClientList = "";
        //        string strPortalList = "";
        //        string strOrderTypeList = "";
        //        string strStateList = "";
        //        string strEmployeeList = "";
        //        DataSet data = new DataSet();
        //        SqlConnection sqlCon = new SqlConnection(connectionstring);
        //        SqlCommand sqlCmd = new SqlCommand();

        //        SqlCommand myCommand = new SqlCommand("PROC_GET_TABLE_EMPLOYEEVALUES");
        //        myCommand.CommandType = CommandType.StoredProcedure;
        //        myCommand.Connection = sqlCon;

        //        myCommand.Parameters.AddWithValue("@userId", refNo);
        //        SqlDataAdapter da = new SqlDataAdapter(myCommand);

        //        da.Fill(data);
        //        da.Dispose();
        //        sqlCon.Close();
        //        var AjaxData = new
        //        {

        //            strEmployeeList = Serialize(data.Tables[0])
        //            //strStatusReasonList = Serialize(data.Tables[4]),
        //            //strAutoBPOList = Serialize(data.Tables[5]),

        //        };

        //        return Json(AjaxData, JsonRequestBehavior.AllowGet);
        //    }
        //    catch
        //    {
        //        return Json(AjaxReturn, JsonRequestBehavior.AllowGet);
        //    }

        //}


        //Manager Login
        //To ADD ,EDIT,RETRIVE,DELETE weightage

        //Insert
        public JsonResult insertalldetails(int client, int state, int portal, int type, int software, float weightage, int role, int otherTypes, int mls_id)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_insertalldetails(client, state, portal, type, software, weightage, role, otherTypes, mls_id);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        //Retrive
        public JsonResult getalldetails(int role)
        {
            Int64 refNo = Convert.ToInt64(Session["refNo"]);

            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getalldetails(role, refNo);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        // Edit Retrival
        public JsonResult getweightagedetails(int wid)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getweightagedetails(wid);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }


        //Update 
        public JsonResult updatedetails(int wid,int client, int state, int portal, int type, int software, float weightage, int othertype_id,int role,int mls_id)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_updatedetails(wid, client, state, portal, type, software, weightage, othertype_id, role, mls_id);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        //Delete
        public JsonResult deletedetails(int wid)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_deletedetails(wid);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }
        //Manager Login
        //MonthlyReport Page
        //Get order details of individual employee in date range
        public JsonResult getemployeedetails(Int64 emp_name, DateTime f_date, DateTime l_date)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getemployeedetails(emp_name, f_date, l_date);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult getemployeedetails_shift_manager(Int64? emp_name, DateTime f_date, DateTime l_date)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getemployeedetails_shift_manager(emp_name, f_date, l_date);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult getorderdetails_shift_manager(Int64? emp_name, DateTime f_date, DateTime l_date)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getorderdetails_shift_manager(emp_name, f_date, l_date);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        //Employee Login
        //MyOrder Page
        //get incentive and order details of individual employee in date range
        public JsonResult getemployeeincentivedetails(DateTime f_date, DateTime l_date,int id)
        {
            Int64 refNo = Convert.ToInt64(Session["refNo"]);

            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getemployeeincentivedetails(f_date, l_date, id, refNo);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }

        }

        //Manager login
        //MonthlyReport Module
        //Get order details of all employee in date range
        public JsonResult getorderdetails(DateTime f_date, DateTime l_date)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getorderdetails(f_date, l_date);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }


        }

        //
        public JsonResult getHistoryDetails(Int64 OrderId)
        {
            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getHistoryDetails(OrderId);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }
        }

        //employee Login
        //MyIncentive Module
        //Get history of particular order
        public JsonResult getEmployeeHistoryDetails(Int64 OrderId)
        {

            Int64 refNo = Convert.ToInt64(Session["refNo"]);

            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getEmployeeHistoryDetails(OrderId, refNo);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }
        }

        //Employee login
        //Method for retriving completed , pending etc. order count for individual employee login
        public JsonResult getDashboardValues(DateTime StartDate,DateTime EndDate)
        {
            Int64 refNo = Convert.ToInt64(Session["refNo"]);

            
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getDashboardValues(StartDate, EndDate, refNo);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }
        }

        //Manager login
        //Method for retriving the count of total,completed,etc. order in menu bar
        public JsonResult getManagerDashboardValues(DateTime StartDate, DateTime EndDate)
        {

            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_getManagerDashboardValues(StartDate, EndDate);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }
        }

        //Employee login
        //Method for retriving the incentive for orders for individual employee based on date range
         public JsonResult employeeIncentivedetails(DateTime StartDate, DateTime? EndDate)
        {
            Int64 refNo = Convert.ToInt64(Session["refNo"]);

            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_employeeIncentivedetails(StartDate, EndDate, refNo);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

            }
            
        }

         public JsonResult manager_employeeIncentivedetails(DateTime StartDate, DateTime EndDate,Int64 refNo)
         {
             if (Session["refNo"] != null)
             {
                 string jsonResult = SqlProvider.load_employeeIncentiveandStatusdetails(StartDate, EndDate, refNo);

                 return Json(jsonResult, JsonRequestBehavior.AllowGet);
             }
             else
             {
                 ViewBag.Message = "Session Time Out";

                 return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
             }
             
         }


        //Manager login
        //Method for retriving employee designation inorder to allocate order limit for various employees
          public JsonResult LoadDesignationData(int role)
          {
              if (Session["refNo"] != null)
              {
                  object jsonResult = SqlProvider.LoadDesignationData(role);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }
              

          }

        //Manager login
        //Method to insert order limit details (Through modal)
          public JsonResult insertorderlimitdetails(int minOrder, int firstInc, int secondInc, float firstAmount, float secondAmount, float balanceAmount, int designation, int role)
        {
            if (Session["refNo"] != null)
            {
                string jsonResult = SqlProvider.load_insertorderlimitdetails(minOrder, firstInc, secondInc, firstAmount, secondAmount, balanceAmount, designation, role);

                return Json(jsonResult, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ViewBag.Message = "Session Time Out";

                return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
            }
            

        }


        //Manager Login
        //Method to retrive all details of order limit and incentive details into data table
          public JsonResult getIncentiveOrderLimitDetails()
          {
              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getIncentiveOrderLimitDetails();

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }

          }


        //Manager login
        //Method to get the order limit details into edit modal
          public JsonResult getOrderLimitdetails(int olId)
          {
              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getOrderLimitdetails(olId);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }
              

          }


        //Manager Login
        //Method to update the order limit and incentive allocation details
          public JsonResult updateOrderLimitDetails(int minOrder, int firstInc, int secondInc, float firstAmount, float secondAmount, float balanceAmount, int designation, int role, int olId)
          {
              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_updateOrderLimitDetails(minOrder, firstInc, secondInc, firstAmount, secondAmount, balanceAmount, designation, role, olId);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }
              

          }


        //Manager login
        //Method to get the order limit and incentive on the same
          public JsonResult getincentiveamount(DateTime f_date, DateTime l_date)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getincentiveamount(f_date, l_date);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }

          public JsonResult getEmployeePerformance(DateTime StartDate, DateTime EndDate)
          {

              if (Session["refNo"] != null)
              {
                  object jsonResult = SqlProvider.load_getEmployeePerformance(StartDate, EndDate);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }

          }

          public JsonResult getincentiveByMonth(DateTime? year)
          {

              Int64 refNo = Convert.ToInt64(Session["refNo"]);

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getincentiveByMonth( year, refNo);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }

          public JsonResult DASHBOARDBAR(DateTime StartDate,DateTime EndDate)
          {

              if (Session["refNo"] != null)
              {
                  object jsonResult = SqlProvider.load_dashboard(StartDate, EndDate);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";
                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }

              

          }

          public JsonResult getVerifyList(int month,int year)
          {
              //Int64 refNo = Convert.ToInt64(Session["refNo"]);
              if(Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getVerifyList(month, year);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";
                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }

          }

          public JsonResult incentiveApproval(Int64 empId, Int64 totalOrder,float incentiveOrder,float incentiveAmount,int month,int year)
          {
              Int64 refNo = Convert.ToInt64(Session["refNo"]);

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_incentiveApproval(empId, totalOrder, incentiveOrder, incentiveAmount, month, year, refNo);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }

              

          }

          public JsonResult getStaticWeightageDetails(Int64? Id)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getStaticWeightageDetails(Id);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }
              

          }

          public JsonResult updateStaticWeightage(string reason,float point,Int64 Id)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_updateStaticWeightage(reason, point, Id);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }
              

          }

          public JsonResult getMlsData(Int64? Id, string mlsName, Int64? mlsId)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.getMlsData(Id, mlsName, mlsId);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }

          public JsonResult getPlatformData(Int64? Id, string platformName, Int64? platformId)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.getPlatformData(Id, platformName, platformId);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }

          public JsonResult getMlsPlatformData(Int64? Id, Int64? mlsId, Int64? platformId, Int64? mapId)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.getMlsPlatformData(Id, mlsId, platformId, mapId);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }

          public JsonResult getZipMlsData(Int64? Id, Int64? mlsId, Int64? platformId, Int64? mapId, Int64? zipcode)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.getZipMlsData(Id, mlsId, platformId, mapId, zipcode);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }

          public JsonResult getZipData(Int64? Id, Int64? mlsId, Int64? platformId, Int64? mapId, Int64? zipcode)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.getZipData(Id, mlsId, platformId, mapId, zipcode);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }

          public JsonResult getAddressDetails(string address)
          {
              Int64 refNo = Convert.ToInt64(Session["refNo"]);


              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getAddressDetails(address, refNo);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

              }

          }

          public JsonResult getDayWiseOrders(Int64? empId,string currentMonth,string currentYear)
          {
              Int64 refNo = Convert.ToInt64(Session["refNo"]);


              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getDayWiseOrders(empId,currentMonth,currentYear);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

              }

          }

          public JsonResult getBatchWiseDetails(int? RefId,int? Id,int? EmpId ,int? BatchId)
          {
              Int64 refNo = Convert.ToInt64(Session["refNo"]);


              if (Session["refNo"] != null)
              {
                  object Result = SqlProvider.load_BatchWiseList(RefId, Id, EmpId, BatchId);

                  return Json(Result, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);

              }

          }

          public JsonResult getBatchData(Int64? Id, Int64? batchId,string batchName, float? restarget, float? entryTarget)
          {

              if (Session["refNo"] != null)
              {
                  string jsonResult = SqlProvider.load_getBatchData(Id, batchId, batchName, restarget, entryTarget);

                  return Json(jsonResult, JsonRequestBehavior.AllowGet);
              }
              else
              {
                  ViewBag.Message = "Session Time Out";

                  return Json(ViewBag.Message, JsonRequestBehavior.AllowGet);
              }


          }



    }
    


    
}