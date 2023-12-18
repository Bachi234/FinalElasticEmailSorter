using automationTest.Context;
using automationTest.Models;

namespace automationTest.Service
{
    public class ElasticDataService
    {
        private readonly MktgDbContext _mktgDbContext;
        private readonly ElasticDbContext _elasticDbContext;
        public ElasticDataService(MktgDbContext mktgDbContext, ElasticDbContext elasticDbContext)
        {
            _mktgDbContext = mktgDbContext;
            _elasticDbContext = elasticDbContext;
        }

        public List<tblElasticData> GetElasticDataBySubject(string searchSubject)
        {
            if (string.IsNullOrWhiteSpace(searchSubject))
            {
                Console.WriteLine("Something went wrong.");
                return new List<tblElasticData>();
            }

            var query = _elasticDbContext.tblElasticData
                .Where(data => !string.IsNullOrWhiteSpace(data.Subject) && data.Subject.Contains(searchSubject));

            var result = ProjectElasticDataProperties(query).ToList();
            return result;
        }

        public List<CombinedModel> GetCombinedDataByDate(DateTime? startDate, DateTime? endDate)
        {
            startDate ??= DateTime.MinValue;
            endDate ??= DateTime.MaxValue;

            var elasticData = _elasticDbContext.tblElasticData
                .Where(data => data.EventDate.Date >= startDate.Value.Date && data.EventDate.Date <= endDate.Value.Date)
                .GroupBy(data => new
                {
                    data.Mail_Number,
                    data.To,
                    data.From,
                    data.Subject,
                    data.EventType,
                    data.EventDate,
                    data.Channel,
                    data.MessageCategory,
                })
                .Select(group => new tblElasticData
                {
                    Mail_Number = group.Key.Mail_Number,
                    To = group.Key.To,
                    From = group.Key.From,
                    Subject = group.Key.Subject,
                    EventType = group.Key.EventType,
                    EventDate = group.Key.EventDate,
                    Channel = group.Key.Channel,
                    MessageCategory = group.Key.MessageCategory,
                    Quantity = group.Count(),
                })
                .ToList();

            var eventDataFromOtherDb = _mktgDbContext.tblEvent.ToList();
            var combinedData = CombineData(elasticData, eventDataFromOtherDb);

            return combinedData;
        }

        private List<CombinedModel> CombineData(List<tblElasticData> elasticData, List<tblEvent> eventDataFromOtherDb)
        {
            var combinedDataDictionary = new Dictionary<string, CombinedModel>();

            foreach (var elasticItem in elasticData)
            {
                var mailNumbers = GetMailNumbersFromEvent(elasticItem, eventDataFromOtherDb);

                var key = GenerateKey(elasticItem, mailNumbers);

                if (combinedDataDictionary.TryGetValue(key, out var existingCombinedItem))
                {
                    // If the key already exists, update the Quantity
                    existingCombinedItem.Quantity += elasticItem.Quantity;
                }
                else
                {
                    // If the key doesn't exist, create a new CombinedModel
                    var combinedItem = new CombinedModel
                    {
                        Mail_Number = mailNumbers.Count > 0 ? mailNumbers[0] : "N/A",
                        To = elasticItem.To,
                        From = elasticItem.From,
                        Subject = elasticItem.Subject,
                        Quantity = elasticItem.Quantity,
                        EventType = elasticItem.EventType,
                        EventDate = elasticItem.EventDate,
                        Channel = elasticItem.Channel,
                        MessageCategory = elasticItem.MessageCategory
                    };

                    combinedDataDictionary[key] = combinedItem;
                }
            }

            // Convert the values of the dictionary to a list
            var combinedDataList = combinedDataDictionary.Values.ToList();

            return combinedDataList;
        }

        private string GenerateKey(tblElasticData elasticItem, List<string> mailNumbers)
        {
            // Include all properties in the key for similarity check
            var key = $"{string.Join("-", mailNumbers)}-{elasticItem.To}-{elasticItem.From}-{elasticItem.Subject}-{elasticItem.EventType}-{elasticItem.EventDate}-{elasticItem.Channel}-{elasticItem.MessageCategory}";

            return key;
        }

        private List<string> GetMailNumbersFromEvent(tblElasticData elasticItem, List<tblEvent> eventDataFromOtherDb)
        {
            // Implement the logic to find and return Mail_Number from tblEvent based on some condition.
            // For example, you might want to find matches based on a common identifier or property.
            var matchingEvents = eventDataFromOtherDb
                .Where(e => e.Subject == elasticItem.Subject)
                .Select(e => e.Mail_Number)
                .ToList();

            // Return a list of Mail_Numbers. The list might be empty if no matches are found.
            return matchingEvents;
        }

        private IQueryable<tblElasticData> ProjectElasticDataProperties(IQueryable<tblElasticData> dataQuery)
        {
            return dataQuery.Select(data => new tblElasticData
            {
                Subject = data.Subject,
                To = data.To,
                From = data.From,
                Mail_Number = data.Mail_Number,
                EventType = data.EventType,
                EventDate = data.EventDate,
                Channel = data.Channel,
                MessageCategory = data.MessageCategory
            });
        }
    }
}
//CODE DUMP
//private IQueryable<tblElasticData> ProjectElasticDataProperties(IQueryable<tblElasticData> dataQuery)
//{
//    return dataQuery.Select(data => new tblElasticData
//    {
//        Subject = data.Subject,
//        To = data.To,
//        From = data.From,
//        EventType = data.EventType,
//        EventDate = data.EventDate,
//        Channel = data.Channel,
//        MessageCategory = data.MessageCategory
//    });
//}



//public List<tblElasticData> GetElasticDataByDate(DateTime? startDate, DateTime? endDate)
//{
//    return ProjectElasticDataProperties(_context.tblElasticData
//        .Where(data => data.EventDate.Date >= startDate && data.EventDate.Date <= endDate))
//        .ToList();
//}


//public List<tblElasticData> GetElasticDataBySubject(string searchSubject)
//{
//    if (searchSubject != null)
//    {
//        searchSubject = searchSubject.Replace(" ", ""); // Remove whitespace from the search subject

//        return ProjectElasticDataProperties(_context.tblElasticData
//            .Where(data => data.Subject != null && data.Subject.Replace(" ", "").Contains(searchSubject)))
//            .ToList();
//    }
//    else
//    {
//        // Handle the case where searchSubject is null (optional)
//        return new List<tblElasticData>();
//    }
//}

//public List<tblElasticData> GetElasticDataByDate(DateTime? startDate, DateTime? endDate)
//{
//    return ProjectElasticDataProperties(_context.tblElasticData
//        .Where(data => data.EventDate.Date >= startDate && data.EventDate.Date <= endDate))
//        .ToList();
//}

//public List<tblElasticData> GetElasticDataByDate(DateTime? startDate, DateTime? endDate)
//{
//    startDate ??= DateTime.MinValue; // If startDate is null, set it to DateTime.MinValue
//    endDate ??= DateTime.MaxValue;   // If endDate is null, set it to DateTime.MaxValue

//    var command = _context.Database.GetDbConnection().CreateCommand();
//    command.CommandTimeout = 0; // 5 min timeout  

//    var result = _context.tblElasticData
//        .Where(data => data.EventDate >= startDate && data.EventDate <= endDate)
//        .GroupBy(data => new
//        {
//            data.To,
//            data.From,
//            data.Subject,
//            data.EventType,
//            data.EventDate,
//            data.Channel,
//            data.MessageCategory
//        })
//        .Select(group => new tblElasticData
//        {
//            To = group.Key.To,
//            From = group.Key.From,
//            Subject = group.Key.Subject,
//            EventType = group.Key.EventType,
//            EventDate = group.Key.EventDate,
//            Channel = group.Key.Channel,
//            MessageCategory = group.Key.MessageCategory,
//            Quantity = group.Count() // Count represents the quantity
//        })
//        .ToList();

//    return result;
//}
//public List<tblElasticData> GetElasticDataByDate(DateTime? startDate, DateTime? endDate)
//{
//    startDate ??= DateTime.MinValue; // If startDate is null, set it to DateTime.MinValue
//    endDate ??= DateTime.MaxValue;   // If endDate is null, set it to DateTime.MaxValue

//    var result = _context.tblElasticData
//        .Where(data => data.EventDate.Date >= startDate.Value.Date && data.EventDate.Date <= endDate.Value.Date)
//        .GroupBy(data => new
//        {
//            data.Mail_Number,
//            data.To,
//            data.From,
//            data.Subject,
//            data.EventType,
//            data.EventDate,
//            data.Channel,
//            data.MessageCategory,
//        })

//        .Select(group => new tblElasticData
//        {
//            Mail_Number = group.Key.Mail_Number,
//            To = group.Key.To,
//            From = group.Key.From,
//            Subject = group.Key.Subject,
//            EventType = group.Key.EventType,
//            EventDate = group.Key.EventDate,
//            Channel = group.Key.Channel,
//            MessageCategory = group.Key.MessageCategory,
//            Quantity = group.Count(), // Count represents the quantity
//        })
//        .ToList();
//    return result;
//}

//private List<CombinedModel> CombineData(List<tblElasticData> elasticData, List<tblEvent> eventDataFromOtherDb)
//{
//    var combinedDataList = new List<CombinedModel>();

//    // Perform a left join operation between tblElasticData and tblEvent
//    var joinedData = from d in elasticData
//                     join e in eventDataFromOtherDb on d.Subject equals e.Mail_Number into events
//                     from evt in events.DefaultIfEmpty()
//                     select new CombinedModel
//                     {
//                         Mail_Number = d.Mail_Number,
//                         Subject = d.Subject ?? evt?.Subject,
//                         To = d.To,
//                         From = d.From,
//                         EventType = d.EventType,
//                         EventDate = d.EventDate,
//                         Channel = d.Channel,
//                         MessageCategory = d.MessageCategory,
//                         Quantity = d.Quantity
//                     };

//    // Filter out rows where both Mail_Number and Subject are null
//    joinedData = joinedData.Where(data => data.Mail_Number != null || data.Subject != null);

//    combinedDataList.AddRange(joinedData);

//    return combinedDataList;
//}
//private List<CombinedModel> CombineData(List<tblElasticData> elasticData, List<tblEvent> eventDataFromOtherDb)
//{
//    var combinedDataList = new List<CombinedModel>();

//    // Create a dictionary for tblEvent data for quick lookup
//    var eventDictionary = eventDataFromOtherDb.ToDictionary(evt => evt.Mail_Number);

//    // Combine data
//    foreach (var elasticItem in elasticData)
//    {
//        var combinedItem = new CombinedModel
//        {
//            Mail_Number = elasticItem.Mail_Number,
//            To = elasticItem.To,
//            From = elasticItem.From,
//            Subject = elasticItem.Subject,
//            Quantity = elasticItem.Quantity,
//            EventType = elasticItem.EventType,
//            EventDate = elasticItem.EventDate,
//            Channel = elasticItem.Channel,
//            MessageCategory = elasticItem.MessageCategory
//        };

//        // Check if Mail_Number is not null before attempting to look up in the dictionary
//        if (elasticItem.Mail_Number != null && eventDictionary.TryGetValue(elasticItem.Mail_Number, out var eventItem))
//        {
//            // Map additional fields from tblEvent
//            combinedItem.Subject = eventItem.Subject;
//            // Add more fields as needed
//        }

//        combinedDataList.Add(combinedItem);
//    }
//    return combinedDataList;
//}

//private List<CombinedModel> CombineData(List<tblElasticData> elasticData, List<tblEvent> eventDataFromOtherDb)
//{
//    var combinedDataList = new List<CombinedModel>();

//    // Create a mapping dictionary from tblEvent
//    var eventMappingDictionary = eventDataFromOtherDb
//        .Where(evt => evt.Mail_Number != null)
//        .ToDictionary(evt => evt.Mail_Number, evt => evt.Subject);

//    // Combine data
//    foreach (var elasticItem in elasticData)
//    {
//        var combinedItem = new CombinedModel
//        {
//            Mail_Number = elasticItem.Mail_Number,
//            To = elasticItem.To,
//            From = elasticItem.From,
//            Subject = elasticItem.Subject,
//            Quantity = elasticItem.Quantity,
//            EventType = elasticItem.EventType,
//            EventDate = elasticItem.EventDate,
//            Channel = elasticItem.Channel,
//            MessageCategory = elasticItem.MessageCategory
//        };

//        // Check if Mail_Number is not null before attempting to map from the dictionary
//        if (elasticItem.Mail_Number != null && eventMappingDictionary.TryGetValue(elasticItem.Mail_Number, out var mappedSubject))
//        {
//            // Map Subject from tblEvent
//            combinedItem.Subject = mappedSubject;
//            // Add more fields as needed
//        }

//        combinedDataList.Add(combinedItem);
//    }

//    return combinedDataList;
//}
//private List<CombinedModel> CombineData(List<tblElasticData> elasticData, List<tblEvent> eventDataFromOtherDb)
//{
//    var combinedDataList = new List<CombinedModel>();

//    // Create a dictionary for tblEvent data for quick lookup
//    var eventDictionary = eventDataFromOtherDb.ToDictionary(evt => evt.Mail_Number);

//    // Combine data
//    foreach (var elasticItem in elasticData)
//    {
//        var combinedItem = new CombinedModel
//        {
//            To = elasticItem.To,
//            From = elasticItem.From,
//            Quantity = elasticItem.Quantity,
//            EventType = elasticItem.EventType,
//            EventDate = elasticItem.EventDate,
//            Channel = elasticItem.Channel,
//            MessageCategory = elasticItem.MessageCategory
//        };

//        // Use Mail_Number from tblEvent as Subject in tblElasticData
//        if (elasticItem.Mail_Number != null && eventDictionary.TryGetValue(elasticItem.Mail_Number, out var eventItem))
//        {
//            // Use Mail_Number as Subject from tblEvent
//            combinedItem.Subject = eventItem.Mail_Number;
//        }
//        else
//        {
//            // Use Subject from tblElasticData if no match found or if Mail_Number is null
//            combinedItem.Subject = elasticItem.Subject;
//        }

//        combinedDataList.Add(combinedItem);
//    }
//    return combinedDataList;
//}