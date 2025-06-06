using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task11.DTOs;
using Task11.Exceptions;
using Task11.Models;
using Task11.Services;

namespace Task11.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PrescriptionsController : ControllerBase
{
    private  readonly IDbService _dbService;

    public PrescriptionsController(IDbService db)
    {
        this._dbService = db;
    }

    [HttpPost]
    public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDto prescription)
    {
        try
        {
            await _dbService.AddPrescription(prescription);
        }
        catch (MedicamentNotExistsException e)
        {
            return NotFound(e.Message);
        }
        catch (MedicamentsOverflowException e)
        {
            return BadRequest(e.Message);
        }
        catch (DueDateBeforeDateException e)
        {
            return BadRequest(e.Message);
        } catch (PrescriptionDuplicateException e)
        {
            return BadRequest(e.Message);
        }
        
        return Ok("Prescription added");
    }
}

