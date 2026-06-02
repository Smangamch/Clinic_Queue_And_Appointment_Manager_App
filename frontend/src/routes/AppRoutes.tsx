import { Routes, Route } from "react-router-dom";
import { Layout } from "../components/Layout/Layout";
import { Dashboard } from "../pages/Dashboard";
import { Appointments } from "../pages/Appointments";
import { Queue } from "../pages/Queue";
import { Notifications } from "../pages/Notifications";
import { Help } from "../pages/Help";
import { Settings } from "../pages/Settings";

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Dashboard />} />
        <Route path="appointments" element={<Appointments />} />
        <Route path="queue" element={<Queue />} />
        <Route path="notifications" element={<Notifications />} />
        <Route path="help" element={<Help />} />
        <Route path="settings" element={<Settings />} />
      </Route>
    </Routes>
  );
}
