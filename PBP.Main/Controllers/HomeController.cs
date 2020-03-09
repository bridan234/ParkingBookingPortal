using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EmailSender;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using PBP.BusinessLogic;
using PBP.DataAccess;
using PBP.Main.Models;
using PBP.Pocos;
using Stripe;

namespace PBP.Main.Controllers
{
    public class HomeController : Controller
    {
        private DBModel db;
        private readonly IConfiguration _config;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _config = config;
            _logger = logger;
            db = new DBModel();
        }

        public IActionResult Index()
        {
            IDataRepository<CalendarPoco> repo = new DataRepository<CalendarPoco>();
            IList<DateTime> UnAvailableDates = repo.GetList(days => days.AvailableSlots <= 0).Select(d => d.Date).ToList();
            
            return View(UnAvailableDates);
        }

        public IActionResult Payment(
              string dateReserved
            , string carPlateNumber
            , string email
            , string fullName
            , string phone
            , DateTime transactionDate
            , int numberOfDaysReserved
            , decimal amountPaid)
        { 
            var charge = new Charge();
            // Check to see that Payment was successful
            if (charge.StripeResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                #region Creating a Reservation and Transaction Objects before saving
                var Reservation = new ReservationPoco() 
                { Id=Guid.NewGuid()
                    , CarPlateNumber= carPlateNumber
                    , FullName= fullName
                    , Email=email
                    , Phone=phone
                    , Date=DateTime.Now
                    , DatesReserved=dateReserved
                    , NumberOfDaysReserved=numberOfDaysReserved
                };

                var Transaction = new TransactionPoco() 
                { Id = Guid.NewGuid()
                    , AmountPaid = amountPaid
                    , ReservationId = Reservation.Id
                    , Details = charge.StripeResponse.Content
                    , MerchantId = charge.Id
                    , Reservation=Reservation 
                };
                #endregion

                // Save/Update all Information into Database
                saveToDatabase(Transaction, Reservation, splitDates(dateReserved));

                //Generate Receipt PDF


                //Generate Reservation Receipt
                var receipt = new object [] {fullName, 
                    Reservation.Id,
                    Reservation.DatesReserved, 
                    Reservation.CarPlateNumber, 
                    Transaction.AmountPaid, 
                    Transaction.MerchantId 
                };

                //MailSender.SendMail(new EMail() { ToAddress = email }, ViewAsPdf(receipt));

                RedirectToAction("TransactionSuccessful", receipt);
                //return View();
            }

            // If Payment wasn't successful
            ViewBag.ErrorMessage = "Payment was unssucessful, Please try again with another card or contact your Bank ";
            return View();
        }

        public IActionResult TransactionSuccessful(object [] receipt)
        {
            return View(receipt.ToList());
        }

        //This method splits/seperates the dates and returns an array of dates
        private DateTime[] splitDates(string dateReserved)
        {
            string[] singleDate = dateReserved.Split(',');

            DateTime[] dates = new DateTime[singleDate.Length];

            for (int i=0; i<singleDate.Length; i++)
            {
                dates[i] = Convert.ToDateTime(singleDate[i]);
            }
            return dates.ToArray();
        }

        private void saveToDatabase(TransactionPoco transaction, ReservationPoco reservation, DateTime[] Dates)
        {
            #region Begin Saving / Updating the Calendar Table
            for (int i = 0; i < Dates.Length; i++)
            {
                var logic = new CalendarLogic(new DataRepository<CalendarPoco>());
                var dateRecord = logic.Get(d => d.Date == Dates[i]); //Tries to search if Date already exists in DB
                if (dateRecord == null)
                {
                     dateRecord = new CalendarPoco() { Id = Guid.NewGuid()
                        , AvailableSlots = _config.GetValue<int>("Calendar:Slots") - 1
                        , ReservedSlots = 1, TotalSlots = _config.GetValue<int>("Calendar:Slots")
                        , Date = Dates[i] };
                    logic.Add(dateRecord);
                }
                else
                {
                    dateRecord.AvailableSlots = dateRecord.AvailableSlots - 1;
                    dateRecord.ReservedSlots = dateRecord.ReservedSlots + 1;
                    logic.Update(dateRecord);
                }
            }
            #endregion //End Saving / Updating the Calendar Table


            #region Begin Saving to the Reservations Table
            {
                var logic = new ReservationLogic(new DataRepository<ReservationPoco>());
                logic.Add(reservation);
            }
            #endregion //End Saving to the Reservations Table

            #region Begin Saving to the Transactions Table
            {
                var logic = new TransactionLogic(new DataRepository<TransactionPoco>());
                logic.Add(transaction);
            }
            #endregion //End Saving to the Transactions Table

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
