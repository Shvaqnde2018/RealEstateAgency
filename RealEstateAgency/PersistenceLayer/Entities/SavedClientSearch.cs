using System;

namespace RealEstateAgency.PersistenceLayer.Entities
{
    public class SavedClientSearch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Title { get; set; } = string.Empty;
        public PropertySearchFilter Filter { get; set; } = new PropertySearchFilter();

        public SavedClientSearch() { }

        public SavedClientSearch(string title, PropertySearchFilter filter)
        {
            Title = string.IsNullOrWhiteSpace(title) ? "Пошук без назви" : title;
            Filter = filter ?? new PropertySearchFilter();
            CreatedAt = DateTime.Now;
        }

        public override string ToString()
        {
            return $"[{Id.ToString().Substring(0, 8)}] ({CreatedAt:dd.MM.yyyy HH:mm}) Назва: \"{Title}\" | {Filter}";
        }
    }
}