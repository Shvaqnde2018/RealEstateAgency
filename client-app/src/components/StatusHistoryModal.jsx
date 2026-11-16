const STATUS_NAMES = {
  1: "На продаж",
  2: "Оренда",
  3: "Заброньовано",
  4: "Продано",
};

export default function StatusHistoryModal({ property, onClose }) {
  if (!property) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-window" onClick={(e) => e.stopPropagation()}>
        <h3>
          Історія статусів: {property.type} ({property.address})
        </h3>
        <ul>
          {property.statusHistory?.map((h, i) => (
            <li key={i} style={{ marginBottom: 6 }}>
              <b>{new Date(h.changedAt).toLocaleString()}</b>:{" "}
              {STATUS_NAMES[h.oldStatus]} → <b>{STATUS_NAMES[h.newStatus]}</b> (
              {h.comment})
            </li>
          ))}
        </ul>
        <button onClick={onClose} className="btn btn-secondary">
          Закрити
        </button>
      </div>
    </div>
  );
}