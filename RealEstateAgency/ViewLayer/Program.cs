using System;
using System.Text;
using RealEstateAgency.BusinessLogicLayer;
using RealEstateAgency.PersistenceLayer;
using RealEstateAgency.PersistenceLayer.Entities;

namespace RealEstateAgency.ViewLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            var dataManager = new JsonDataManager("agency_data.json");
            var service = new AgencyService(dataManager);

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("\n================ РІЄЛТОРСЬКЕ АГЕНТСТВО (ЕТАП 2) ================");
                Console.WriteLine("1. Показати всі об'єкти нерухомості");
                Console.WriteLine("2. Додати об'єкт нерухомості");
                Console.WriteLine("3. Редагувати об'єкт нерухомості");
                Console.WriteLine("4. Видалити об'єкт нерухомості");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("5. Показати всіх клієнтів");
                Console.WriteLine("6. Додати клієнта");
                Console.WriteLine("7. Редагувати клієнта");
                Console.WriteLine("8. Видалити клієнта");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("9. Керування рієлторами (Список / Додати / Видалити)");
                Console.WriteLine("10. Створити нову угоду (з розрахунком комісії)");
                Console.WriteLine("11. Розірвати угоду (скасувати договір)");
                Console.WriteLine("12. Показати всі угоди");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("13. Змінити статус об'єкта (На продаж -> Заброньовано -> Продано)");
                Console.WriteLine("14. Показати історію змін статусів об'єкта");
                Console.WriteLine("15. Пошук нерухомості за комбінованими фільтрами");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";

                try
                {
                    switch (choice)
                    {
                        case "1": ShowProperties(service); break;
                        case "2": AddPropertyUI(service); break;
                        case "3": UpdatePropertyUI(service); break;
                        case "4": DeletePropertyUI(service); break;
                        case "5": ShowClients(service); break;
                        case "6": AddClientUI(service); break;
                        case "7": UpdateClientUI(service); break;
                        case "8": DeleteClientUI(service); break;
                        case "9": ManageAgentsUI(service); break;
                        case "10": CreateDealUI(service); break;
                        case "11": CancelDealUI(service); break;
                        case "12": ShowDeals(service); break;
                        case "13": ChangePropertyStatusUI(service); break;
                        case "14": ShowPropertyHistoryUI(service); break;
                        case "15": SearchPropertiesUI(service); break;
                        case "0":
                            exit = true;
                            Console.WriteLine("Роботу завершено. Дані збережено в agency_data.json.");
                            break;
                        default:
                            Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                            break;
                    }
                }

                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"\n[Помилка]: {ex.Message}");
                    Console.ResetColor();
                }

            }
        }

    static void ShowProperties(AgencyService service)
        {
            var list = service.GetAllProperties();
            Console.WriteLine("\n--- СПИСОК НЕРУХОМОСТІ ---");
            if (list.Count == 0)
            {
                Console.WriteLine("Список нерухомості порожній.");
                return;
            }
            foreach (var property in list)
            {
                Console.WriteLine(property);
            }
        }

        static void AddPropertyUI(AgencyService service)
        {
            Console.Write("Тип (Квартира/Будинок/Офіс): ");
            string type = Console.ReadLine() ?? "";

            Console.Write("Кількість кімнат: ");
            int rooms = int.Parse(Console.ReadLine() ?? "1");

            Console.Write("Площа (м²): ");
            double area = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Район (напр. Центр, Поділ): ");
            string district = Console.ReadLine() ?? "";

            Console.Write("Адреса: ");
            string address = Console.ReadLine() ?? "";

            Console.Write("Орієнтири (напр. біля метро, парк): ");
            string landmark = Console.ReadLine() ?? "";

            Console.Write("Ціна ($): ");
            decimal price = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Статус (1 - На продаж, 2 - Оренда, 3 - Заброньовано, 4 - Продано): ");
            var status = (PropertyStatus)int.Parse(Console.ReadLine() ?? "1");

            service.CreateProperty(type, area, address, district, landmark, rooms, price, status);
            Console.WriteLine("Об'єкт успішно додано!");
        }

        static void UpdatePropertyUI(AgencyService service)
        {
            ShowProperties(service);
            Console.Write("\nВведіть ID об'єкта для редагування (перші символи): ");
            string id = Console.ReadLine() ?? "";

            Console.Write("Новий тип: ");
            string type = Console.ReadLine() ?? "";

            Console.Write("Нова кількість кімнат: ");
            int rooms = int.Parse(Console.ReadLine() ?? "1");

            Console.Write("Нова площа (м²): ");
            double area = double.Parse(Console.ReadLine() ?? "0");

            Console.Write("Новий район: ");
            string district = Console.ReadLine() ?? "";

            Console.Write("Нова адреса: ");
            string address = Console.ReadLine() ?? "";

            Console.Write("Нові орієнтири поруч: ");
            string landmarks = Console.ReadLine() ?? "";

            Console.Write("Нова ціна ($): ");
            decimal price = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Новий статус (1 - На продаж, 2 - Оренда, 3 - Заброньовано, 4 - Продано): ");
            var status = (PropertyStatus)int.Parse(Console.ReadLine() ?? "1");

            service.UpdateProperty(id, type, area, address, district, landmarks, rooms, price, status);
            Console.WriteLine("Об'єкт успішно оновлено!");
        }

        static void ChangePropertyStatusUI(AgencyService service)
        {
            ShowProperties(service);
            Console.Write("\nВведіть ID об'єкта для зміни статусу (перші символи): ");
            string id = Console.ReadLine() ?? "";

            Console.Write("Оберіть новий статус (1 - На продаж, 2 - Оренда, 3 - Заброньовано, 4 - Продано): ");
            var newStatus = (PropertyStatus)int.Parse(Console.ReadLine() ?? "3");

            Console.Write("Коментар до зміни статусу (напр. 'Клієнт вніс завдаток'): ");
            string comment = Console.ReadLine() ?? "Зміна статусу користувачем";

            service.ChangePropertyStatus(id, newStatus, comment);
            Console.WriteLine("Статус об'єкта успішно змінено та записано в історію!");
        }

        static void ShowPropertyHistoryUI(AgencyService service)
        {
            ShowProperties(service);
            Console.Write("\nВведіть ID об'єкта для перегляду історії змін статусів (перші символи): ");
            string id = Console.ReadLine() ?? "";

            var history = service.GetPropertyStatusHistory(id);
            Console.WriteLine($"\n--- ІСТОРІЯ ЗМІН СТАТУСІВ ОБ'ЄКТА [{id}] ---");
            if (history.Count == 0)
            {
                Console.WriteLine("Історія змін статусів порожня.");
                return;
            }
            foreach (var entry in history)
            {
                Console.WriteLine(entry);
            }
        }

        static void SearchPropertiesUI(AgencyService service)
        {
            Console.WriteLine("\n--- ПОШУК НЕРУХОМОСТІ ЗА ФІЛЬТРАМИ ---");
            Console.WriteLine("(Натискайте Enter, щоб пропустити критерій, який не потрібен)");

            var filter = new PropertySearchFilter();

            Console.Write("Тип нерухомості (Квартира/Будинок/Офіс): ");
            string type = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(type)) filter.Type = type;

            Console.Write("Район (напр. Центр): ");
            string district = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(district)) filter.District = district;

            Console.Write("Орієнтир поруч (напр. метро, школа, парк): ");
            string landmark = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(landmark)) filter.Landmark = landmark;

            Console.Write("Кількість кімнат: ");
            string roomsInput = Console.ReadLine() ?? "";
            if (int.TryParse(roomsInput, out int rooms)) filter.Rooms = rooms;

            Console.Write("Мінімальна площа (м²): ");
            string minAreaStr = Console.ReadLine() ?? "";
            if (double.TryParse(minAreaStr, out double minArea)) filter.MinArea = minArea;

            Console.Write("Максимальна площа (м²): ");
            string maxAreaStr = Console.ReadLine() ?? "";
            if (double.TryParse(maxAreaStr, out double maxArea)) filter.MaxArea = maxArea;

            Console.Write("Мінімальна ціна ($): ");
            string minPriceStr = Console.ReadLine() ?? "";
            if (decimal.TryParse(minPriceStr, out decimal minPrice)) filter.MinPrice = minPrice;

            Console.Write("Максимальна ціна ($): ");
            string maxPriceStr = Console.ReadLine() ?? "";
            if (decimal.TryParse(maxPriceStr, out decimal maxPrice)) filter.MaxPrice = maxPrice;

            Console.Write("Статус (1 - На продаж, 2 - Оренда, 3 - Заброньовано, 4 - Продано): ");
            string statusStr = Console.ReadLine() ?? "";
            if (int.TryParse(statusStr, out int statusInt) && Enum.IsDefined(typeof(PropertyStatus), statusInt))
                filter.Status = (PropertyStatus)statusInt;

            var results = service.SearchProperties(filter);
            Console.WriteLine($"\n--- ЗНАЙДЕНО ОБ'ЄКТІВ: {results.Count} ---");
            foreach (var p in results)
                Console.WriteLine(p);
        }

            static void DeletePropertyUI(AgencyService service)
        {
            ShowProperties(service);
            Console.Write("\nВведіть ID об'єкта для видалення (перші символи): ");
            string id = Console.ReadLine() ?? "";

            service.DeleteProperty(id);
            Console.WriteLine("Об'єкт успішно видалено!");
        }

        static void ShowClients(AgencyService service)
        {
            var list = service.GetAllClients();
            Console.WriteLine("\n--- СПИСОК КЛІЄНТІВ ---");
            if (list.Count == 0)
            {
                Console.WriteLine("Список клієнтів порожній.");
                return;
            }
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }
        }

        static void AddClientUI(AgencyService service)
        {
            Console.Write("Ім'я клієнта: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Контактні дані (тел/email): ");
            string contactInfo = Console.ReadLine() ?? "";

            Console.Write("Тип клієнта (1 - Покупець, 2 - Власник): ");
            var type = (ClientType)int.Parse(Console.ReadLine() ?? "1");

            service.CreateClient(name, contactInfo, type);
            Console.WriteLine("Клієнт успішно доданий!");
        }

        static void UpdateClientUI(AgencyService service)
        {
            ShowClients(service);
            Console.Write("\nВведіть ID клієнта для редагування (перші символи): ");
            string id = Console.ReadLine() ?? "";

            Console.Write("Нове ім'я клієнта: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Нові контактні дані (тел/email): ");
            string contactInfo = Console.ReadLine() ?? "";

            Console.Write("Новий тип клієнта (1 - Покупець, 2 - Власник): ");
            var type = (ClientType)int.Parse(Console.ReadLine() ?? "1");

            service.UpdateClient(id, name, contactInfo, type);
            Console.WriteLine("Клієнт успішно оновлений!");
        }

        static void DeleteClientUI(AgencyService service)
        {
            ShowClients(service);
            Console.Write("\nВведіть ID клієнта для видалення (перші символи): ");
            string id = Console.ReadLine() ?? "";

            service.DeleteClient(id);
            Console.WriteLine("Клієнт успішно видалений!");
        }

        static void ManageAgentsUI(AgencyService service)
        {
            Console.WriteLine("\n--- СПИСОК РІЄЛТОРІВ ---");
            var agents = service.GetAllAgents();
            if (agents.Count == 0)
            {
                Console.WriteLine("Список рієлторів порожній.");
            }
            else
            {
                foreach (var a in agents)
                    Console.WriteLine(a);
            }

            Console.WriteLine("\nДії з рієлторами: 1 - Додати рієлтора | 2 - Видалити рієлтора | Enter - Назад у меню");
            Console.Write("Ваш вибір: ");
            string subChoice = Console.ReadLine() ?? "";

            if (subChoice == "1")
            {
                Console.Write("Ім'я рієлтора: ");
                string name = Console.ReadLine() ?? "";

                Console.Write("Досвід роботи (років): ");
                int exp = int.Parse(Console.ReadLine() ?? "0");

                service.CreateAgent(name, exp);
                Console.WriteLine("Рієлтора успішно додано!");
            }
            else if (subChoice == "2")
            {
                Console.Write("Введіть ID рієлтора для видалення (перші символи): ");
                string id = Console.ReadLine() ?? "";

                service.DeleteAgent(id);
                Console.WriteLine("Рієлтора успішно видалено!");
            }
        }

        static void CreateDealUI(AgencyService service)
        {
            ShowProperties(service);
            Console.Write("Введіть ID об'єкта (перші символи): ");
            string propId = Console.ReadLine() ?? "";

            Console.WriteLine("\n--- РІЄЛТОРИ ---");

            foreach (var agent in service.GetAllAgents())
                Console.WriteLine(agent);

            Console.Write("Введіть ID рієлтора (перші символи): ");
            string agentId = Console.ReadLine() ?? "";

            ShowClients(service);
            Console.Write("Введіть ID клієнта (перші символи): ");
            string clientId = Console.ReadLine() ?? "";

            Console.Write("Фінальна вартість угоди ($): ");
            decimal cost = decimal.Parse(Console.ReadLine() ?? "0");

            Console.Write("Тип угоди (1 - Купівля, 2 - Оренда): ");
            var type = (DealType)int.Parse(Console.ReadLine() ?? "1");

            Console.Write("Комісійний відсоток (%): ");
            decimal comm = decimal.Parse(Console.ReadLine() ?? "0");

            var deal = service.CreateDeal(propId, clientId, agentId, cost, type, comm);
            Console.WriteLine($"Угоду укладено! Сума комісії агентства: ${deal.CalculateCommissionAmount():F2}");
        }

        static void ShowDeals(AgencyService service)
        {
            var list = service.GetAllDeals();
            Console.WriteLine("\n--- СПИСОК УГОД ---");
            if (list.Count == 0)
            {
                Console.WriteLine("Список угод порожній.");
                return;
            }
            foreach (var deal in list)
            {
                Console.WriteLine(deal);
            }
        }

        static void CancelDealUI(AgencyService service)
        {
            ShowDeals(service);
            Console.Write("\nВведіть ID угоди для скасування (перші символи): ");
            string dealId = Console.ReadLine() ?? "";

            Console.Write("Причина розірвання договору: ");
            string reason = Console.ReadLine() ?? "";
            if(string.IsNullOrWhiteSpace(reason))
                reason = "Розірвання договору за згодою сторін";
            
            service.CancelDeal(dealId, reason);
            Console.WriteLine("Угоду успішно скасовано, а об'єкт повернуто в статус 'На продаж'.");
        }
    }
}


