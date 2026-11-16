using System;
using System.Text;
using System.Globalization;
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
                Console.WriteLine("\n================ РІЄЛТОРСЬКЕ АГЕНТСТВО (ЕТАП 3) ================");
                Console.WriteLine("1. Показати всі об'єкти нерухомості");
                Console.WriteLine("2. Додати об'єкт нерухомості (з автоперевіркою підписок клієнтів)");
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
                Console.WriteLine("11. Показати всі угоди");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("12. Змінити статус об'єкта (На продаж -> Заброньовано -> Продано)");
                Console.WriteLine("13. Показати історію змін статусів об'єкта");
                Console.WriteLine("14. Пошук нерухомості за фільтрами (+ збереження пошуку клієнта)");
                Console.WriteLine("15. Розірвати угоду (скасувати договір)");
                Console.WriteLine("----------------------------------------------------------------");
                Console.WriteLine("16. Пошуки клієнта (Переглянути збережені до 10 шт. / Сповіщення)");
                Console.WriteLine("17. Статистика та аналітика агентства (Етап 3)");
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
                        case "11": ShowDeals(service); break;
                        case "12": ChangePropertyStatusUI(service); break;
                        case "13": ShowPropertyHistoryUI(service); break;
                        case "14": SearchPropertiesUI(service); break;
                        case "15": CancelDealUI(service); break;
                        case "16": ManageClientSearchesUI(service); break;
                        case "17": ShowStatisticsAndAnalyticsUI(service); break;
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

            Console.Write("Рік спорудження (напр. 2015): ");
            string yearStr = Console.ReadLine() ?? "";
            int yearBuilt = int.TryParse(yearStr, out int y) ? y : 2020;

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

            service.CreateProperty(type, area, address, district, landmark, rooms, price, status, yearBuilt, out var notifications);
            Console.WriteLine("Об'єкт успішно додано!");

            if(notifications.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nЗнайдено збіги зі збереженими пошуками клієнтів ({notifications.Count}):");

                foreach (var note in notifications)
                    Console.WriteLine(" -> " + note);
                Console.ResetColor();
            }
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

            Console.Write("Новий рік спорудження: ");
            string yearStr = Console.ReadLine() ?? "";
            int yearBuilt = int.TryParse(yearStr, out int y) ? y : 2020;

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

            service.UpdateProperty(id, type, area, address, district, landmarks, rooms, price, status, yearBuilt);
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
            string roomInput = Console.ReadLine() ?? "";
            if (int.TryParse(roomInput, out int rooms)) filter.Rooms = rooms;

            Console.Write("Рік спорудження ВІД (напр. 2010): ");
            string minYearStr = Console.ReadLine() ?? "";
            if (int.TryParse(minYearStr, out int minYear)) filter.MinYearBuilt = minYear;

            Console.Write("Рік спорудження ДО: ");
            string maxYearStr = Console.ReadLine() ?? "";
            if (int.TryParse(maxYearStr, out int maxYear)) filter.MaxYearBuilt = maxYear;

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

            Console.Write("\nБажаєте зберегти цей пошук для клієнта? (y/n): ");
            if(Console.ReadLine()?.Trim().ToLower() == "y")
            {
                ShowClients(service);
                Console.Write("Введіть ID клієнта (перші символи): ");
                string clientId = Console.ReadLine() ?? "";

                Console.Write("Введіть назву пошуку (напр. 'Рік від 2010 з площею до 70 м2'): ");
                string title = Console.ReadLine() ?? "";

                service.SaveClientSearch(clientId, title, filter);
                Console.WriteLine("Пошук успішно збережено у профілі клієнта (зберігається до 10 останніх пошуків)!");
            }
        }

        static void ManageClientSearchesUI(AgencyService service)
        {
            ShowClients(service);
            Console.Write("\nВведіть ID клієнта для перегляду його пошуків та сповіщень (перші символи): ");
            string clientId = Console.ReadLine() ?? "";

            var client = service.GetClientById(clientId);
            Console.WriteLine($"\n=== ПРОФІЛЬ ПОШУКІВ КЛІЄНТА: {client.Name} ===");

            var searches = service.GetClientSavedSearches(clientId);
            Console.WriteLine($"\n--- ЗБЕРЕЖЕНІ ПОШУКИ (останні {searches.Count} з 10) ---");
            if(searches.Count == 0)
            {
                Console.WriteLine("У клієнта ще немає збережених пошуків.");
            }
            else
            {
                for(int i = 0; i < searches.Count; i++)
                    Console.WriteLine($"{i + 1}. {searches[i]}");
            }

            var notifications = service.GetClientNotifications(clientId);
            Console.WriteLine($"\n--- АВТОМАТИЧНІ СПОВІЩЕННЯ ПРО НОВІ ОБ'ЄКТИ ({notifications.Count}) ---");

            if(notifications.Count == 0)
            {
                Console.WriteLine("Нових сповіщень немає.");
            }
            else
            {
                foreach (var note in notifications)
                    Console.WriteLine(" * " + note);
            }

            if(searches.Count > 0)
            {
                Console.Write("\nБажаєте повторно виконати один зі збережених пошуків? Введіть його номер (1..10) або Enter для виходу: ");
                string idxStr = Console.ReadLine() ?? "";

                if(int.TryParse(idxStr, out int idx) && idx >= 1 && idx <= searches.Count)
                {
                    var selectedSearch = searches[idx - 1];
                    var matchedProps = service.SearchProperties(selectedSearch.Filter);
                    Console.WriteLine($"\n--- РЕЗУЛЬТАТИ ЗА ПОШУКОМ \"{selectedSearch.Title}\" (Знайдено: {matchedProps.Count}) ---");

                    foreach (var p in matchedProps)
                        Console.WriteLine(p);
                }
            }
        }

        static void ShowStatisticsAndAnalyticsUI(AgencyService service)
        {
            Console.WriteLine("\n================ СТАТИСТИКА ТА АНАЛІТИКА (ЕТАП 3) ================");
            Console.Write("Введіть початкову дату періоду (дд.мм.рррр) або натисніть Enter (за весь час): ");
            string startStr = Console.ReadLine() ?? "";

            DateTime? startDate = null;
            if (DateTime.TryParseExact(startStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime sDate))
                startDate = sDate;

            Console.Write("Введіть кінцеву дату періоду (дд.мм.рррр) або натисніть Enter (до сьогодні): ");
            string endStr = Console.ReadLine() ?? "";

            DateTime? endDate = null;
            if (DateTime.TryParseExact(endStr, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime eDate))
                endDate = eDate;

            var periodDeals = service.GetDealsByPeriod(startDate, endDate);
            decimal totalSales = service.GetTotalSalesAmount(startDate, endDate);
            decimal totalProfit = service.GetTotalAgencyProfit(startDate, endDate);
            decimal avgSqmCatalog = service.GetAveragePricePerSquareMeterAllProperties();
            decimal avgSqmDeals = service.GetAveragePricePerSquareMeterSoldDeals();

            Console.WriteLine("\n--- 1. ЗАГАЛЬНІ ПОКАЗНИКИ ПРОДАЖІВ ТА ВАРТОСТІ М² ---");
            Console.WriteLine($"Кількість успішних угод за обраний період: {periodDeals.Count}");
            Console.WriteLine($"Загальна сума продажів (обсяг угод):       ${totalSales:F2}");
            Console.WriteLine($"Загальний прибуток агентства (комісійні):  ${totalProfit:F2}");
            Console.WriteLine($"Середня вартість 1 м² (по всіх об'єктах):  ${avgSqmCatalog:F2} / м²");
            Console.WriteLine($"Середня вартість 1 м² (по угодах):         ${avgSqmDeals:F2} / м²");

            Console.WriteLine("\n--- 2. ЕФЕКТИВНІСТЬ РІЄЛТОРІВ ---");
            var agentStats = service.GetAgentsPerformance();

            foreach(var stat in agentStats)
            {
                Console.WriteLine($"Рієлтор: {stat.Agent.Name} (досвід {stat.Agent.ExperienceYears} р.) | Угод: {stat.DealsCount} | Сума угод: ${stat.TotalSalesVolume:F2} | Прибуток (комісія): ${stat.TotalCommissionProfit:F2}");
            }

            Console.WriteLine("\n--- 3. НАЙАКТИВНІШІ КЛІЄНТИ ---");
            var clientStats = service.GetMostActiveClients();

            if (clientStats.Count == 0)
            {
                Console.WriteLine("Клієнти відсутні.");
            }
            else
            {
                foreach(var cStat in clientStats)
                {
                    Console.WriteLine($"Клієнт: {cStat.Client.Name} ({cStat.Client.Type}) | Укладено угод: {cStat.DealsCount} | На суму: ${cStat.TotalSpent:F2} | Збережених пошуків: {cStat.SavedSearchesCount}");
                }
            }
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


