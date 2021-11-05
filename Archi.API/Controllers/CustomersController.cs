using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Archi.API.Data;
using Archi.API.Models;
using Archi.Librari.Controllers;

namespace Archi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : BaseController<ArchiDbContext, Customer>
    {

        public CustomersController(ArchiDbContext context): base(context)
        {
        }
    }
}
