using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using calhfa_webapi.Models;

namespace calhfa_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanCategoriesController : ControllerBase
    {
        private readonly cal_haf_dummyContext _context;

        public LoanCategoriesController(cal_haf_dummyContext context)
        {
            _context = context;
        }

        // GET: api/LoanCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanCategory>>> GetLoanCategories()
        {
            return await _context.LoanCategories.ToListAsync();
        }

        // GET: api/LoanCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanCategory>> GetLoanCategory(int id)
        {
            var loanCategory = await _context.LoanCategories.FindAsync(id);

            if (loanCategory == null)
            {
                return NotFound();
            }

            return loanCategory;
        }
    }
}
