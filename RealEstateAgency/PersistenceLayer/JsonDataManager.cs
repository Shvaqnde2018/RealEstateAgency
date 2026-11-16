using RealEstateAgency.PersistenceLayer.Entities;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace RealEstateAgency.PersistenceLayer
{
    public class JsonDataManager
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _options;


        public JsonDataManager(string filePath = "agency_data.json")
        {
            _filePath = filePath;
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic)
            };
        }

        public void SaveData(AgencyDataContext context)
        {
            string jsonData = System.Text.Json.JsonSerializer.Serialize(context, _options);
            File.WriteAllText(_filePath, jsonData);
        }

        public AgencyDataContext LoadData()
        {
            if (!File.Exists(_filePath))
                return new AgencyDataContext();

            string jsonData = File.ReadAllText(_filePath);
            if(string.IsNullOrWhiteSpace(jsonData))
                return new AgencyDataContext();

            return JsonSerializer.Deserialize<AgencyDataContext>(jsonData, _options) ?? new AgencyDataContext();
        }
    }
}