using System;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class Agent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public int ExperienceYears { get; set; }

        public Agent() { }

        public Agent(string name, int experienceYears)
        {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Ім'я рієлтора не може бути порожнім.");
            if(experienceYears < 0)
                throw new ArgumentException("Стаж рієлтора не може бути від'ємним.");

            Name = name;
            ExperienceYears = experienceYears;
        }

        public override string ToString()
        {
            return $"[{Id.ToString().Substring(0, 8)}] Рієлтор: {Name} | Досвід: {ExperienceYears} р.";
        }
    }
}