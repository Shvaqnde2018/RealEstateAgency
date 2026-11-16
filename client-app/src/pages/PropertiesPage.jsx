import { useState, useEffect } from "react";
import { agencyApi } from "../api/agencyApi";
import StatusHistoryModal from "../components/StatusHistoryModal";

const STATUS_NAMES = { 1: "На продаж", 2: "Оренда", 3: "Заброньовано", 4: "Продано" };
const STATUS_COLORS = { 1: "#16a34a", 2: "#2563eb", 3: "#d97706", 4: "#dc2626" };

const INITIAL_PROP_FORM = {
  type: "",
  rooms: "",
  yearBuilt: "",
  area: "",
  district: "",
  address: "",
  landmark: "",
  price: "",
  status: 1,
};

const INITIAL_FILTER = {
  type: "",
  district: "",
  address: "",
  landmark: "",
  rooms: "",
  minYearBuilt: "",
  maxYearBuilt: "",
  minArea: "",
  maxArea: "",
  minPrice: "",
  maxPrice: "",
  status: "",
};

export default function PropertiesPage() {
  const [properties, setProperties] = useState([]);
  const [clients, setClients] = useState([]);
  const [alertBanner, setAlertBanner] = useState([]);
  const [historyModal, setHistoryModal] = useState(null);

  const [propForm, setPropForm] = useState(INITIAL_PROP_FORM);
  const [filter, setFilter] = useState(INITIAL_FILTER);

  // Стан для відстеження режиму редагування
  const [editingId, setEditingId] = useState(null);

  const [saveSearchClient, setSaveSearchClient] = useState("");
  const [saveSearchTitle, setSaveSearchTitle] = useState("");

  const loadData = async () => {
    try {
      const [propsData, clientsData] = await Promise.all([
        agencyApi.getProperties(),
        agencyApi.getClients(),
      ]);
      setProperties(propsData);
      setClients(clientsData);
    } catch (err) {
      alert(err.message);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // Універсальний обробник для створення або оновлення об'єкта
  const handleFormSubmit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        ...propForm,
        rooms: Number(propForm.rooms),
        yearBuilt: Number(propForm.yearBuilt),
        area: Number(propForm.area),
        price: Number(propForm.price),
        status: Number(propForm.status),
      };

      if (editingId) {
        // Режим оновлення (ЕДИТ)
        await agencyApi.updateProperty(editingId, payload);
        alert("Об'єкт успішно оновлено!");
        setEditingId(null);
      } else {
        // Режим створення
        const data = await agencyApi.createProperty(payload);
        if (data.notifications?.length > 0) setAlertBanner(data.notifications);
      }
      
      setPropForm(INITIAL_PROP_FORM);
      loadData();
    } catch (err) {
      alert(err.message);
    }
  };

  // Кнопка початку редагування (заповнює форму даними об'єкта)
  const handleStartEdit = (p) => {
    setEditingId(p.id);
    setPropForm({
      type: p.type,
      rooms: p.rooms,
      yearBuilt: p.yearBuilt,
      area: p.area,
      district: p.district,
      address: p.address,
      landmark: p.landmark,
      price: p.price,
      status: p.status,
    });
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const handleCancelEdit = () => {
    setEditingId(null);
    setPropForm(INITIAL_PROP_FORM);
  };

  const handleChangeStatus = async (id, newStatus) => {
    const comment = prompt("Коментар до зміни статусу:", "Зміна статусу через веб-інтерфейс");
    if (comment === null) return;
    try {
      await agencyApi.changePropertyStatus(id, newStatus, comment);
      loadData();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleDeleteProperty = async (id) => {
    try {
      await agencyApi.deleteProperty(id);
      loadData();
    } catch (err) {
      alert(err.message);
    }
  };

  const buildCleanFilter = () => ({
    type: filter.type || null,
    district: filter.district || null,
    address: filter.address || null,
    landmark: filter.landmark || null,
    rooms: filter.rooms ? Number(filter.rooms) : null,
    minYearBuilt: filter.minYearBuilt ? Number(filter.minYearBuilt) : null,
    maxYearBuilt: filter.maxYearBuilt ? Number(filter.maxYearBuilt) : null,
    minArea: filter.minArea ? Number(filter.minArea) : null,
    maxArea: filter.maxArea ? Number(filter.maxArea) : null,
    minPrice: filter.minPrice ? Number(filter.minPrice) : null,
    maxPrice: filter.maxPrice ? Number(filter.maxPrice) : null,
    status: filter.status ? Number(filter.status) : null,
  });

  const handleSearch = async () => {
    try {
      const results = await agencyApi.searchProperties(buildCleanFilter());
      setProperties(results);
    } catch (err) {
      alert(err.message);
    }
  };

  const handleResetFilter = () => {
    setFilter(INITIAL_FILTER);
    loadData();
  };

  const handleSaveClientSearch = async () => {
    if (!saveSearchClient) return alert("Оберіть клієнта зі списку!");
    try {
      await agencyApi.saveClientSearch(
        saveSearchClient,
        saveSearchTitle || "Збережений фільтр",
        buildCleanFilter()
      );
      alert("Пошук збережено в профілі клієнта!");
      setSaveSearchTitle("");
      setSaveSearchClient("");
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <>
      {alertBanner.length > 0 && (
        <div className="alert-banner">
          <strong>Сповіщення системи про нові об'єкти за підписками клієнтів:</strong>
          <ul>
            {alertBanner.map((msg, idx) => (
              <li key={idx}>{msg}</li>
            ))}
          </ul>
          <button className="btn btn-secondary" onClick={() => setAlertBanner([])}>
            Закрити
          </button>
        </div>
      )}

      <div className="main-grid">
        <div className="panel">
          <h3>{editingId ? "Редагувати об'єкт" : "Додати об'єкт"}</h3>
          <form onSubmit={handleFormSubmit} className="form-stack">
            <input placeholder="Тип (Квартира / Будинок / Офіс)" value={propForm.type} onChange={(e) => setPropForm({ ...propForm, type: e.target.value })} required />
            <input placeholder="Район (напр. Центр, Поділ)" value={propForm.district} onChange={(e) => setPropForm({ ...propForm, district: e.target.value })} required />
            <input placeholder="Адреса (вул. Нова, 10)" value={propForm.address} onChange={(e) => setPropForm({ ...propForm, address: e.target.value })} required />
            <input placeholder="Орієнтири (метро, парк)" value={propForm.landmark} onChange={(e) => setPropForm({ ...propForm, landmark: e.target.value })} required />
            <div className="two-col">
              <input type="number" placeholder="Кімнат" value={propForm.rooms} onChange={(e) => setPropForm({ ...propForm, rooms: e.target.value })} required />
              <input type="number" placeholder="Рік (напр. 2023)" value={propForm.yearBuilt} onChange={(e) => setPropForm({ ...propForm, yearBuilt: e.target.value })} required />
            </div>
            <div className="two-col">
              <input type="number" placeholder="Площа (м²)" value={propForm.area} onChange={(e) => setPropForm({ ...propForm, area: e.target.value })} required />
              <input type="number" placeholder="Ціна ($)" value={propForm.price} onChange={(e) => setPropForm({ ...propForm, price: e.target.value })} required />
            </div>
            <select value={propForm.status} onChange={(e) => setPropForm({ ...propForm, status: e.target.value })}>
              <option value="1">Статус: На продаж</option>
              <option value="2">Статус: Оренда</option>
              <option value="3">Статус: Заброньовано</option>
              <option value="4">Статус: Продано</option>
            </select>

            <div style={{ display: "flex", gap: 10 }}>
              <button type="submit" className="btn btn-success" style={{ flex: 1 }}>
                {editingId ? "Зберегти зміни" : "+ Додати об'єкт"}
              </button>
              {editingId && (
                <button type="button" onClick={handleCancelEdit} className="btn btn-secondary" style={{ padding: "8px 12px" }}>
                  Скасувати
                </button>
              )}
            </div>
          </form>

          <hr style={{ margin: "20px 0", borderColor: "#e2e8f0" }} />
          <h3>Комбінований фільтр</h3>
          <div className="form-stack">
            <input placeholder="Тип (Квартира / Будинок / Офіс)" value={filter.type} onChange={(e) => setFilter({ ...filter, type: e.target.value })} />
            <input placeholder="Район (Центр, Поділ...)" value={filter.district} onChange={(e) => setFilter({ ...filter, district: e.target.value })} />
            <input placeholder="Адреса (вулиця або номер)" value={filter.address} onChange={(e) => setFilter({ ...filter, address: e.target.value })} />
            <input placeholder="Орієнтир поруч (метро, парк...)" value={filter.landmark} onChange={(e) => setFilter({ ...filter, landmark: e.target.value })} />
            <div className="two-col">
              <input type="number" placeholder="Кімнат" value={filter.rooms} onChange={(e) => setFilter({ ...filter, rooms: e.target.value })} />
              <select value={filter.status} onChange={(e) => setFilter({ ...filter, status: e.target.value })}>
                <option value="">Всі статуси</option>
                <option value="1">На продаж</option>
                <option value="2">Оренда</option>
                <option value="3">Заброньовано</option>
                <option value="4">Продано</option>
              </select>
            </div>
            <div className="two-col">
              <input type="number" placeholder="Площа від (м²)" value={filter.minArea} onChange={(e) => setFilter({ ...filter, minArea: e.target.value })} />
              <input type="number" placeholder="Площа до (м²)" value={filter.maxArea} onChange={(e) => setFilter({ ...filter, maxArea: e.target.value })} />
            </div>
            <div className="two-col">
              <input type="number" placeholder="Ціна від ($)" value={filter.minPrice} onChange={(e) => setFilter({ ...filter, minPrice: e.target.value })} />
              <input type="number" placeholder="Ціна до ($)" value={filter.maxPrice} onChange={(e) => setFilter({ ...filter, maxPrice: e.target.value })} />
            </div>
            <div className="two-col">
              <input type="number" placeholder="Рік від" value={filter.minYearBuilt} onChange={(e) => setFilter({ ...filter, minYearBuilt: e.target.value })} />
              <input type="number" placeholder="Рік до" value={filter.maxYearBuilt} onChange={(e) => setFilter({ ...filter, maxYearBuilt: e.target.value })} />
            </div>
            <div className="two-col">
              <button onClick={handleSearch} className="btn btn-primary">Знайти</button>
              <button onClick={handleResetFilter} className="btn btn-secondary">Скинути</button>
            </div>

            <div className="sub-box">
              <strong>Зберегти пошук клієнту:</strong>
              <div className="form-stack" style={{ marginTop: 8 }}>
                <select value={saveSearchClient} onChange={(e) => setSaveSearchClient(e.target.value)}>
                  <option value="">-- Оберіть клієнта --</option>
                  {clients.map((c) => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
                <input placeholder="Назва підписки" value={saveSearchTitle} onChange={(e) => setSaveSearchTitle(e.target.value)} />
                <button onClick={handleSaveClientSearch} className="btn btn-dark">Зберегти пошук</button>
              </div>
            </div>
          </div>
        </div>

        <div>
          <h3 style={{ marginTop: 0 }}>Каталог об'єктів ({properties.length})</h3>
          <div className="card-list">
            {properties.map((p) => (
              <div key={p.id} className="item-card" style={{ flexDirection: "column", alignItems: "stretch" }}>
                <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center", flexWrap: "wrap", gap: 10 }}>
                  <div style={{ display: "flex", alignItems: "center", gap: 10, flexWrap: "wrap" }}>
                    <span className="status-badge" style={{ backgroundColor: STATUS_COLORS[p.status] }}>
                      {STATUS_NAMES[p.status]}
                    </span>
                    <strong style={{ fontSize: 18 }}>
                      {p.type} ({p.rooms} кімн., {p.area} м², {p.yearBuilt} р.)
                    </strong>
                    <span className="price-tag">${p.price.toLocaleString()}</span>
                    <span className="text-muted">(${Math.round(p.price / p.area)}/м²)</span>
                  </div>
                  <div style={{ display: "flex", gap: 8, alignItems: "center" }}>
                    <select value={p.status} onChange={(e) => handleChangeStatus(p.id, e.target.value)} style={{ width: 130 }}>
                      <option value="1">На продаж</option>
                      <option value="2">Оренда</option>
                      <option value="3">Заброньовано</option>
                      <option value="4">Продано</option>
                    </select>
                    {/* Кнопка редагування об'єкта */}
                    <button onClick={() => handleStartEdit(p)} className="btn btn-primary" style={{ padding: "4px 8px", fontSize: 13 }}>
                      Редагувати
                    </button>
                    <button onClick={() => setHistoryModal(p)} className="btn btn-secondary">
                      Історія ({p.statusHistory?.length || 0})
                    </button>
                    <button onClick={() => handleDeleteProperty(p.id)} className="btn btn-danger">
                      Видалити
                    </button>
                  </div>
                </div>

                <div className="text-muted" style={{ marginTop: 6 }}>
                  Район: <b>{p.district}</b> | Адреса: <b>{p.address}</b> | Орієнтири: <i>{p.landmark}</i>
                </div>

                {/* Блок динаміки цін */}
                {p.priceHistory && p.priceHistory.length > 0 && (
                  <div style={{ marginTop: 8, fontSize: 12, background: "#f8fafc", padding: "6px 10px", borderRadius: 6, border: "1px solid #e2e8f0" }}>
                    <b>Динаміка цін:</b>
                    <ul style={{ margin: "2px 0 0 16px", padding: 0 }}>
                      {p.priceHistory.map((ph, idx) => (
                        <li key={idx}>
                          ${ph.oldPrice.toLocaleString()} ➔ <b style={{ color: "#16a34a" }}>${ph.newPrice.toLocaleString()}</b> 
                          <span className="text-muted" style={{ fontSize: 11, marginLeft: 6 }}>
                            ({new Date(ph.changedAt).toLocaleDateString()}) — <i>{ph.comment}</i>
                          </span>
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </div>

      <StatusHistoryModal property={historyModal} onClose={() => setHistoryModal(null)} />
    </>
  );
}