using automationTest.Context;
using automationTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace automationTest.Service
{
    public class EventDataService
    {
        private readonly ApplicationDbContext_ _context;

        public EventDataService(ApplicationDbContext_ context)
        {
            _context = context;
        }
        public List<tblEvent> GetMailNumber(string searchMailNumber)
        {
            return _context.tblEvent
            .Where(data => data.Mail_Number == searchMailNumber)
            .Select(data => new tblEvent
            {
                Subject = data.Subject
            })
            .ToList();
        }
    }
}

//mailnumber
