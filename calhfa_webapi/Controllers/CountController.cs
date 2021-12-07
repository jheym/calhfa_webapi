using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CalhfaWebapi.Models;
using System.Globalization;

namespace CalhfaWebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountController : Controller
    {
        private readonly DBContext _context;

        public CountController(DBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a count of loans in the first and subordinate queues.
        /// </summary>
        /// <remarks>
        /// Sample JSON returned by count
        /// 
        ///     GET api/count
        ///     {
        ///         "PreClosing" : {
        ///             "compliance" : {
        ///                 "count" : 10,
        ///                 "date" : 2021-08-21
        ///             },
        ///             "inSuspense" : {
        ///                 "count" : 4,
        ///                 "date" : 2021-07-22
        ///             }
        ///         },
        ///         "PostClosing" : {
        ///             "compliance" : {
        ///                 "count" : 2,
        ///                 "date" : 2021-10-01
        ///             },
        ///             "inSuspense" : {
        ///                 "count" : 3,
        ///                 "date" : 2021-12-25
        ///             }
        ///         }
        ///   }
        ///   
        /// </remarks>
        /// <returns> a json formatted string which contains the counts and oldest dates for first and subordinate queues </returns>
        // GET: /api/LoanStatus/count
        [HttpGet]
        public Dictionary<string,Dictionary<string, LoanStatus>> GetLoanCount()
        {
            string dateFormatting = "yyyy-MM-dd";
            Dictionary<string, LoanStatus> preClosingLoansCounts = new();
            Dictionary<string, LoanStatus> postClosingLoansCounts = new();

            var preClosingComplianceList = (GetQueueList(410, 1)); //returns list of loans which are PRE closing and in Compliance Review
            var preClosingComplianceeDate = GetReviewDate(preClosingComplianceList); 
            LoanStatus preClosingComplianceCounts = new(preClosingComplianceList.Count, preClosingComplianceeDate.ToString(dateFormatting));

            var preClosingInSuspenseList = GetQueueList(422, 1); //returns list of loans which are PRE closing and in suspense review
            var preClosingInSuspenseDate = GetReviewDate(preClosingInSuspenseList);
            LoanStatus preClosingInSuspenseCounts = new(preClosingInSuspenseList.Count, preClosingInSuspenseDate.ToString(dateFormatting));

            // builds a dictionary which mirrors JSON formatting for the first Section of website
            preClosingLoansCounts.Add("compliance", preClosingComplianceCounts);
            preClosingLoansCounts.Add("inSuspense", preClosingInSuspenseCounts);


            var postClosingComplianceList= GetQueueList(510, 2); //returns list of loans which are POST closing and in compliance review
            var postClosingComplianceDate = GetReviewDate(postClosingComplianceList);
            LoanStatus postClosingComplianceCounts= new(postClosingComplianceList.Count, postClosingComplianceDate.ToString(dateFormatting));

            var postClosingInSuspenseList = GetQueueList(522, 2); //returns list of loans which are POST closing and in suspense review
            var postClosingInSuspenseDate = GetReviewDate(postClosingInSuspenseList);
            LoanStatus postClosingInSuspenseCount = new(postClosingInSuspenseList.Count, postClosingInSuspenseDate.ToString(dateFormatting));

            // builds a dictionary which mirrors JSON formatting for the post closing Section of website
            postClosingLoansCounts.Add("compliance", postClosingComplianceCounts);
            postClosingLoansCounts.Add("inSuspense", postClosingInSuspenseCount);

            // combines both table sections (pre closing and post closing) into one dictionary
            // web api will serialize output to JSON formatting.
            // formatting is automatic and any object/data structure which be stored in an Enumerable type 
            // can be serialized to JSON formatting.
            return new Dictionary<string, Dictionary<string, LoanStatus>>
            {
                { "PreClosing", preClosingLoansCounts},
                { "PostClosing", postClosingLoansCounts}
            };
        }

        /// <summary>
        /// Queries the database for the loans which match the specified statuscode and categoryID.
        /// The query is stored as a list with columns matching LoanID, LoanCategoryID, StatusCode, and StatusDate
        /// </summary>
        /// <param name="statusCode">int</param>
        /// <param name="categoryID">int</param>
        /// <returns>list with all loans in specified queue</returns>
        private List<ReviewCount> GetQueueList(int statusCode, int categoryID)
        {
            string sqlQuery = @"SELECT LoanStatus.StatusDate
                                FROM Loan
                                INNER JOIN(
                                    SELECT LoanStatus.LoanID, LoanStatus.StatusCode, LoanStatus.StatusSequence, LoanStatus.StatusDate
                                    FROM LoanStatus
                                    INNER JOIN (
                                        SELECT LoanStatus.LoanID, MAX(LoanStatus.StatusSequence) AS StatusSequence
                                        FROM LoanStatus
                                        GROUP BY LoanID
                                    ) MaxTable ON LoanStatus.LoanID = MaxTable.LoanID AND LoanStatus.StatusSequence = MaxTable.StatusSequence
                                ) LoanStatus ON Loan.LoanID = LoanStatus.LoanID
                                INNER Join(
                                    SELECT LoanType.LoanCategoryID, LoanType.LoanTypeID
                                    FROM LoanType
                                    WHERE LoanType.LoanCategoryID = {0}     
                                ) LoanType ON LoanType.LoanTypeID = Loan.LoanTypeID
                                WHERE StatusCode = {1}
                                ORDER BY Loan.LoanID";
            var queuedLoans = _context.ReviewQueue.FromSqlRaw(sqlQuery, categoryID, statusCode).ToList();

            return queuedLoans;
        }

        private DateTime GetReviewDate(List<ReviewCount> list)
        {
            DateTime reviewDate;

            if (list.Count != 0)
            {
                reviewDate = list[0].StatusDate;
                for(int i = 1; i < list.Count; i++)
                {
                    // returns oldest (first in queue)
                    if (list[i].StatusDate < reviewDate)
                    {
                        reviewDate = list[i].StatusDate;
                    }
                }
            } else
            {
                reviewDate = DateTime.Now; // returns current date if Count = 0
            }

            return reviewDate;
        }
        
        // each instance of LoanStatus is one row in the website's table data
        // keeps track of how many lines are in that queue and what the oldest date in the queue is
        public class LoanStatus
        {
            public LoanStatus(int count, string date)
            {
                this.count = count;
                this.date = date;
            }
            public int count { get; set; }
            public string date { get; set; }
        }
    }
}
