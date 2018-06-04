using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Api.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Api.Controllers
{
    public class StudController : ApiController
    {
        private readonly IStudentMapper _mapper;
        public StudController(IStudentMapper mapper)
        {
            _mapper = mapper;
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.ActionName("GetName")]
        public HttpResponseMessage GetName()
        {
            HttpResponseMessage httpResponseMessage = null;
            string result=string.Format("Welcome Students !!!");
            httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, result);
            return httpResponseMessage;
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.ActionName("GetRetrieveStudents")]
        public HttpResponseMessage GetRetrieveStudents()
        {

            Api.Models.Student studentobj = new Api.Models.Student();
            studentobj.StudentId = 1;
            var c = studentobj;

            HttpResponseMessage httpResponseMessage = null;
            try
            {
                var result = _mapper.RetrieveStudents(c);

                httpResponseMessage = Request.CreateResponse(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                httpResponseMessage = Request.CreateResponse(HttpStatusCode.InternalServerError, new Api.Models.ErrorInfo() { ErrorMessage = ex.Message, ErrorStackTrace = ex.ToString() });
            }
            return httpResponseMessage;
        }

    }
}

