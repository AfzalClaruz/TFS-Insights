using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace Insights.Models
{


    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [EmailAddress]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class Users
    {
        public string Username { get; set; }

        public string Password { get; set; }

        public string emp_first_name { get; set; }

        public string emp_last_name { get; set; }

        public string emp_full_name { get; set; }

        public string emp_ref_code { get; set; }

        public Int64 emp_ref_no { get; set; }

        public Int64 role_id { get; set; }
    }

    public class DateClassModel
    {
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Required(ErrorMessage = "Due Date/Time is Required!", AllowEmptyStrings = false)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date cannot be empty")]
        //validate:must be greater than StartDate
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; }

        public int currentStatus { get; set; }

        public int month { get; set; }

        public int year { get; set; }

        [Required(ErrorMessage = "Role cannot be empty")]
        public int role { get; set; }

        [Required(ErrorMessage = "Address cannot be empty")]
        public string address { get; set; }
    }

    //public class NotImplementedException : SystemException
    //{
    //    //// Public Constructors
    //    //public NotImplementedException();
    //    //public NotImplementedException(string message);
    //    //public NotImplementedException(string message,
    //    //     Exception inner);
    //    //// Protected Constructors
    //    //protected NotImplementedException(
    //    //     System.Runtime.Serialization.SerializationInfo info,
    //    //     System.Runtime.Serialization.StreamingContext context);
    //}

   
}  




