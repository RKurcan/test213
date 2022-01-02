﻿using RTech.Demo.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RTech.Demo.Areas.Office.Controllers
{
    [MenuFilter("1004")]
    public class BranchController : Controller
    {
        //
        // GET: /Office/Branch/
        public ActionResult Index()
        {
            return View();
        }
	}
}