using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task11.Exceptions;
using Task11.Services;

namespace Task11.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PatientsController : ControllerBase
{
    private  readonly IDbService _dbService;

    public PatientsController(IDbService db)
    {
        this._dbService = db;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientInfo([FromRoute] int id)
    {
        
        try
        {
            var patientInfo = await _dbService.GetPatientFullInfo(id);
            
            return Ok(patientInfo);
        }
        catch (NoPatientFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

