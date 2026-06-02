import { useState } from "react";
import { Outlet } from "react-router-dom";
import { Sidebar } from "../Sidebar/Sidebar";
import "./Layout.css";

export function Layout() {
  const [collapsed, setCollapsed] = useState(false);

  return (
    <div className={`app-shell ${collapsed ? "collapsed" : "expanded"}`}>
      <Sidebar collapsed={collapsed} onToggle={() => setCollapsed(prev => !prev)} />
      <main className="app-content">
        <Outlet />
      </main>
    </div>
  );
}
