using System.IO;
using System.Text.Json;
using System.Text.Unicode;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using RealEstateAgency.PersistenceLayer.Entities;


namespace RealEstateAgency.PersistenceLayer
{
    public class AgencyDataContext
    {
        public List<Property> Properties { get; set; } = new List<Property>();
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<Agent> Agents { get; set; } = new List<Agent>();
        public List<Deal> Deals { get; set; } = new List<Deal>();

        public List<ClientRequest> Requests { get; set; } = new List<ClientRequest>();
    }

}