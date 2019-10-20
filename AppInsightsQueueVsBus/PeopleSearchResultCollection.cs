using System;
using System.Collections.Generic;
using System.Text;

namespace AppInsightsQueueVsBus
{
    class PeopleSearchResultCollection
    {
        public int count { get; set; }
        public object next { get; set; }
        public object previous { get; set; }
        public IEnumerable<Person> results { get; set; }
    }

}
