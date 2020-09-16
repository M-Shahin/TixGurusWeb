using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace TixGurusWeb.Controllers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MultipleButtonAttribute : ActionNameSelectorAttribute
    {
        public string ActionName { get; set; }
        public override bool IsValidName(ControllerContext controllerContext, string actionName, MethodInfo methodInfo)
        {
            var isValidName = false;
            var value = controllerContext.Controller.ValueProvider.GetValue(this.ActionName);

            if (value != null)
            {
                controllerContext.Controller.ControllerContext.RouteData.Values[actionName] = this.ActionName;
                isValidName = true;
            }

            return isValidName;
        }

    }
}