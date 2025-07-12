using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MK.DynamicReport.Application.Interfaces;

namespace MK.DynamicReport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] 
    public class ReportTestController : ControllerBase
    {
  
    }
}
