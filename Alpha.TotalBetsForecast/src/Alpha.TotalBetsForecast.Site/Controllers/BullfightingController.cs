using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Alpha.Web;
using MediatR;
using System.Web.Mvc;
using Alpha.TotalBetsForecast.Site.Requests;

namespace Alpha.TotalBetsForecast.Site.Controllers
{
    public class BullfightingController : BaseController
    {
        private readonly IMediator mediator;

        public BullfightingController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public ActionResult Index() => View();

        public JsonResult GetMatches(BullfightingQuery query) => Json(mediator.Send(query), JsonRequestBehavior.AllowGet);
    }
}