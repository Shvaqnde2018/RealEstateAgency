using System;
using System.Linq;
using System.Collections.Generic;
using RealEstateAgency.PersistenceLayer;
using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.BusinessLogicLayer
{
    public class AgencyService
    {
        private readonly JsonDataManager _dataManager;
        private AgencyDataContext _context;

        public AgencyService(JsonDataManager dataManager)
        {
            _dataManager = dataManager;
            _context = _dataManager.LoadData();

            if (!_context.Agents.Any())
            {
                _context.Agents.Add(new Agent("Іван Іваненко", 5));
                _dataManager.SaveData(_context);
            }
        }

        public void SaveChanges() => _dataManager.SaveData(_context);
        public void ReloadData() => _context = _dataManager.LoadData();

        // CRUD Property

        public Property CreateProperty(string type, double area, string address, string district, string landmark, int rooms, decimal price, PropertyStatus status)
        {
            var property = new Property(type, area, address, district, landmark, rooms, price, status);
            _context.Properties.Add(property);
            SaveChanges();
            return property;
        }

        public List<Property> GetAllProperties() => _context.Properties;

        public Property GetPropertyById(string shortOrFullId)
        {
            if (string.IsNullOrWhiteSpace(shortOrFullId))
                throw new ArgumentException("ID нерухомості не може бути порожнім.");

            var prop = _context.Properties.FirstOrDefault(p => p.Id.ToString().StartsWith(shortOrFullId, StringComparison.OrdinalIgnoreCase));
            if (prop == null)
                throw new ArgumentException($"Нерухомість з ідентифікатором '{shortOrFullId}' не знайдена.");
            return prop;
        }

        public void UpdateProperty(string id, string type, double area, string address, string district, string landmark, int rooms, decimal price, PropertyStatus status)
        {
            var prop = GetPropertyById(id);
            prop.Update(type, area, address, district, landmark, rooms, price, status);
            SaveChanges();
        }

        public void ChangePropertyStatus(string id, PropertyStatus newStatus, string comment = "Зміна статусу")
        {
            var prop = GetPropertyById(id);
            prop.ChangeStatus(newStatus, comment);
            SaveChanges();
        }

        public List<StatusHistoryEntry> GetPropertyStatusHistory(string id)
        {
            var prop = GetPropertyById(id);
            return prop.StatusHistory;
        }

        public void DeleteProperty(string id)
        {
            var prop = GetPropertyById(id);
            if (_context.Deals.Any(d => d.PropertyId == prop.Id))
                throw new InvalidOperationException("Неможливо видалити нерухомість, яка пов'язана з угодою.");

            _context.Properties.Remove(prop);
            SaveChanges();
        }

        // ПОШУК ТА КОМБІНОВАНА СИСТЕМА ФІЛЬТРІВ (ЕТАП 2)

        public List<Property> SearchProperties(PropertySearchFilter filter)
        {
            IEnumerable<Property> query = _context.Properties;

            if (!string.IsNullOrWhiteSpace(filter.Type))
                query = query.Where(p => p.Type.Contains(filter.Type, StringComparison.OrdinalIgnoreCase));

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.MinArea.HasValue)
                query = query.Where(p => p.Area >= filter.MinArea.Value);

            if (filter.MaxArea.HasValue)
                query = query.Where(p => p.Area <= filter.MaxArea.Value);

            if (!string.IsNullOrWhiteSpace(filter.District))
                query = query.Where(p => p.District.Contains(filter.District, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(filter.Landmark))
                query = query.Where(p => p.Landmark.Contains(filter.Landmark, StringComparison.OrdinalIgnoreCase));

            if (filter.Rooms.HasValue)
                query = query.Where(p => p.Rooms == filter.Rooms.Value);

            if (filter.Status.HasValue)
                query = query.Where(p => p.Status == filter.Status.Value);

            return query.ToList();
        }

        // CRUD Client

        public Client CreateClient(string name, string contactInfo, ClientType type)
        {
            var client = new Client(name, contactInfo, type);
            _context.Clients.Add(client);
            SaveChanges();
            return client;
        }

        public List<Client> GetAllClients() => _context.Clients;

        public Client GetClientById(string shortOrFullId)
        {
            if (string.IsNullOrWhiteSpace(shortOrFullId))
                throw new ArgumentException("ID клієнта не може бути порожнім.");

            var client = _context.Clients.FirstOrDefault(c => c.Id.ToString().StartsWith(shortOrFullId, StringComparison.OrdinalIgnoreCase));
            if (client == null)
                throw new ArgumentException($"Клієнт з ідентифікатором '{shortOrFullId}' не знайдений.");
            return client;
        }

        public void UpdateClient(string id, string name, string contactInfo, ClientType type)
        {
            var client = GetClientById(id);
            client.Update(name, contactInfo, type);
            SaveChanges();
        }

        public void DeleteClient(string id)
        {
            var client = GetClientById(id);
            if (_context.Deals.Any(d => d.ClientId == client.Id))
                throw new InvalidOperationException("Неможливо видалити клієнта, який пов'язаний з угодою.");

            _context.Clients.Remove(client);
            SaveChanges();
        }

        // Операції з Agent, Deal

        public Agent CreateAgent(string name, int experienceYears)
        {
            var agent = new Agent(name, experienceYears);
            _context.Agents.Add(agent);
            SaveChanges();
            return agent;
        }

        public void DeleteAgent(string id)
        {
            var agent = GetAgentById(id);
            if (_context.Deals.Any(d => d.AgentId == agent.Id))
                throw new InvalidOperationException("Неможливо видалити рієлтора, який пов'язаний з угодою.");

            _context.Agents.Remove(agent);
            SaveChanges();
        }

        public List<Agent> GetAllAgents() => _context.Agents;

        public Agent GetAgentById(string shortOrFullId)
        {
            if (string.IsNullOrWhiteSpace(shortOrFullId))
                throw new ArgumentException("ID рієлтора не може бути порожнім.");

            var agent = _context.Agents.FirstOrDefault(a => a.Id.ToString().StartsWith(shortOrFullId, StringComparison.OrdinalIgnoreCase));
            if (agent == null)
                throw new ArgumentException($"Рієлтор з ідентифікатором '{shortOrFullId}' не знайдений.");
            return agent;
        }

        public Deal CreateDeal(string propertyId, string clientId, string agentId, decimal price, DealType dealType, decimal commission)
        {
            var property = GetPropertyById(propertyId);
            var client = GetClientById(clientId);
            var agent = GetAgentById(agentId);

            if (property.Status == PropertyStatus.Sold)
                throw new InvalidOperationException("Цей об'єкт вже продано! Угоду створити неможливо.");

            var deal = new Deal(property.Id, agent.Id, client.Id, price, dealType, commission);
            property.ChangeStatus(PropertyStatus.Sold, $"Укладено угоду [{deal.Id.ToString().Substring(0, 8)}]");

            _context.Deals.Add(deal);
            SaveChanges();
            return deal;
        }

        public List<Deal> GetAllDeals() => _context.Deals;

        public Deal GetDealById(string shortOrFullId)
        {
            if (string.IsNullOrWhiteSpace(shortOrFullId))
                throw new ArgumentException("ID угоди не може бути порожнім.");

            var deal = _context.Deals.FirstOrDefault(d => d.Id.ToString().StartsWith(shortOrFullId, StringComparison.OrdinalIgnoreCase));
            if (deal == null)
                throw new ArgumentException($"Угода з ідентифікатором '{shortOrFullId}' не знайдена.");
            return deal;
        }

        public void CancelDeal(string dealId, string reason = "Розірвання договору за згодою сторін")
        {
            {
                var deal = GetDealById(dealId);
                var property = _context.Properties.FirstOrDefault(p => p.Id == deal.PropertyId);

                if (property != null)
                {
                    var restoredStatus = deal.Type == DealType.Purchase ? PropertyStatus.ForSale : PropertyStatus.ForRent;
                    property.ChangeStatus(restoredStatus, $"Угода [{deal.Id.ToString().Substring(0, 8)}] розірвана: {reason}", allowFromSold: true);
                }

                _context.Deals.Remove(deal);
                SaveChanges();
            }
        }
    }
}