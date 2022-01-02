﻿using System.Web.Mvc;

namespace RTech.Demo.Areas.PayRoll
{
    public class PayRollAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PayRoll";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PayRoll_default",
                "PayRoll/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}