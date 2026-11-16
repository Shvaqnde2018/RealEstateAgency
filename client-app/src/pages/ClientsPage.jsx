import { useState, useEffect } from "react";
import { agencyApi } from "../api/agencyApi";

const CLIENT_TYPE_NAMES = { 1: "Покупець", 2: "Орендар", 3: "Власник" };
const CLIENT_TYPE_COLORS = { 1: "#2563eb", 2: "#d97706", 3: "#16a34a" };

export default function ClientsPage() {
  const [clients, setClients] = useState([]);
  const [clientForm, setClientForm] = useState({ name: "", contactInfo: "", type: 1 });
  
  const [requestModalClient, setRequestModalClient] = useState(null);
  const [requestTitle, setRequestTitle] = useState("");

  const loadClients = async () => {
    try {
      const data = await agencyApi.getClients();
      setClients(data);
    } catch (err) {
      alert(err.message);
    }
  };

  useEffect(() => {
    loadClients();
  }, []);

  const handleCreateClient = async (e) => {
    e.preventDefault();
    try {
      await agencyApi.createClient({
        name: clientForm.name,
        contactInfo: clientForm.contactInfo,
        type: Number(clientForm.type),
      });
      setClientForm({ name: "", contactInfo: "", type: 1 });
      loadClients();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleDeleteClient = async (id) => {
    try {
      await agencyApi.deleteClient(id);
      loadClients();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleCreateRequest = async (e) => {
    e.preventDefault();
    if (!requestModalClient) return;
    try {
      await agencyApi.createClientRequest(requestModalClient.id, {
        title: requestTitle,
        filter: { type: "", district: "", minPrice: null, maxPrice: null }
      });
      alert("Запит успішно створено!");
      setRequestTitle("");
      setRequestModalClient(null);
      loadClients();
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <div className="main-grid">
      <div className="panel">
        <h3>Додати клієнта</h3>
        <form onSubmit={handleCreateClient} className="form-stack">
          <input
            placeholder="Ім'я клієнта"
            value={clientForm.name}
            onChange={(e) => setClientForm({ ...clientForm, name: e.target.value })}
            required
          />
          <input
            placeholder="Контакти (телефон, email)"
            value={clientForm.contactInfo}
            onChange={(e) => setClientForm({ ...clientForm, contactInfo: e.target.value })}
            required
          />
          <select
            value={clientForm.type}
            onChange={(e) => setClientForm({ ...clientForm, type: e.target.value })}
          >
            <option value="1">Роль: Покупець</option>
            <option value="2">Роль: Орендар</option>
            <option value="3">Роль: Власник</option>
          </select>
          <button type="submit" className="btn btn-primary">+ Додати клієнта</button>
        </form>
      </div>

      <div>
        <h3 style={{ marginTop: 0 }}>Клієнти агентства ({clients.length})</h3>
        <div className="card-list">
          {clients.map((c) => (
            <div key={c.id} className="item-card" style={{ flexDirection: "column", alignItems: "stretch" }}>
              <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
                <div>
                  <span
                    className="status-badge"
                    style={{ backgroundColor: CLIENT_TYPE_COLORS[c.type] || "#64748b", marginRight: 8, padding: "3px 8px", borderRadius: 4, color: "#fff", fontSize: 12 }}
                  >
                    {CLIENT_TYPE_NAMES[c.type] || "Клієнт"}
                  </span>
                  <strong style={{ fontSize: 18 }}>{c.name}</strong>
                  <span className="text-muted" style={{ marginLeft: 10 }}>({c.contactInfo})</span>
                </div>
                <div style={{ display: "flex", gap: 6 }}>
                  <button onClick={() => setRequestModalClient(c)} className="btn btn-secondary" style={{ padding: "4px 8px" }}>
                    + Запит
                  </button>
                  <button onClick={() => handleDeleteClient(c.id)} className="btn btn-danger" style={{ padding: "4px 8px" }}>
                    Видалити
                  </button>
                </div>
              </div>

              {c.savedSearches?.length > 0 && (
                <div style={{ marginTop: 10, fontSize: 13, background: "#f8fafc", padding: 8, borderRadius: 6 }}>
                  <b>Збережені пошуки ({c.savedSearches.length}):</b>
                  <ul style={{ margin: "4px 0 0 16px", padding: 0 }}>
                    {c.savedSearches.map((s, idx) => (
                      <li key={idx}><i>{s.title}</i></li>
                    ))}
                  </ul>
                </div>
              )}

              {c.requests?.length > 0 && (
                <div style={{ marginTop: 8, fontSize: 13, background: "#f0fdf4", padding: 8, borderRadius: 6, border: "1px solid #bbf7d0" }}>
                  <b>Запити клієнта ({c.requests.length}):</b>
                  <ul style={{ margin: "4px 0 0 16px", padding: 0 }}>
                    {c.requests.map((r, idx) => (
                      <li key={idx}>
                        <b>{r.title}</b> <span className="text-muted" style={{ fontSize: 11 }}>(від {new Date(r.createdAt).toLocaleDateString()})</span>
                      </li>
                    ))}
                  </ul>
                </div>
              )}
            </div>
          ))}
        </div>
      </div>

      {requestModalClient && (
        <div className="modal-backdrop" style={{ position: "fixed", top: 0, left: 0, width: "100%", height: "100%", background: "rgba(0,0,0,0.5)", display: "flex", justifyContent: "center", alignItems: "center" }}>
          <div className="modal-content panel" style={{ maxWidth: 400, background: "#fff", padding: 20, borderRadius: 8 }}>
            <h3>Створити запит для: {requestModalClient.name}</h3>
            <form onSubmit={handleCreateRequest} className="form-stack" style={{ display: "flex", flexDirection: "column", gap: 10, marginTop: 15 }}>
              <input
                placeholder="Назва запиту (напр. 2-кімнатна до $50k)"
                value={requestTitle}
                onChange={(e) => setRequestTitle(e.target.value)}
                required
                style={{ padding: 8 }}
              />
              <div style={{ display: "flex", gap: 10, justifyContent: "flex-end", marginTop: 10 }}>
                <button type="button" onClick={() => setRequestModalClient(null)} className="btn btn-secondary" style={{ padding: "6px 12px" }}>
                  Скасувати
                </button>
                <button type="submit" className="btn btn-success" style={{ padding: "6px 12px" }}>Зберегти</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}