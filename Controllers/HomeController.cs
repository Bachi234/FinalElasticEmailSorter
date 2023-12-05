//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;
//using System.Data;
//using automationTest.Models;
//using automationTest.Service;
//using Microsoft.Extensions.Caching.Memory;
//using automationTest.Context;
//using automationTest.ViewModel;

//namespace automationTest.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ApplicationDbContext_ __context;
//        private readonly ElasticDataService _tblElasticData;
//        private readonly EventDataService _tblEventData;
//        private readonly IMemoryCache _memoryCache;
//        private readonly ILogger _logger;

//        public HomeController(ApplicationDbContext context, ApplicationDbContext_ context_, ElasticDataService elasticDataServices, 
//            EventDataService eventDataServices, ILogger<HomeController> logger, IMemoryCache memoryCache)
//        {
//            _context = context;
//            __context = context_;
//            _tblElasticData = elasticDataServices;
//            _tblEventData = eventDataServices;
//            _logger = logger;
//            _memoryCache = memoryCache;
//        }

//        public IActionResult Index(string searchMailNumbers)
//        {
//            ViewBag.SearchMailNumbers = searchMailNumbers;
//            List<tblEvent> events = new List<tblEvent>();

//            if (!string.IsNullOrWhiteSpace(searchMailNumbers))
//            {
//                // Split the input by whitespace to get individual mail numbers
//                string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//                foreach (var mailNumber in mailNumbers)
//                {
//                    List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
//                    events.AddRange(eventsForMailNumber);
//                }
//            }
//            return View(events);
//        }

//        [HttpGet]
//        public IActionResult DisplayElasticData(string searchMailNumbers, string searchSubject, DateTime? startDate, DateTime? endDate)
//        {
//            ViewBag.SearchSubject = searchSubject;
//            ViewBag.SearchMailNumber = searchMailNumbers;
//            ViewBag.SearchSubjectDateStart = startDate;
//            ViewBag.SearchSubjectDateEnd = endDate;
//            ViewBag.LastMailNumber = searchMailNumbers;

//            List<tblEvent> events = new List<tblEvent>();

//            if (!string.IsNullOrWhiteSpace(searchMailNumbers))
//            {
//                string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//                foreach (var mailNumber in mailNumbers)
//                {
//                    List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
//                    events.AddRange(eventsForMailNumber);
//                }
//            }
//            List<tblElasticData> elasticData;
//            if (startDate != null && endDate != null)
//            {
//                elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);
//                // Aggregation on the data using LINQ
//                var aggregatedData = elasticData
//                    .GroupBy(data => new { data.To, data.From, data.Subject, data.EventType, data.EventDate, data.Channel, data.MessageCategory })
//                    .Select(group => new tblElasticData
//                    {
//                        To = group.Key.To,
//                        From = group.Key.From,
//                        Subject = group.Key.Subject,
//                        EventType = group.Key.EventType,
//                        EventDate = group.Key.EventDate,
//                        Channel = group.Key.Channel,
//                        MessageCategory = group.Key.MessageCategory,
//                        Quantity = group.Count()
//                    })
//                    .ToList();

//                // Create the composite view model
//                var compositeViewModel = new CombinedViewModel
//                {
//                    EventData = events,
//                    ElasticData = aggregatedData
//                };

//                return View(compositeViewModel);
//            }
//            else
//            {
//                elasticData = _tblElasticData.GetElasticDataBySubject(searchSubject);
//                if (!string.IsNullOrEmpty(searchSubject))
//                {
//                    // Aggregation on the data using LINQ for searchSubject
//                    var aggregatedData = elasticData
//                        .GroupBy(data => new { data.To, data.From, data.Subject, data.EventType, data.EventDate, data.Channel, data.MessageCategory })
//                        .Select(group => new tblElasticData
//                        {
//                            To = group.Key.To,
//                            From = group.Key.From,
//                            Subject = group.Key.Subject,
//                            EventType = group.Key.EventType,
//                            EventDate = group.Key.EventDate,
//                            Channel = group.Key.Channel,
//                            MessageCategory = group.Key.MessageCategory,
//                            Quantity = group.Count()
//                        })
//                        .ToList();

//                    // Create the composite view model
//                    var compositeViewModel = new CombinedViewModel
//                    {
//                        EventData = events,
//                        ElasticData = aggregatedData
//                    };

//                    return View(compositeViewModel);
//                }
//            }

//            // Create the composite view model without aggregation for searchSubject
//            var compositeViewModelWithoutAggregation = new CombinedViewModel
//            {
//                EventData = events,
//                ElasticData = elasticData
//            };

//            return View(compositeViewModelWithoutAggregation);
//        }

//        [HttpPost]
//        public IActionResult DisplayElasticData(DateTime? startDate, DateTime? endDate)
//        {
//            List<tblElasticData> elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);

//            if (startDate != null && endDate != null)
//            {
//                elasticData = elasticData.Where(data => data.EventDate.Date >= startDate && data.EventDate.Date <= endDate).ToList();
//            }

//            ViewBag.SearchSubject = null; // Clear search subject when filtering by dates
//            ViewBag.SearchSubjectDateStart = startDate;
//            ViewBag.SearchSubjectDateEnd = endDate;

//            return View(elasticData);
//        }


//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }   
//    }

//}

//[HttpGet]
//public IActionResult DisplayElasticData(string searchMailNumbers, string searchSubject, DateTime? startDate, DateTime? endDate)
//{
//    ViewBag.SearchSubject = searchSubject;
//    ViewBag.SearchMailNumber = searchMailNumbers;
//    ViewBag.SearchSubjectDateStart = startDate;
//    ViewBag.SearchSubjectDateEnd = endDate;

//    List<tblEvent> events = new List<tblEvent>();

//    if (!string.IsNullOrWhiteSpace(searchMailNumbers))
//    {
//        string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//        foreach (var mailNumber in mailNumbers)
//        {
//            List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
//            events.AddRange(eventsForMailNumber);
//        }
//    }

//    List<tblElasticData> elasticData;

//    if (startDate != null && endDate != null)
//    {
//        elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);
//        // Aggregation on the data using LINQ
//        var aggregatedData = elasticData
//            .GroupBy(data => new { data.To, data.From, data.Subject, data.EventType, data.EventDate, data.Channel, data.MessageCategory })
//            .Select(group => new tblElasticData
//            {
//                To = group.Key.To,
//                From = group.Key.From,
//                Subject = group.Key.Subject,
//                EventType = group.Key.EventType,
//                EventDate = group.Key.EventDate,
//                Channel = group.Key.Channel,
//                MessageCategory = group.Key.MessageCategory,
//                Quantity = group.Count()
//            })
//            .ToList();

//        // Create the composite view model
//        var compositeViewModel = new CombinedViewModel
//        {
//            EventData = events,
//            ElasticData = aggregatedData
//        };

//        return View(compositeViewModel);
//    }
//    else
//    {
//        elasticData = _tblElasticData.GetElasticDataBySubject(searchSubject);
//        if (!string.IsNullOrEmpty(searchSubject))
//        {
//            List<tblElasticData> mailNumberData = _tblElasticData.GetElasticDataBySubject(searchMailNumbers);
//            elasticData = mailNumberData.Any() ? mailNumberData : elasticData;
//        }

//        // Create the composite view model
//        var compositeViewModel = new CombinedViewModel
//        {
//            EventData = events,
//            ElasticData = elasticData
//        };

//        return View(compositeViewModel);
//    }
//}
//public IActionResult Index(string searchMailNumbers)
//{
//    ViewBag.SearchMailNumbers = searchMailNumbers;
//    List<tblEvent> events = new List<tblEvent>();

//    if (!string.IsNullOrWhiteSpace(searchMailNumbers))
//    {
//        // Split the input by whitespace to get individual mail numbers
//        string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//        foreach (var mailNumber in mailNumbers)
//        {
//            List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
//            events.AddRange(eventsForMailNumber);
//        }
//    }
//    return View(events);
//}

//LATEST CODE
//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;
//using System.Data;
//using automationTest.Models;
//using automationTest.Service;
//using Microsoft.Extensions.Caching.Memory;
//using automationTest.Context;
//using automationTest.ViewModel;

//namespace automationTest.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ApplicationDbContext_ __context;
//        private readonly ElasticDataService _tblElasticData;
//        private readonly EventDataService _tblEventData;
//        private readonly IMemoryCache _memoryCache;
//        private readonly ILogger _logger;

//        public HomeController(ApplicationDbContext context, ApplicationDbContext_ context_, ElasticDataService elasticDataServices,
//            EventDataService eventDataServices, ILogger<HomeController> logger, IMemoryCache memoryCache)
//        {
//            _context = context;
//            __context = context_;
//            _tblElasticData = elasticDataServices;
//            _tblEventData = eventDataServices;
//            _logger = logger;
//            _memoryCache = memoryCache;
//        }

//        [HttpGet]
//        public IActionResult DisplayElasticData(string searchMailNumbers, string searchSubject, DateTime? startDate, DateTime? endDate)
//        {
//            ViewBag.SearchSubject = searchSubject;
//            ViewBag.SearchMailNumber = searchMailNumbers;
//            ViewBag.SearchSubjectDateStart = startDate;
//            ViewBag.SearchSubjectDateEnd = endDate;

//            //SEARCH MAIL NUMBER
//            List<tblEvent> events = new List<tblEvent>();
//            if (!string.IsNullOrWhiteSpace(searchMailNumbers))
//            {
//                string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//                foreach (var mailNumber in mailNumbers)
//                {
//                    List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
//                    events.AddRange(eventsForMailNumber);
//                }
//            }
//            //DATE FILTER CONTROLLER
//            List<tblElasticData> elasticData;
//            if (startDate != null && endDate != null)
//            {
//                elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);
//                // Aggregation on the data using LINQ
//                var aggregatedData = elasticData
//                    .GroupBy(data => new { data.To, data.From, data.Subject, data.EventType, data.EventDate, data.Channel, data.MessageCategory })
//                    .Select(group => new tblElasticData
//                    {
//                        To = group.Key.To,
//                        From = group.Key.From,
//                        Subject = group.Key.Subject,
//                        EventType = group.Key.EventType,
//                        EventDate = group.Key.EventDate,
//                        Channel = group.Key.Channel,
//                        MessageCategory = group.Key.MessageCategory,
//                        Quantity = group.Count()
//                    })
//                    .ToList();

//                // Create the composite view model
//                var compositeViewModel = new CombinedViewModel
//                {
//                    EventData = events,
//                    ElasticData = aggregatedData
//                };

//                return View(compositeViewModel);
//            }
//            else //SEARCH SUBJECT FILTER
//            {
//                elasticData = _tblElasticData.GetElasticDataBySubject(searchSubject);
//                if (!string.IsNullOrEmpty(searchSubject))
//                {
//                    // Aggregation on the data using LINQ for searchSubject
//                    var aggregatedData = elasticData
//                        .GroupBy(data => new { data.To, data.From, data.Subject, data.EventType, data.EventDate, data.Channel, data.MessageCategory })
//                        .Select(group => new tblElasticData
//                        {
//                            To = group.Key.To,
//                            From = group.Key.From,
//                            Subject = group.Key.Subject,
//                            EventType = group.Key.EventType,
//                            EventDate = group.Key.EventDate,
//                            Channel = group.Key.Channel,
//                            MessageCategory = group.Key.MessageCategory,
//                            Quantity = group.Count()
//                        })
//                        .ToList();

//                    // Create the composite view model
//                    var compositeViewModel = new CombinedViewModel
//                    {
//                        EventData = events,
//                        ElasticData = aggregatedData
//                    };

//                    return View(compositeViewModel);
//                }
//            }
//            // Create the composite view model without aggregation for searchSubject
//            var compositeViewModelWithoutAggregation = new CombinedViewModel
//            {
//                EventData = events,
//                ElasticData = elasticData
//            };
//            return View(compositeViewModelWithoutAggregation);
//        }
//        //[HttpPost]
//        //public IActionResult DisplayElasticData(DateTime? startDate, DateTime? endDate)
//        //{
//        //    List<tblElasticData> elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);

//        //    if (startDate != null && endDate != null)
//        //    {
//        //        elasticData = elasticData.Where(data => data.EventDate.Date >= startDate && data.EventDate.Date <= endDate).ToList();
//        //    }

//        //    ViewBag.SearchSubject = null; // Clear search subject when filtering by dates
//        //    ViewBag.SearchSubjectDateStart = startDate;
//        //    ViewBag.SearchSubjectDateEnd = endDate;

//        //    return View(elasticData);
//        //}
//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}

//[HttpGet]
//public IActionResult DisplayElasticData(string searchMailNumbers, string searchSubject, DateTime? startDate, DateTime? endDate)
//{
//    ViewBag.SearchSubject = searchSubject;
//    ViewBag.SearchMailNumber = searchMailNumbers;
//    ViewBag.SearchSubjectDateStart = startDate;
//    ViewBag.SearchSubjectDateEnd = endDate;

//    List<tblEvent> events = new List<tblEvent>();

//    if (!string.IsNullOrWhiteSpace(searchMailNumbers))
//    {
//        events.AddRange(GetEventsForMailNumbers(searchMailNumbers));
//    }

//    List<tblElasticData> elasticData;
//    if (startDate != null && endDate != null)
//    {
//        elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);
//    }
//    else
//    {
//        elasticData = GetElasticDataBySubject(searchSubject);
//    }

//    // Aggregation on the data using LINQ
//    var aggregatedData = elasticData
//        .GroupBy(data => new { data.To, data.From, data.Subject, 
//            data.EventType, data.EventDate, data.Channel, data.MessageCategory })
//        .Select(group => new tblElasticData
//        {
//            To = group.Key.To,
//            From = group.Key.From,
//            Subject = group.Key.Subject,
//            EventType = group.Key.EventType,
//            EventDate = group.Key.EventDate,
//            Channel = group.Key.Channel,
//            MessageCategory = group.Key.MessageCategory,
//            Quantity = group.Count()
//        })
//        .ToList();

//    // Create the composite view model
//    var compositeViewModel = new CombinedViewModel
//    {
//        EventData = events,
//        ElasticData = aggregatedData
//    };

//    return View(compositeViewModel);
//}

//private List<tblEvent> GetEventsForMailNumbers(string searchMailNumbers)
//{
//    List<tblEvent> events = new List<tblEvent>();

//    string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//    foreach (var mailNumber in mailNumbers)
//    {
//        List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
//        events.AddRange(eventsForMailNumber);
//    }
//    return events;
//}

//private List<tblElasticData> GetElasticDataBySubject(string searchSubject)
//{
//    List<tblElasticData> elasticData = _tblElasticData.GetElasticDataBySubject(searchSubject);

//    // Aggregation on the data using LINQ for searchSubject
//    var aggregatedData = elasticData
//        .GroupBy(data => new { data.To, data.From, data.Subject, 
//            data.EventType, data.EventDate, data.Channel, data.MessageCategory })
//        .Select(group => new tblElasticData
//        {
//            To = group.Key.To,
//            From = group.Key.From,
//            Subject = group.Key.Subject,
//            EventType = group.Key.EventType,
//            EventDate = group.Key.EventDate,
//            Channel = group.Key.Channel,
//            MessageCategory = group.Key.MessageCategory,
//            Quantity = group.Count()
//        })
//        .ToList();

//    return aggregatedData;
//}

//[HttpGet]
//public IActionResult DisplayElasticData(string searchMailNumbers, string searchSubject, DateTime? startDate, DateTime? endDate)
//{
//    ViewBag.SearchSubject = searchSubject;
//    ViewBag.SearchMailNumber = searchMailNumbers;
//    ViewBag.SearchSubjectDateStart = startDate;
//    ViewBag.SearchSubjectDateEnd = endDate;

//    List<tblEvent> events = new List<tblEvent>();

//    if (!string.IsNullOrWhiteSpace(searchMailNumbers))
//    {
//        events.AddRange(GetEventsForMailNumbers(searchMailNumbers));
//    }

//    List<tblElasticData> elasticData;
//    if (startDate != null && endDate != null)
//    {
//        elasticData = _tblElasticData.GetElasticDataByDate(startDate, endDate);
//    }
//    else
//    {
//        elasticData = GetElasticData(searchSubject, aggregate: true);
//    }

//    // Create the composite view model
//    var compositeViewModel = new CombinedViewModel
//    {
//        EventData = events,
//        ElasticData = elasticData
//    };

//    return View(compositeViewModel);
//}

//private List<tblElasticData> GetElasticData(string searchSubject, bool aggregate)
//{
//    List<tblElasticData> elasticData = _tblElasticData.GetElasticDataBySubject(searchSubject);

//    if (aggregate)
//    {
//        // Aggregation on the data using LINQ
//        elasticData = elasticData
//            .GroupBy(data => new {
//                data.To,
//                data.From,
//                data.Subject,
//                data.EventType,
//                data.EventDate,
//                data.Channel,
//                data.MessageCategory
//            })
//            .Select(group => new tblElasticData
//            {
//                To = group.Key.To,
//                From = group.Key.From,
//                Subject = group.Key.Subject,
//                EventType = group.Key.EventType,
//                EventDate = group.Key.EventDate,
//                Channel = group.Key.Channel,
//                MessageCategory = group.Key.MessageCategory,
//                Quantity = group.Count()
//            })
//            .ToList();
//    }

//    return elasticData;
//}
//private List<tblEvent> GetEventsForMailNumbers(string searchMailNumbers)
//{
//    List<tblEvent> events = new List<tblEvent>();

//    string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

//    foreach (var mailNumber in mailNumbers)
//    {
//        List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
//        events.AddRange(eventsForMailNumber);
//    }
//    return events;
//}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using automationTest.Context;
using automationTest.Models;
using automationTest.Service;
using automationTest.ViewModel;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace automationTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly MktgDbContext _mktgDbContext;
        private readonly ElasticDbContext _elasticDbContext;
        private readonly EventDataService _tblEventData;
        private readonly ElasticDataService _tblElasticData;
        private readonly ILogger<HomeController> _logger;


        public HomeController(MktgDbContext mktgDbContext, ElasticDbContext elasticDbContext, EventDataService eventDataServices,
                ElasticDataService elasticDataServices, ILogger<HomeController> logger)
        {
            _mktgDbContext = mktgDbContext;
            _elasticDbContext = elasticDbContext;
            _tblEventData = eventDataServices;
            _tblElasticData = elasticDataServices;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult DisplayElasticData(string searchMailNumbers, string searchSubject, DateTime? startDate, DateTime? endDate)
        {
            ViewBag.SearchSubject = searchSubject;
            ViewBag.SearchMailNumber = searchMailNumbers;
            ViewBag.SearchSubjectDateStart = startDate;
            ViewBag.SearchSubjectDateEnd = endDate;

            List<tblEvent> events = new List<tblEvent>();

            if (!string.IsNullOrWhiteSpace(searchMailNumbers))
            {
                events.AddRange(GetEventsForMailNumbers(searchMailNumbers));
            }

            List<CombinedModel> elasticData;
            if (startDate != null && endDate != null)
            {
                elasticData = _tblElasticData.GetCombinedDataByDate(startDate, endDate);
            }
            else
            {
                elasticData = GetElasticDataBySubject(searchSubject);
            }

            // Create the composite view model
            var compositeViewModel = new CombinedViewModel
            {
                EventData = events,
                CombinedModel = elasticData
            };

            return View(compositeViewModel);
        }

        private List<tblEvent> GetEventsForMailNumbers(string searchMailNumbers)
        {
            List<tblEvent> events = new List<tblEvent>();

            string[] mailNumbers = searchMailNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var mailNumber in mailNumbers)
            {
                List<tblEvent> eventsForMailNumber = _tblEventData.GetMailNumber(mailNumber.Trim());
                events.AddRange(eventsForMailNumber);
            }
            return events;
        }

        private List<CombinedModel> GetElasticDataBySubject(string searchSubject)
        {
            List<tblElasticData> elasticData = _tblElasticData.GetElasticDataBySubject(searchSubject);

            // Aggregation on the data using LINQ for searchSubject
            var aggregatedData = elasticData
                .GroupBy(data => new
                {
                    data.To,
                    data.From,
                    data.Subject,
                    data.EventType,
                    data.EventDate,
                    data.Channel,
                    data.MessageCategory
                })
                .Select(group => new CombinedModel
                {
                    To = group.Key.To,
                    From = group.Key.From,
                    Subject = group.Key.Subject,
                    EventType = group.Key.EventType,
                    EventDate = group.Key.EventDate,
                    Channel = group.Key.Channel,
                    MessageCategory = group.Key.MessageCategory,
                    Quantity = group.Count()
                })
                .ToList();

            return aggregatedData;
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}