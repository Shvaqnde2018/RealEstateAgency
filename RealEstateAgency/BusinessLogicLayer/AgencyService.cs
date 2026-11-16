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
        private  AgencyDataContext _context;
        public AgencyService(JsonDataManager dataManager)
        {
            _dataManager = dataManager;
            _context = _dataManager.LoadData();

            if(!_context.Agents.Any())
            {
                _context.Agents.Add(new Agent("Іван Іваненко", 5));
                _dataManager.SaveData(_context);
            }
        }

        public void SaveChanges() =>_dataManager.SaveData(_context);
        public void ReloadData() => _context = _dataManager.LoadData();

        //CRUD Property

        public Property CreateProperty(string type, double area, string address, decimal price, PropertyStatus status)
        {
            var property = new Property(type, area, address, price, status);
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

        public void UpdateProperty(string id, string type, double area, string address, decimal price, PropertyStatus status)
        {
            var prop = GetPropertyById(id);
            prop.Update(type, area, address, price, status);
            SaveChanges();
        }

        public void DeleteProperty(string id)
        {
            var prop = GetPropertyById(id);
            if(_context.Deals.Any(d => d.PropertyId == prop.Id))
                throw new InvalidOperationException("Неможливо видалити нерухомість, яка пов'язана з угодою.");

            _context.Properties.Remove(prop);
            SaveChanges();
        }

        //CRUD Client

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
            if(_context.Deals.Any(d => d.ClientId == client.Id))
                throw new InvalidOperationException("Неможливо видалити клієнта, який пов'язаний з угодою.");

            _context.Clients.Remove(client);
            SaveChanges();
        }

        //Операції з Agent, Deal

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

        public Deal CreateDeal(string propertyId, string clientId, string agentId, decimal price, DealType dealType, decimal commission)
        {
            var property = GetPropertyById(propertyId);
            var client = GetClientById(clientId);
            var agent = GetAgentById(agentId);

            if(property.Status == PropertyStatus.Sold)
                throw new InvalidOperationException("Цей об'єкт вже продано! Угоду створити неможливо.");

            var deal = new Deal(property.Id, agent.Id, client.Id, price, dealType, commission);
            property.Status = PropertyStatus.Sold;

            _context.Deals.Add(deal);
            SaveChanges();
            return deal;
        }

        public List<Deal> GetAllDeals() => _context.Deals;
    }
}