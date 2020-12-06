using System.Collections.Generic;

namespace Funda.Models
{
    public class FeedResponse
    {
        public List<ObjectForSale> Objects { get; set; }
        
        public Paging Paging { get; set; }

        public int TotaalAantalObjecten { get; set; }
    }

    public class ObjectForSale
    {
        public int MakelaarId { get; set; }
        public string MakelaarNaam { get; set; }
    }

    public class Paging
    {
        public int AantalPaginas { get; set; }
    }
}
