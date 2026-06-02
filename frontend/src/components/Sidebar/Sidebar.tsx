import { NavLink } from "react-router-dom";
import "./Sidebar.css";

type NavItem = {
  to: string;
  label: string;
  icon: string;
};

const navItems: NavItem[] = [
  { to: "/", label: "Dashboard", icon: "🏠" },
  { to: "/appointments", label: "Book Appointment", icon: "📅" },
  { to: "/queue", label: "Check Queue", icon: "📋" },
  { to: "/notifications", label: "Notifications", icon: "🔔" },
  { to: "/help", label: "Help", icon: "❓" },
  { to: "/settings", label: "Settings", icon: "⚙" }
];

export function Sidebar({
  collapsed,
  onToggle
}: {
  collapsed: boolean;
  onToggle: () => void;
}) {
  return (
    <aside className={`sidebar${collapsed ? " collapsed" : ""}`}>
      <div className="sidebar-header">
        <button
          className="sidebar-toggle"
          onClick={onToggle}
          aria-label={collapsed ? "Expand sidebar" : "Collapse sidebar"}
        >
          <span className="hamburger" />
          <span className="sidebar-title">Clinic Queue</span>
        </button>
      </div>

      <nav className="sidebar-nav">
        {navItems.map(({ to, label, icon }) => (
          <NavLink
            key={to}
            to={to}
            end={to === "/"}
            className={({ isActive }) =>
              `sidebar-link${isActive ? " active" : ""}`
            }
          >
            <span className="sidebar-icon">{icon}</span>
            <span className="sidebar-label">{label}</span>
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
