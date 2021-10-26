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
        /// <summary>
        /// Returns the loan with the specififed LoanStatusId. Returns 404 if not found.
        /// </summary>
        /// <param name="id"> int </param>
        /// <returns> loanStatus </returns>
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
            var ComplianceReviewList = GetReviewList(410);
            var ComplianceReviewDate = GetReviewDate(ComplianceReviewList);

            var ComplianceSuspenseReviewList = GetReviewList(422);
            var ComplianceSuspenseDate = GetReviewDate(ComplianceSuspenseReviewList);

            var PurchaseReviewList = GetReviewList(510);
            var PurchaseReviewDate = GetReviewDate(PurchaseReviewList);

            var PurchaseSuspenseReviewList = GetReviewList(522);
            var PurchaseSuspenseDate = GetReviewDate(PurchaseSuspenseReviewList);

            string jsonData = String.Format("{{compliantReview: {{count: '{0}', date: '{1}'}}, compliantSuspense: {{ count: '{2}', date: '{3}' }}, purchaseReview: {{ count: '{4}', date: '{5}' }}, purchaseSuspense: '{{ count: '{6}', date: '{7}' }} }}",
                ComplianceReviewList.Count, ComplianceReviewDate, ComplianceSuspenseReviewList.Count, ComplianceSuspenseDate, PurchaseReviewList.Count, PurchaseReviewDate, PurchaseSuspenseReviewList.Count, PurchaseSuspenseDate);

            return Newtonsoft.Json.JsonConvert.SerializeObject(jsonData);
        }

        /// <summary>
        /// takes a specific status code and counts how many loans are still at that specific stage of review
        /// </summary>
        /// <param name="statusCode">int</param>
        /// <returns>a count of the loans still at that particular stage</returns>
        private List<LoanStatus> GetReviewList(int statusCode)
        {
            int count = 0;
            var ComplianceReviewList =  _context.LoanStatuses.Where(l => l.StatusCode == statusCode).ToListAsync();
            List<int> idList = new List<int>();
            // adds all IDs which have the statusCode
            foreach ( var loanId in ComplianceReviewList.Result)
            {
                idList.Add(loanId.LoanId);
            }
            // queries database again and sorts results based on LoanID and StatusDate 
            var unparsedList = _context.LoanStatuses.Where(l => idList.Contains(l.LoanId)).OrderBy(l => l.LoanId).ThenBy(l => l.StatusDate).ToListAsync();
            List<LoanStatus> parsedList = new List<LoanStatus>();
            for (var i = 0; i < unparsedList.Result.Count; i++)
            {
                if(unparsedList.Result[i].StatusCode == statusCode)
                {
                    if(unparsedList.Result[i] == unparsedList.Result.Last())
                    {
                        parsedList.Add(unparsedList.Result[i]);
                    }
                    else
                    {
                        if(unparsedList.Result[i].LoanId != unparsedList.Result[i + 1].LoanId)
                        {
                            parsedList.Add(unparsedList.Result[i]);
                        }
                    }
                }
            }

            return parsedList;
        }

        private DateTime GetReviewDate(List<LoanStatus> list)
        {
            var reviewDate = list[0].StatusDate;
            foreach (var loan in list)
            {
                //NOTE this can change depending on whether the latest or oldest date is needed
                if(loan.StatusDate.HasValue && loan.StatusDate < reviewDate)
                {
                    reviewDate = loan.StatusDate;
                }
            }

            return (DateTime) reviewDate;
        }
    }
}
