using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ConsultaMD.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static ConsultaMD.Data.EspecialidadesData;

namespace ConsultaMD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ListController : ControllerBase
    {
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(JsonResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //val : rut | razon_social | actividades | all
        public IActionResult GetSpecialtyList()
        {
            return Ok(Enum<Especialidades>.ToSelect);
        }
    }
}