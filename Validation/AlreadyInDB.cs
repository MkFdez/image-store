using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DataAccess;
namespace Validation
{
    public class AlreadyInDB : ValidationAttribute
    {
        private int choise;
        private bool negativeResult;
        string UserorEmail;
        bool edit;
        public AlreadyInDB(int emailOrUser = 0, bool not = false, bool _edit = false)
        {
            choise = emailOrUser;
            negativeResult = not;
            UserorEmail = choise == 0 ? "User" : "Email";
            edit = _edit;
        }
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            using (var container = new Project1DBEntities())
            {
                
                bool inDB;
                if (edit)
                {
                    int userid = int.Parse(HttpContext.Current.Session["userid"].ToString());
                    inDB = choise == 0 ? container.Users.Any(x => x.UserName == value.ToString() && x.Id != userid) : container.Users.Any(x => x.Email == value.ToString() && x.Id != userid);
                }
                else
                {
                    inDB = choise == 0 ? container.Users.Any(x => x.UserName == value.ToString()) : container.Users.Any(x => x.Email == value.ToString());
                }
                if (negativeResult)
                {
                    return inDB == false ? ValidationResult.Success : new ValidationResult($"The {UserorEmail} already Exist");
                }
                else
                {
                    return inDB ? ValidationResult.Success : new ValidationResult($"The {UserorEmail} doesn't exist");
                }
            }

        }

    }
}
