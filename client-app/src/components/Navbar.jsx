import { NavLink } from "react-router-dom";

export default function Navbar() {
  const links = [
    { to: "/", label: "Нерухомість та Фільтри" },
    { to: "/clients", label: "Клієнти та Підписки" },
    { to: "/deals", label: "Угоди та Рієлтори" },
    { to: "/analytics", label: "Статистика та Аналітика" },
  ];

  return (
    <header className="app-header">
      <div>
        <h1>Рієлторське агентство — CRM</h1>
        <small>Трирівнева архітектура C# .NET + React Router SPA</small>
      </div>
      <nav className="nav-tabs">
        {links.map((link) => (
          <NavLink
            key={link.to}
            to={link.to}
            end={link.to === "/"}
            className={({ isActive }) => `tab-btn ${isActive ? "active" : ""}`}
            style={{ textDecoration: "none" }}
          >
            {link.label}
          </NavLink>
        ))}
      </nav>
    </header>
  );
}