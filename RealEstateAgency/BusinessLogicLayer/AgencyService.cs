using System;
using System.Linq;
using System.Collections.Generic;
using RealEstateAgency.PersistenceLayer;
using RealEstateAgency.PersistenceLayer.Entities;
using System.Numerics;

namespace RealEstateAgency.BusinessLogicLayer
{
    public class PriceDynamicsStat
    {
        public string Period { get; set; } = string.Empty;
        public decimal AveragePrice { get; set; }
        public int DealsCount { get; set; }
    }

    public class AgentPerformanceStat
    {
        public Agent Agent { get; set; } = null!;
        public int DealsCount { get; set; }
        public decimal TotalSalesVolume { get; set; }
        public decimal TotalCommissionProfit { get; set; }
    }

    public class ClientActivityStat
    {
        public Client Client { get; set; } = null!;
        public int DealsCount { get; set; }
        public decimal TotalSpent { get; set; }
        public int SavedSearchesCount { get; set; }
    }

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

        public Property CreateProperty(string type, double area, string address, string district, string landmark, int rooms, decimal price, PropertyStatus status, int yearBuilt, out List<string> generatedNotifications)
        {
            var property = new Property(type, area, address, district, landmark, rooms, price, status, yearBuilt);
            _context.Properties.Add(property);

            generatedNotifications = NotifyMatchingClients(property);

            SaveChanges();
            return property;
        }

        private List<string> NotifyMatchingClients(Property newProperty)
        {
            var alerts = new List<string>();
            foreach (var client in _context.Clients)
            {
                foreach (var savedSearch in client.SavedSearches)
                {
                    if (savedSearch.Filter.Matches(newProperty))
                    {
                        string msg = $"[{DateTime.Now:dd.MM.yyyy HH:mm}] Клієнту {client.Name} ({client.ContactInfo}): з'явився новий об'єкт за вашим пошуком \"{savedSearch.Title}\" -> {newProperty.Type}, {newProperty.Area} м², {newProperty.YearBuilt} р., р-н {newProperty.District}, ${newProperty.Price}";
                        client.Notifications.Insert(0, msg);
                        alerts.Add(msg);
                        break;
                    }
                }
            }
            return alerts;
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

        public void UpdateProperty(string id, string type, double area, string address, string district, string landmark, int rooms, decimal price, PropertyStatus status, int yearBuilt)
        {
            var prop = GetPropertyById(id);
            prop.Update(type, area, address, district, landmark, rooms, price, status, yearBuilt);
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

        //ПОШУК ТА ЗБЕРЕЖЕННЯ ПОШУКІВ КЛІЄНТІВ

        public List<Property> SearchProperties(PropertySearchFilter filter)
        {
            return _context.Properties.Where(p => filter.Matches(p)).ToList();
        }

        public void SaveClientSearch(string clientId, string searchTitle, PropertySearchFilter filter)
        {
            var client = GetClientById(clientId);
            client.AddSavedSearch(searchTitle, filter);
            SaveChanges();
        }

        public List<SavedClientSearch> GetClientSavedSearches(string clientId)
        {
            var client = GetClientById(clientId);
            return client.SavedSearches;
        }

        public List<string> GetClientNotifications(string clientId)
        {
            var client = GetClientById(clientId);
            return client.Notifications;
        }

        // CRUD ДЛЯ CLIENT

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

        public void DeleteAgent(string id)
        {
            var agent = GetAgentById(id);
            if (_context.Deals.Any(d => d.AgentId == agent.Id))
                throw new InvalidOperationException("Неможливо видалити рієлтора, який пов'язаний з угодою.");

            _context.Agents.Remove(agent);
            SaveChanges();
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

        // СТАТИСТИКА ТА АНАЛІТИКА (ЕТАП 3)

        public List<Deal> GetDealsByPeriod(DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<Deal> query = _context.Deals;

            if (startDate.HasValue)
                query = query.Where(d => d.Date.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                query = query.Where(d => d.Date.Date <= endDate.Value.Date);

            return query.OrderByDescending(d => d.Date).ToList();
        }

        public decimal GetTotalSalesAmount(DateTime? startDate = null, DateTime? endDate = null)
        {
            return GetDealsByPeriod(startDate, endDate).Sum(d => d.TotalCost);
        }

        public decimal GetTotalAgencyProfit(DateTime? startDate = null, DateTime? endDate = null)
        {
            return GetDealsByPeriod(startDate, endDate).Sum(d => d.CalculateCommissionAmount());
        }

        public decimal GetAveragePricePerSquareMeterAllProperties()
        {
            var validProps = _context.Properties.Where(p => p.Area > 0).ToList();
            if (!validProps.Any())
                return 0;

            return validProps.Average(p => p.GetPricePerSquareMeter());
        }

        public decimal GetAveragePricePerSquareMeterSoldDeals()
        {
            if (!_context.Deals.Any())
                return 0m;
            var dealPricesPerSqm = new List<decimal>();

            foreach (var deal in _context.Deals)
            {
                var prop = _context.Properties.FirstOrDefault(p => p.Id == deal.PropertyId);

                if (prop != null && prop.Area > 0)
                {
                    dealPricesPerSqm.Add(deal.TotalCost / (decimal)prop.Area);
                }
            }
            return dealPricesPerSqm.Any() ? dealPricesPerSqm.Average() : 0m;
        }

        public List<AgentPerformanceStat> GetAgentsPerformance()
        {
            return _context.Agents.Select(agent =>
            {
                var agentDeals = _context.Deals.Where(d => d.AgentId == agent.Id).ToList();
                return new AgentPerformanceStat
                {
                    Agent = agent,
                    DealsCount = agentDeals.Count,
                    TotalSalesVolume = agentDeals.Sum(d => d.TotalCost),
                    TotalCommissionProfit = agentDeals.Sum(d => d.CalculateCommissionAmount())
                };
            })
            .OrderByDescending(s => s.TotalCommissionProfit)
            .ThenByDescending(s => s.DealsCount)
            .ToList();
        }

        public List<ClientActivityStat> GetMostActiveClients()
        {
            return _context.Clients.Select(client =>
            {
                var clientDeals = _context.Deals.Where(d => d.ClientId == client.Id).ToList();
                return new ClientActivityStat
                {
                    Client = client,
                    DealsCount = clientDeals.Count,
                    TotalSpent = clientDeals.Sum(d => d.TotalCost),
                    SavedSearchesCount = client.SavedSearches.Count
                };
            })
            .OrderByDescending(c => c.DealsCount)
            .ThenByDescending(c => c.TotalSpent)
            .ThenByDescending(c => c.SavedSearchesCount)
            .ToList();
        }

        public ClientRequest CreateClientRequest(Guid clientId, string title, PropertySearchFilter filter)
        {
            var client = GetClientById(clientId.ToString()) ?? throw new Exception("Клієнта не знайдено.");

            var request = new ClientRequest(clientId, title, filter);
            _context.Requests.Add(request);
            SaveChanges();
            return request;
        }

        public List<Property> MatchPropertiesForRequest(Guid requestId)
        {
            var request = _context.Requests.FirstOrDefault(r => r.Id == requestId)
                ?? throw new Exception("Запит не знайдено.");

            return _context.Properties.Where(p => request.Filter.Matches(p)).ToList();
        }

        public List<Property> MatchPropertiesForClientSavedSearches(Guid clientId)
        {
            var client = GetClientById(clientId.ToString()) ?? throw new Exception("Клієнта не знайдено.");
            var matchedProperties = new HashSet<Property>();

            foreach (var search in client.SavedSearches)
            {
                var found = _context.Properties.Where(p => search.Filter.Matches(p));
                foreach (var prop in found)
                {
                    matchedProperties.Add(prop);
                }
            }

            return matchedProperties.ToList();
        }

        public decimal GetTotalAgencyIncome()
        {
            return _context.Deals.Sum(d => d.TotalCost * (d.CommissionPercent / 100m));
        }

        public List<PriceDynamicsStat> GetPriceDynamics()
        {
            return _context.Deals
                .GroupBy(d => new { d.Date.Year, d.Date.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new PriceDynamicsStat
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    AveragePrice = g.Average(d => d.TotalCost),
                    DealsCount = g.Count()
                })
                .ToList();
        }

        public int GetTotalRequestsCount()
        {
            return _context.Requests?.Count ?? 0;
        }
    }
}