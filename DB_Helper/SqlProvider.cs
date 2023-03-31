using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Insights.Models;
using System.Web.Script.Serialization;
//using System.Web.Helpers.Json;
//using System.Web.HttpContext;

namespace Insights.DB_Helper
{
    public class SqlProvider 
    {
        static string connectionstring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //private static string Session;
        //private static string Session;


        private static bool FormatJsonOutput { get; set; }
        public static string Serialize(object value)
        {
            Type type = value.GetType();

            Newtonsoft.Json.JsonSerializer json = new Newtonsoft.Json.JsonSerializer();


            json.NullValueHandling = NullValueHandling.Ignore;

            json.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
            json.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore;
            json.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            if (type == typeof(DataTable))
                json.Converters.Add(new DataTableConverter());
            else if (type == typeof(DataSet))
                json.Converters.Add(new DataSetConverter());

            StringWriter sw = new StringWriter();
            Newtonsoft.Json.JsonTextWriter writer = new JsonTextWriter(sw);
            if (FormatJsonOutput)
                writer.Formatting = Newtonsoft.Json.Formatting.Indented;
            else
                writer.Formatting = Newtonsoft.Json.Formatting.None;

            writer.QuoteChar = '"';
            json.Serialize(writer, value);

            string output = sw.ToString();

            writer.Close();
            sw.Close();

            return output;
        }

        //Login Page
        //Checking the existance of user
        public static Int64 CheckLogin(LoginViewModel model)
        {
            Int64 iResult = 0;
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("PROC_CHECK_LOGIN");
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@password", model.Password);
                    iResult = Convert.ToInt64(cmd.ExecuteScalar());
                    con.Close();
                }
                catch (Exception ex)
                {
                    var AjaxReturn = new { Message = ex.ToString() };
                    //TempData["exError"] = ex.ToString();
                }
                finally
                {
                    if (con != null)
                    {
                        if (con.State == System.Data.ConnectionState.Open)
                            con.Close();
                    }
                }
            }
            return iResult;
        }
        public static object LoadTableData(Int64 refNo)
        {
            var AjaxReturn = "Error";
            try
            {
                string strClientList = "";
                string strPortalList = "";
                string strOrderTypeList = "";
                string strStateList = "";
                string strEmployeeList = "";
                string strEmployeeRoleList = "";
                string strOtherTypesList = "";
                string strMlsNames = "";
                string strPlatform = "";
                string strMlsPlatformMap = "";
                string strTrainees = "";
                string strBatch = "";
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_GET_TABLE_VALUES");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@userId", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();
                var abc = new
                {

                    strClientList = Serialize(data.Tables[0]),
                    strPortalList = Serialize(data.Tables[1]),
                    strOrderTypeList = Serialize(data.Tables[2]),
                    strStateList = Serialize(data.Tables[3]),
                    strEmployeeList = Serialize(data.Tables[4]),
                    strEmployeeRoleList = Serialize(data.Tables[6]),
                    strOtherTypesList = Serialize(data.Tables[9]),
                    strMlsNames = Serialize(data.Tables[8]),
                    strPlatform = Serialize(data.Tables[9]),
                    strMlsPlatformMap = Serialize(data.Tables[10]),
                    strTrainees = Serialize(data.Tables[11]),
                    strBatch = Serialize(data.Tables[12]),

                    //strStatusReasonList = Serialize(data.Tables[4]),
                    //strAutoBPOList = Serialize(data.Tables[5]),

                };
                var AjaxData = abc;

                return AjaxData;
            }
            catch
            {
                return AjaxReturn;
            }

        }

        public static Users GetUserDetailsByUserID(Int64 iUserID)
        {
            Users user = new Users();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("PROC_INCENTIVE_GET_USER_DETAILS_BY_REFNO");
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    cmd.Connection = con;
                    cmd.Parameters.AddWithValue("@refNo", iUserID);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        user.emp_ref_no = Convert.ToInt64(reader["ref_no"]);

                        user.role_id = reader["role_id"] == DBNull.Value ? 0 : Convert.ToInt32(reader["role_id"]);

                        user.emp_full_name = reader["emp_full_name"] == DBNull.Value ? "" : reader["emp_full_name"].ToString();

                        user.emp_ref_code = reader["emp_ref_code"] == DBNull.Value ? "" : reader["emp_ref_code"].ToString();

                    }
                    reader.Close();
                    con.Close();
                }
                catch (Exception ex)
                {
                    //return ex.ToString();
                }
                finally
                {
                    if (con != null)
                    {
                        if (con.State == System.Data.ConnectionState.Open)
                            con.Close();
                    }
                }
            }

            //user.Password = CommonFunctions.Decrypt_Password(user.Password);

            return user;
        }

        //Manager Login
        //To ADD ,EDIT,RETRIVE,DELETE weightage

        //Insert
        public static string load_insertalldetails(int client, int state, int portal, int type, int software, float weightage, int role, int otherTypes, int mls_id)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            
            try
            {

                SqlConnection sqlCon = new SqlConnection(connectionstring);
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_ADDWEIGHTAGE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                myCommand.Parameters.AddWithValue("@ClientId", client);
                myCommand.Parameters.AddWithValue("@OrderTypeId", type);
                myCommand.Parameters.AddWithValue("@PortalId", portal);
                myCommand.Parameters.AddWithValue("@StateId", state);
                myCommand.Parameters.AddWithValue("@Software", software);
                myCommand.Parameters.AddWithValue("@Weightage", weightage);
                myCommand.Parameters.AddWithValue("@role_id", role);
                myCommand.Parameters.AddWithValue("@othertype_id", otherTypes);
                myCommand.Parameters.AddWithValue("@mls_id", mls_id);
                myCommand.Parameters.AddWithValue("@refNo", refNo);
                var AjaxData = Convert.ToString(myCommand.ExecuteScalar());

                sqlCon.Close();

                

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/insertalldetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        //Retrive
        public static string load_getalldetails(int role,Int64 refNo)
        {
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_ORDERWEIGHTAGE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@refNo", refNo);
                myCommand.Parameters.AddWithValue("@type", role);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getalldetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        // Edit Retrival
        public static string load_getweightagedetails(int wid)
        {
            var AjaxReturn = "Error";
            try
            {
                Int64 id = Convert.ToInt64(wid);

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_EDITWEIGHTAGE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@wid", id);
                //var AjaxData = Convert.ToString(myCommand.ExecuteScalar());
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch
            {
                return AjaxReturn;
            }

        }

        //Update 
        public static string load_updatedetails(int wid, int client, int state, int portal, int type, int software, float weightage, int othertype_id,int role,int mls_id)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_UPDATEWEIGHTAGE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@wid", wid);
                myCommand.Parameters.AddWithValue("@ClientId", client);
                myCommand.Parameters.AddWithValue("@OrderTypeId", type);
                myCommand.Parameters.AddWithValue("@PortalId", portal);
                myCommand.Parameters.AddWithValue("@StateId", state);
                myCommand.Parameters.AddWithValue("@Software", software);
                myCommand.Parameters.AddWithValue("@Weightage", weightage);
                myCommand.Parameters.AddWithValue("@othertype_id", othertype_id);
                myCommand.Parameters.AddWithValue("@roleId", role);
                myCommand.Parameters.AddWithValue("@mls_id", mls_id);
                myCommand.Parameters.AddWithValue("@refNo", refNo);
                var AjaxData = Convert.ToString(myCommand.ExecuteScalar());
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                //var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/updatedetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        //Delete
        public static string load_deletedetails(int wid)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                //DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                //SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_DELETEWEIGHTAGE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@wid", wid);
                myCommand.Parameters.AddWithValue("@refNo", refNo);
                var AjaxData = Convert.ToString(myCommand.ExecuteScalar());
                //SqlDataAdapter da = new SqlDataAdapter(myCommand);

                //da.Fill(data);
                //da.Dispose();
                sqlCon.Close();

                //var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {

                trackException("Home/deletedetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        //Manager Login
        //MonthlyReport Page
        //Get order details of individual employee in date range
        public static string load_getemployeedetails(Int64 emp_name, DateTime f_date, DateTime l_date)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_INCENTIVEORDERS");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@emp_name", emp_name);
                myCommand.Parameters.AddWithValue("@from_date", f_date);
                myCommand.Parameters.AddWithValue("@to_date", l_date);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getemployeedetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }
        public static string load_getemployeedetails_shift_manager(Int64? emp_name, DateTime f_date, DateTime l_date)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_EMPLOYEEORDERS_SHIFTMANAGER");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@emp_name", emp_name);
                myCommand.Parameters.AddWithValue("@from_date", f_date);
                myCommand.Parameters.AddWithValue("@to_date", l_date);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getemployeedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        //Employee Login
        //MyOrder Page
        //get incentive and order details of individual employee in date range
        public static string load_getemployeeincentivedetails(DateTime f_date, DateTime l_date, int id, Int64 refNo)
        {
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_EMPLOYEEORDERS");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@emp_no", refNo);
                myCommand.Parameters.AddWithValue("@from_date", f_date);
                myCommand.Parameters.AddWithValue("@to_date", l_date);
                myCommand.Parameters.AddWithValue("@id", id);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getemployeeincentivedetails", ex.ToString(),refNo);

               // AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }


        //Manager login
        //MonthlyReport Module
        //Get order details of all employee in date range
        public static string load_getorderdetails(DateTime f_date, DateTime l_date)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETORDERBYDATE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@emp_no", emp_ref_no);
                myCommand.Parameters.AddWithValue("@from_date", f_date);
                myCommand.Parameters.AddWithValue("@to_date", l_date);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getorderdetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string load_getorderdetails_shift_manager(Int64? emp_name, DateTime f_date, DateTime l_date)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETORDERBYDATE_SHIFTMANAGER");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@emp_no", emp_name);
                myCommand.Parameters.AddWithValue("@from_date", f_date);
                myCommand.Parameters.AddWithValue("@to_date", l_date);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getorderdetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        //
        public static string load_getHistoryDetails(Int64 OrderId)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_ORDERHISTORY");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@emp_no", emp_ref_no);
                myCommand.Parameters.AddWithValue("@orderId", OrderId);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getHistoryDetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }
        }

        //employee Login
        //MyIncentive Module
        //Get history of particular order
        public static string load_getEmployeeHistoryDetails(Int64 OrderId,Int64 refNo)
        {
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_EMPLOYEEORDERHISTORY");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@emp_no", emp_ref_no);
                myCommand.Parameters.AddWithValue("@orderId", OrderId);
                myCommand.Parameters.AddWithValue("@empId", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getEmployeeHistoryDetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }
        }

        private static JsonResult Json(object AjaxReturn, JsonRequestBehavior jsonRequestBehavior)
        {
            throw new NotImplementedException();
        }


        //Employee login
        //Method for retriving completed , pending etc. order count for individual employee login
        public static string load_getDashboardValues(DateTime StartDate, DateTime EndDate, Int64 refNo)
        {
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_DASHBOARD_MENUBAR");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@emp_no", emp_ref_no);
                myCommand.Parameters.AddWithValue("@empId", refNo);
                myCommand.Parameters.AddWithValue("@from_date", StartDate);
                myCommand.Parameters.AddWithValue("@to_date", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getDashboardValues", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                //return new JavaScriptSerializer().Serialize(new { Message = ex.ToString() });

                return AjaxReturn;
            }
        }

        //Manager login
        //Method for retriving the count of total,completed,etc. order in menu bar
        public static string load_getManagerDashboardValues(DateTime StartDate, DateTime EndDate)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                //Int64 refNo = Convert.ToInt64(Session["refNo"]);

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_MANAGERDASHBOARD_MENUBAR");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@emp_no", emp_ref_no);
                myCommand.Parameters.AddWithValue("@from_date", StartDate);
                myCommand.Parameters.AddWithValue("@to_date", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getManagerDashboardValues", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }
        }

        //Employee login
        //Method for retriving the incentive for orders for individual employee based on date range
        public static string load_employeeIncentivedetails(DateTime StartDate, DateTime? EndDate,Int64 refNo)
        {
           // Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_MYINCENTIVE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@emp_no", refNo);
                myCommand.Parameters.AddWithValue("@from_date", StartDate);
                myCommand.Parameters.AddWithValue("@to_date", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(),refNo);

                //AjaxReturn = ex.Message;

                //TempData["exError"] = ex.ToString();

                return AjaxReturn;
            }
        }

        public static string load_employeeIncentiveandStatusdetails(DateTime StartDate, DateTime? EndDate, Int64 refNo)
        {
            // Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_STATUSANDINCENTIVE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@emp_no", refNo);
                myCommand.Parameters.AddWithValue("@from_date", StartDate);
                myCommand.Parameters.AddWithValue("@to_date", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                //TempData["exError"] = ex.ToString();

                return AjaxReturn;
            }
        }



//////////////////////////////////////
        //Manager login
        //Method for retriving employee designation inorder to allocate order limit for various employees
        public static object LoadDesignationData(int role)
        {
            var AjaxReturn = "Error";
            try
            {

                string strDesignationList = "";
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_GET_DESIGNATION_VALUES");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@roleId", role);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();
                var AjaxData = new
                {
                    strDesignationList = Serialize(data.Tables[0]),
                };

                return AjaxData;
            }
            catch
            {
                return AjaxReturn;
            }

        }

        //Manager login
        //Method to insert order limit details (Through modal)
        public static string load_insertorderlimitdetails(int minOrder, int firstInc, int secondInc, float firstAmount, float secondAmount, float balanceAmount, int designation, int role)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_ORDERLIMIT");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@MinOrder", minOrder);
                myCommand.Parameters.AddWithValue("@FirstInc", firstInc);
                myCommand.Parameters.AddWithValue("@SecInc", secondInc);
                myCommand.Parameters.AddWithValue("@FirstAmount", firstAmount);
                myCommand.Parameters.AddWithValue("@SecAmount", secondAmount);
                myCommand.Parameters.AddWithValue("@BalanceAmount", balanceAmount);
                myCommand.Parameters.AddWithValue("@Designation", designation);
                myCommand.Parameters.AddWithValue("@roleId", role);
                myCommand.Parameters.AddWithValue("@refNo", refNo);
                var AjaxData = Convert.ToString(myCommand.ExecuteScalar());

                sqlCon.Close();

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                return AjaxReturn;
            }

        }


        //Manager Login
        //Method to retrive all details of order limit and incentive details into data table
        public static string load_getIncentiveOrderLimitDetails()
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETORDERLIMIT");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        //Manager login
        //Method to get the order limit details into edit modal
        public static string load_getOrderLimitdetails(int olId)
        {
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_EDITORDERLIMIT");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@olId", olId);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch
            {
                return AjaxReturn;
            }

        }


        //Manager Login
        //Method to update the order limit and incentive allocation details
        public static string load_updateOrderLimitDetails(int minOrder, int firstInc, int secondInc, float firstAmount, float secondAmount, float balanceAmount, int designation, int role, int olId)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);

            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_UPDATEORDERLIMIT");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@MinOrder", minOrder);
                myCommand.Parameters.AddWithValue("@FirstInc", firstInc);
                myCommand.Parameters.AddWithValue("@SecInc", secondInc);
                myCommand.Parameters.AddWithValue("@FirstAmount", firstAmount);
                myCommand.Parameters.AddWithValue("@SecAmount", secondAmount);
                myCommand.Parameters.AddWithValue("@BalanceAmount", balanceAmount);
                myCommand.Parameters.AddWithValue("@Designation", designation);
                myCommand.Parameters.AddWithValue("@roleId", role);
                myCommand.Parameters.AddWithValue("@olId", olId);
                myCommand.Parameters.AddWithValue("@refNo", refNo);
                var AjaxData = Convert.ToString(myCommand.ExecuteScalar());
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                //var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

               // AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        //Manager login
        //Method to get the order limit and incentive on the same
        public static string load_getincentiveamount(DateTime f_date, DateTime l_date)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_CALCULATION");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@from_date", f_date);
                myCommand.Parameters.AddWithValue("@to_date", l_date);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getincentiveamount", ex.ToString(),refNo);

                //AjaxReturn = new { Message = ex.ToString() };

                //TempData["exError"] = ex.ToString();

                return AjaxReturn;
            }

        }

        //All Modules
        //Method for tracking exception occured while functioning
        public static string trackException(string controller, string exception, Int64 refNo)
        {
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_EXCEPTION_TRACKING");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                myCommand.Parameters.AddWithValue("@controllerAction", controller);
                myCommand.Parameters.AddWithValue("@exception", exception);
                myCommand.Parameters.AddWithValue("@ref_no", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return "Success";
            }
            catch (Exception ex)
            {
                //TempData["exError"] = ex.ToString();

                //AjaxReturn = new { Message = ex.ToString() };

                return "Failed";
            }
        }



        public static string load_getincentiveByMonth(DateTime? year,Int64 refNo)
        {
            //Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_BY_MONTH");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@emp_no", refNo);
                myCommand.Parameters.AddWithValue("@year", year);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getincentiveByMonth", ex.ToString(), refNo);

                //AjaxReturn = new { Message = ex.ToString() };

                //TempData["exError"] = ex.ToString();

                return AjaxReturn;
            }

        }


        public static object load_getEmployeePerformance(DateTime StartDate, DateTime EndDate)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                string table1 = "";
                string table2 = "";
                //Int64 refNo = Convert.ToInt64(Session["refNo"]);

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_EMPLOYEE_PERFORMANCE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                //myCommand.Parameters.AddWithValue("@emp_no", emp_ref_no);
                myCommand.Parameters.AddWithValue("@from_date", StartDate);
                myCommand.Parameters.AddWithValue("@to_date", EndDate);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var abc = new
                {

                    table1 = Serialize(data.Tables[0]),
                    table2 = Serialize(data.Tables[1]),


                };
                var AjaxData = abc;

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getManagerDashboardValues", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }
        }

        public static object load_dashboard(DateTime? StartDate, DateTime? EndDate)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                //Int64 id = Convert.ToInt64(wid);
                string table1 = "";
                string table2 = "";
                string table3 = "";
                string table4 = "";

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_DASHBOARDBAR");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@start_date", StartDate);
                myCommand.Parameters.AddWithValue("@end_date", EndDate);
                //var AjaxData = Convert.ToString(myCommand.ExecuteScalar());
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var datas = new
                {

                    table1 = Serialize(data.Tables[0]),
                    table2 = Serialize(data.Tables[1]),
                    table3 = Serialize(data.Tables[2]),
                    table4 = Serialize(data.Tables[3]),


                };
                var AjaxData = datas;

                //var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch(Exception ex)
            {
                trackException("Home/getManagerDashboardValues", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string load_getVerifyList(int month, int year)
        {
            var AjaxReturn = "Error";
            try
            {

                

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_VERIFY_LIST");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@month", month);
                myCommand.Parameters.AddWithValue("@year", year);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {

                Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);

                trackException("Home/getalldetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string load_incentiveApproval(Int64 empId, Int64 totalOrder, float incentiveOrder, float incentiveAmount, int month, int year,Int64 refNo)
        {
            var AjaxReturn = "Error";
            try
            {

                

                //DataSet data = new DataSet();
                //SqlConnection sqlCon = new SqlConnection(connectionstring);
                //SqlCommand sqlCmd = new SqlCommand();

                //SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_APPROVAL");
                //myCommand.CommandType = CommandType.StoredProcedure;
                //myCommand.Connection = sqlCon;


                SqlConnection sqlCon = new SqlConnection(connectionstring);
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_APPROVAL");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@empId", empId);
                myCommand.Parameters.AddWithValue("@totalOrder", totalOrder);
                myCommand.Parameters.AddWithValue("@incentiveOrder", incentiveOrder);
                myCommand.Parameters.AddWithValue("@incentiveAmount", incentiveAmount);
                myCommand.Parameters.AddWithValue("@month", month);
                myCommand.Parameters.AddWithValue("@year", year);
                myCommand.Parameters.AddWithValue("@refNo", refNo);
                string AjaxData = Convert.ToString(myCommand.ExecuteScalar());

                sqlCon.Close();



                return AjaxData;
            }
            catch (Exception ex)
            {

                trackException("Home/getalldetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string load_getStaticWeightageDetails(Int64? Id)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETSTATICWEIGHTAGE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@Id", Id);

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string load_updateStaticWeightage(string reason,float point,Int64 Id)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_UPDATE_STATICWEIGHTAGE");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@reason", reason);
                myCommand.Parameters.AddWithValue("@point", point);
                myCommand.Parameters.AddWithValue("@Id", Id);
                myCommand.Parameters.AddWithValue("@refNo", refNo);

                var AjaxData = Convert.ToString(myCommand.ExecuteScalar());
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                //var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/updateStaticWeightage", ex.ToString(), refNo);

                // AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string getMlsData(Int64? Id, string mlsName, Int64? mlsId)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETMLSDATA");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@Id", Id);
                myCommand.Parameters.AddWithValue("@mlsId", mlsId);
                myCommand.Parameters.AddWithValue("@mlsName", mlsName);
                myCommand.Parameters.AddWithValue("@empId", refNo);

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string getPlatformData(Int64? Id, string platformName, Int64? platformId)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETPLATFORMDATA");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@Id", Id);
                myCommand.Parameters.AddWithValue("@platformId", platformId);
                myCommand.Parameters.AddWithValue("@platformName", platformName);
                myCommand.Parameters.AddWithValue("@empId", refNo);

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string getMlsPlatformData(Int64? Id, Int64? mlsId, Int64? platformId, Int64? mapId)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETMLSPLATFORMDATA");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@Id", Id);
                myCommand.Parameters.AddWithValue("@mlsId", mlsId);
                myCommand.Parameters.AddWithValue("@platformId", platformId);
                myCommand.Parameters.AddWithValue("@mapId", mapId);
                myCommand.Parameters.AddWithValue("@empId", refNo);

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string getZipMlsData(Int64? Id, Int64? mlsId, Int64? platformId, Int64? mapId, Int64? zipcode)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETZIPCODEDATA");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@Id", Id);
                myCommand.Parameters.AddWithValue("@mlsId", mlsId);
                myCommand.Parameters.AddWithValue("@platformId", platformId);
                myCommand.Parameters.AddWithValue("@mapId", mapId);
                myCommand.Parameters.AddWithValue("@zipcode", zipcode);
                myCommand.Parameters.AddWithValue("@empId", refNo);

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string getZipData(Int64? Id, Int64? mlsId, Int64? platformId, Int64? mapId, Int64? zipcode)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GETZIP");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@Id", Id);
                myCommand.Parameters.AddWithValue("@mlsId", mlsId);
                myCommand.Parameters.AddWithValue("@platformId", platformId);
                myCommand.Parameters.AddWithValue("@mapId", mapId);
                myCommand.Parameters.AddWithValue("@zipcode", zipcode);
                myCommand.Parameters.AddWithValue("@empId", refNo);

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string load_getAddressDetails(string address, Int64 refNo)
        {
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_ADDRESSHISTORY");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@refNo", refNo);
                myCommand.Parameters.AddWithValue("@address", address);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/getalldetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }

        public static string load_getDayWiseOrders(Int64? empId,string currentMonth,string currentYear)
        {
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_GET_DAYWISEORDERS");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@empId", empId);
                myCommand.Parameters.AddWithValue("@month", currentMonth);
                myCommand.Parameters.AddWithValue("@year", currentYear);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                //trackException("Home/getalldetails", ex.ToString(), empId);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }


        public static string load_BatchWiseList(int? RefId, int? Id, int? EmpId, int? BatchId)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {
                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();

                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_EMPLOYEE_BATCH_DETAILS");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;

                myCommand.Parameters.AddWithValue("@RefId", RefId);
                myCommand.Parameters.AddWithValue("@Id", Id);
                myCommand.Parameters.AddWithValue("@EmpId", EmpId);
                myCommand.Parameters.AddWithValue("@BatchId", BatchId);
                myCommand.Parameters.AddWithValue("@MarkedEmployee", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                //trackException("Home/getalldetails", ex.ToString(), empId);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }


        public static string load_getBatchData(Int64? Id, Int64? batchId,string batchName, float? restarget, float? entryTarget)
        {
            Int64 refNo = Convert.ToInt64(HttpContext.Current.Session["refNo"]);
            var AjaxReturn = "Error";
            try
            {

                DataSet data = new DataSet();
                SqlConnection sqlCon = new SqlConnection(connectionstring);
                SqlCommand sqlCmd = new SqlCommand();
                sqlCon.Open();
                SqlCommand myCommand = new SqlCommand("PROC_INCENTIVE_BATCH_DETAILS");
                myCommand.CommandType = CommandType.StoredProcedure;
                myCommand.Connection = sqlCon;
                myCommand.Parameters.AddWithValue("@RefId", Id);
                myCommand.Parameters.AddWithValue("@BatchId", batchId);
                myCommand.Parameters.AddWithValue("@BatchName", batchName);
                myCommand.Parameters.AddWithValue("@ResTarget", restarget);
                myCommand.Parameters.AddWithValue("@EntryTarget", entryTarget);
                myCommand.Parameters.AddWithValue("@EmpId", refNo);

                //myCommand.Parameters.AddWithValue("@refNo", refNo);
                SqlDataAdapter da = new SqlDataAdapter(myCommand);

                da.Fill(data);
                da.Dispose();
                sqlCon.Close();

                var AjaxData = Serialize(data.Tables[0]);

                return AjaxData;
            }
            catch (Exception ex)
            {
                trackException("Home/employeeIncentivedetails", ex.ToString(), refNo);

                //AjaxReturn = ex.Message;

                return AjaxReturn;
            }

        }
    }
}