using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using ApiModels = Api.Models;
using Contracts;
using System.Web.Http.Results;

namespace Api.Controllers
{
    public class TestController : ApiController
    {
        private readonly IEmployeeMapper _mapper;

        public TestController(IEmployeeMapper mapper)
        {
            _mapper = mapper;
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.ActionName("GetName")]
        public IHttpActionResult GetName()
        {
            return Ok(string.Format("Welcome Raja!!!"));
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("GetAge")]
        public HttpResponseMessage GetAge()
        {
            var httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, "Age is XYZ");

            return httpResponseMessage;
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("SaveEmployee")]
        public HttpResponseMessage GetSaveEmployee()
        {
            ApiModels.Employee employee = new ApiModels.Employee();
            employee.Name = "XYZ";
            employee.Age = 25;

            HttpResponseMessage httpResponseMessage = null;
            try
            {
                var result = _mapper.SaveEmployee(employee);

                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, employee);
            }
            catch (Exception ex)
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new ApiModels.ErrorInfo() { ErrorMessage = ex.Message, ErrorStackTrace = ex.ToString() });
            }
            return httpResponseMessage;
        }

    }

}
