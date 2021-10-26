using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using calhfa_webapi.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace calhfa_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanStatusController : ControllerBase
    {
        private readonly cal_haf_dummyContext _context;

        public LoanStatusController(cal_haf_dummyContext context)
        {
            _context = context;
        }

        // GET: api/LoanStatus
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanStatus>>> GetLoanStatuses()
        {
            return await _context.LoanStatuses.ToListAsync();
        }

        // GET: api/LoanStatus/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LoanStatus>> GetLoanStatus(int id)
        {
            var loanStatus = await _context.LoanStatuses.FindAsync(id);

            if (loanStatus == null)
            {
                return NotFound();
            }

            return loanStatus;


        }

        /// <summary>
        /// Counts the total number of loans which are being reviwed for either compliance or for purchase.
        /// In these categories, it also counts which loans are being reviewed after suspension.
        /// Pulls data from the LoanStatus table for counting
        /// </summary>
        /// <returns> a json formatted string which contains the counts for the review categories </returns>
        // GET: /api/LoanStatus/count
        [Route("count")]
        [HttpGet]
        public string GetLoanCount()
        {
            // codes ending with '10' are in review, codes ending in '22' are suspended & being reviewed after a resubmit
            int ComplianceReviewCount = GetReviewCount(410);
            int ComplianceSuspenseReviewCount = GetReviewCount(422);
            int PurchaseReviewCount = GetReviewCount(510);
            int PurchaseSuspenseReviewCount = GetReviewCount(522);

            string jsonData = String.Format("{{compliantReviewCount: '{0}', compliantSuspenseCount: '{1}', purchaseReviewCount: '{2}', purchaseSuspenseCount: '{3}'}}",
                ComplianceReviewCount, ComplianceSuspenseReviewCount, PurchaseReviewCount, PurchaseSuspenseReviewCount);

            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
        }

        /// <summary>
        /// takes a specific status code and counts how many loans are still at that specific stage of review
        /// </summary>
        /// <param name="statusCode">int</param>
        /// <returns>a count of the loans still at that particular stage</returns>
        private int GetReviewCount(int statusCode)
        {
            int count = 0;
            var ComplianceReviewList =  _context.LoanStatuses.Where(l => l.StatusCode == statusCode).ToListAsync();
            List<int> idList = new List<int>();
            foreach ( var loanId in ComplianceReviewList.Result)
            {
                idList.Add(loanId.LoanId);
            }
            var unparsedList = _context.LoanStatuses.Where(l => idList.Contains(l.LoanId)).OrderBy(l => l.LoanId).ThenBy(l => l.StatusDate).ToListAsync();
            for (var i = 0; i < unparsedList.Result.Count; i++)
            {
                if(unparsedList.Result[i].StatusCode == statusCode)
                {
                    if(unparsedList.Result[i] == unparsedList.Result.Last())
                    {
                        count++;
                    }
                    else
                    {
                        if(unparsedList.Result[i].LoanId != unparsedList.Result[i + 1].LoanId)
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }
    }
}
