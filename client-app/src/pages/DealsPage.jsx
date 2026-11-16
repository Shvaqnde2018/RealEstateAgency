import { useState, useEffect } from "react";
import { agencyApi } from "../api/agencyApi";

const INITIAL_DEAL_FORM = {
  propertyId: "",
  clientId: "",
  agentId: "",
  price: "",
  dealType: 1,
  commission: "",
};

export default function DealsPage() {
  const [properties, setProperties] = useState([]);
  const [clients, setClients] = useState([]);
  const [agents, setAgents] = useState([]);
  const [deals, setDeals] = useState([]);

  const [agentForm, setAgentForm] = useState({ name: "", experienceYears: "" });
  const [dealForm, setDealForm] = useState(INITIAL_DEAL_FORM);

  const loadData = async () => {
    try {
      const [p, c, a, d] = await Promise.all([
        agencyApi.getProperties(),
        agencyApi.getClients(),
        agencyApi.getAgents(),
        agencyApi.getDeals(),
      ]);
      setProperties(p);
      setClients(c);
      setAgents(a);
      setDeals(d);
    } catch (err) {
      alert(err.message);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleCreateAgent = async (e) => {
    e.preventDefault();
    try {
      await agencyApi.createAgent({
        name: agentForm.name,
        experienceYears: Number(agentForm.experienceYears),
      });
      setAgentForm({ name: "", experienceYears: "" });
      loadData();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleDeleteAgent = async (id) => {
    try {
      await agencyApi.deleteAgent(id);
      loadData();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleCreateDeal = async (e) => {
    e.preventDefault();
    try {
      await agencyApi.createDeal({
        ...dealForm,
        price: Number(dealForm.price),
        dealType: Number(dealForm.dealType),
        commission: Number(dealForm.commission),
      });
      // Скидаємо форму ПІСЛЯ завершення createDeal:
      setDealForm(INITIAL_DEAL_FORM);
      loadData();
    } catch (err) {
      alert(err.message);
    }
  };

  const handleCancelDeal = async (id) => {
    const reason = prompt("Причина розірвання договору:", "За згодою сторін");
    if (reason === null) return;
    try {
      await agencyApi.cancelDeal(id, reason);
      loadData();
    } catch (err) {
      alert(err.message);
    }
  };

  return (
    <div className="main-grid">
      <div className="card-list">
        <form onSubmit={handleCreateDeal} className="panel form-stack">
          <h3>Укласти нову угоду</h3>
          <select value={dealForm.propertyId} onChange={(e) => setDealForm({ ...dealForm, propertyId: e.target.value })} required>
            <option value="">-- Оберіть об'єкт --</option>
            {properties.filter((p) => p.status !== 4).map((p) => (
              <option key={p.id} value={p.id}>
                {p.type} ({p.address}) - ${p.price}
              </option>
            ))}
          </select>
          <select value={dealForm.clientId} onChange={(e) => setDealForm({ ...dealForm, clientId: e.target.value })} required>
            <option value="">-- Оберіть клієнта --</option>
            {clients.map((c) => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
          <select value={dealForm.agentId} onChange={(e) => setDealForm({ ...dealForm, agentId: e.target.value })} required>
            <option value="">-- Оберіть рієлтора --</option>
            {agents.map((a) => (
              <option key={a.id} value={a.id}>{a.name} ({a.experienceYears} р.)</option>
            ))}
          </select>
          <select value={dealForm.dealType} onChange={(e) => setDealForm({ ...dealForm, dealType: e.target.value })}>
            <option value="1">Тип угоди: Купівля</option>
            <option value="2">Тип угоди: Оренда</option>
          </select>
          <input type="number" placeholder="Сума угоди ($)" value={dealForm.price} onChange={(e) => setDealForm({ ...dealForm, price: e.target.value })} required />
          <input type="number" placeholder="Комісія рієлтора (%)" value={dealForm.commission} onChange={(e) => setDealForm({ ...dealForm, commission: e.target.value })} required />
          <div style={{ fontSize: 13, color: "#16a34a", fontWeight: 600 }}>
            Комісія агентства: ${((Number(dealForm.price || 0) * Number(dealForm.commission || 0)) / 100).toFixed(2)}
          </div>
          <button type="submit" className="btn btn-success">Оформити угоду</button>
        </form>

        <form onSubmit={handleCreateAgent} className="panel form-stack">
          <h3>Рієлтори агентства</h3>
          <input placeholder="Ім'я рієлтора" value={agentForm.name} onChange={(e) => setAgentForm({ ...agentForm, name: e.target.value })} required />
          <input type="number" placeholder="Досвід (років)" value={agentForm.experienceYears} onChange={(e) => setAgentForm({ ...agentForm, experienceYears: e.target.value })} required />
          <button type="submit" className="btn btn-primary">+ Додати рієлтора</button>
          <div style={{ marginTop: 8 }}>
            {agents.map((a) => (
              <div key={a.id} className="stat-row" style={{ alignItems: "center" }}>
                <span>{a.name} ({a.experienceYears} р.)</span>
                <button type="button" onClick={() => handleDeleteAgent(a.id)} className="btn btn-danger" style={{ padding: "4px 8px" }}>
                  ×
                </button>
              </div>
            ))}
          </div>
        </form>
      </div>

      <div>
        <h3 style={{ marginTop: 0 }}>Активні угоди ({deals.length})</h3>
        <div className="card-list">
          {deals.map((d) => (
            <div key={d.id} className="item-card">
              <div>
                <strong>Сума: ${d.totalCost.toLocaleString()}</strong> | Комісія:{" "}
                <b>{d.commissionPercent}% (${((d.totalCost * d.commissionPercent) / 100).toFixed(2)})</b>
                <div className="text-muted">Дата: {new Date(d.date).toLocaleString()}</div>
              </div>
              <button onClick={() => handleCancelDeal(d.id)} className="btn btn-danger">
                Розірвати договір
              </button>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}