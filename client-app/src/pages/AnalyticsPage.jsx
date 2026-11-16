import { useState, useEffect } from "react";
import { agencyApi } from "../api/agencyApi";

export default function AnalyticsPage() {
  const [analytics, setAnalytics] = useState(null);

  useEffect(() => {
    agencyApi.getAnalytics().then(setAnalytics).catch((e) => alert(e.message));
  }, []);

  if (!analytics) return <p>Завантаження аналітики...</p>;

  return (
    <div>
      <h2>Фінансова та загальна аналітика</h2>
      <div className="stats-grid">
        <div className="panel">
          <span className="text-muted">Успішних угод</span>
          <div className="stat-value">{analytics.dealsCount}</div>
        </div>
        <div className="panel">
          <span className="text-muted">Загальна сума продажів</span>
          <div className="stat-value" style={{ color: "#2563eb" }}>
            ${analytics.totalSales.toLocaleString()}
          </div>
        </div>
        <div className="panel">
          <span className="text-muted">Загальний дохід (комісійні)</span>
          <div className="stat-value" style={{ color: "#16a34a" }}>
            ${analytics.totalProfit.toLocaleString()}
          </div>
        </div>
        <div className="panel">
          <span className="text-muted">Середня ціна м² (каталог / угоди)</span>
          <div className="stat-value" style={{ fontSize: 22 }}>
            ${Math.round(analytics.avgSqmCatalog)} / ${Math.round(analytics.avgSqmDeals)}
          </div>
        </div>
      </div>

      {/* Блок обліку сутностей (будь-які цифри системи) */}
      <div className="panel" style={{ marginTop: 20 }}>
        <h3>Статистика та стан сутностей системи</h3>
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fit, minmax(200px, 1fr))", gap: 15, marginTop: 10 }}>
          <div style={{ background: "#f8fafc", padding: 12, borderRadius: 6 }}>
            <span className="text-muted">Всього об'єктів:</span>
            <div style={{ fontSize: 20, fontWeight: 700 }}>{analytics.totalProperties}</div>
            <div style={{ fontSize: 12, color: "#64748b" }}>На продаж: {analytics.forSaleCount} | В оренді: {analytics.forRentCount} | Продано: {analytics.soldCount}</div>
          </div>
          <div style={{ background: "#f8fafc", padding: 12, borderRadius: 6 }}>
            <span className="text-muted">Всього клієнтів:</span>
            <div style={{ fontSize: 20, fontWeight: 700 }}>{analytics.totalClients}</div>
            <div style={{ fontSize: 12, color: "#64748b" }}>Покупці: {analytics.buyersCount} | Орендарі: {analytics.tenantsCount} | Власники: {analytics.ownersCount}</div>
          </div>
          <div style={{ background: "#f8fafc", padding: 12, borderRadius: 6 }}>
            <span className="text-muted">Активні запити клієнтів:</span>
            <div style={{ fontSize: 20, fontWeight: 700, color: "#2563eb" }}>{analytics.totalRequests}</div>
          </div>
        </div>
      </div>

      {/* Динаміка цін за періодами */}
      <div className="panel" style={{ marginTop: 20 }}>
        <h3>Динаміка середніх цін за угодами (по місяцях)</h3>
        {analytics.priceDynamics.length === 0 ? (
          <p className="text-muted">Недостатньо даних для побудови динаміки (потрібні завершені угоди).</p>
        ) : (
          <table style={{ width: "100%", marginTop: 10, borderCollapse: "collapse" }}>
            <thead>
              <tr style={{ borderBottom: "2px solid #e2e8f0", textAlign: "left" }}>
                <th style={{ padding: 8 }}>Період (Рік-Місяць)</th>
                <th style={{ padding: 8 }}>Кількість угод</th>
                <th style={{ padding: 8 }}>Середня вартість угоди</th>
              </tr>
            </thead>
            <tbody>
              {analytics.priceDynamics.map((item, idx) => (
                <tr key={idx} style={{ borderBottom: "1px solid #f1f5f9" }}>
                  <td style={{ padding: 8 }}><b>{item.period}</b></td>
                  <td style={{ padding: 8 }}>{item.dealsCount}</td>
                  <td style={{ padding: 8, color: "#16a34a", fontWeight: 700 }}>${item.averagePrice.toLocaleString(undefined, {maximumFractionDigits: 0})}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="analytics-panels" style={{ marginTop: 20, display: "grid", gridTemplateColumns: "1fr 1fr", gap: 20 }}>
        <div className="panel">
          <h3>Ефективність рієлторів</h3>
          {analytics.agentsPerformance.map((st) => (
            <div key={st.agent.id} className="stat-row" style={{ display: "flex", justifyContent: "space-between", padding: "8px 0", borderBottom: "1px solid #eee" }}>
              <span>
                <b>{st.agent.name}</b> ({st.agent.experienceYears} р.) — угод: {st.dealsCount}
              </span>
              <span style={{ color: "#16a34a", fontWeight: 700 }}>
                ${st.totalCommissionProfit.toLocaleString()}
              </span>
            </div>
          ))}
        </div>

        <div className="panel">
          <h3>Найактивніші клієнти</h3>
          {analytics.mostActiveClients.map((cl) => (
            <div key={cl.client.id} className="stat-row" style={{ display: "flex", justifyContent: "space-between", padding: "8px 0", borderBottom: "1px solid #eee" }}>
              <span>
                <b>{cl.client.name}</b> — угод: {cl.dealsCount}, пошуків: {cl.savedSearchesCount}
              </span>
              <span style={{ fontWeight: 700 }}>${cl.totalSpent.toLocaleString()}</span>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}