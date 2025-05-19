using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    
    
}

