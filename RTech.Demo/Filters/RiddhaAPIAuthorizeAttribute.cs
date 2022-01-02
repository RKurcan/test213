﻿using RTech.Demo.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http.Filters;

namespace RTech.Demo.Filters
{
    public class RiddhaAPIAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override bool AllowMultiple
        {
            get { return false; }
        }
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            SetCulture();
            //actionContext.ActionDescriptor.ControllerDescriptor.ControllerName
            //
            base.OnAuthorization(actionContext);
        }
        private void SetCulture()
        {
            string language = CultureHelper.GetImplementedCulture(RiddhaSession.Language);
            CultureInfo ci = CultureInfo.GetCultureInfo(language);
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;
        }
    }
}