using Client.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClientController : ControllerBase
{
    //[HttpPost]
    //[ProducesResponseType(StatusCodes.Status200OK)]
    //public ActionResult<String> Create([FromBody] ClientCreateDTO createDTO)
    //{
    //    // Ajouter un client
    //    return Ok("Je suis client Said wahid");


    //}

    [HttpPost]
    public ActionResult<List<ClientDTO>> Get(
    [FromForm] ClientCreateDTO data)
    {
        List<ClientDTO> list = null;



        return list;
    }

//    [HttpPost]
//    public ActionResult<List<ClientDTO>> CreateFromBody(
//[FromBody] ClientCreateDTO data)
//    {
//        List<ClientDTO> list = null;


//        return list;
//    }

//    [HttpPost]
//    public ActionResult<List<ClientDTO>> CreateFromQuery(
//    [FromQuery] ClientCreateDTO data )
//    {
//        List<ClientDTO> list = null;

//        return list;
//    }

}
