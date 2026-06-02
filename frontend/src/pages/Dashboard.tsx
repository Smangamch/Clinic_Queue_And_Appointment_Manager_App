import { useNavigate } from "react-router-dom";

export function Dashboard() {
  const navigate = useNavigate();

  return (
    <section className="dashboard-page">
      <header className="dashboard-hero">
        <div>
          <p className="dashboard-subtitle">Clinic Queue & Appointment Manager</p>
          <h1>Welcome, Mangaliso</h1>
        </div>
      </header>

      <div className="dashboard-grid">
        <article className="card card-hero">
          <h2>Today's Queue Status</h2>
          <p className="dashboard-value">15 Patients Waiting</p>
        </article>

        <article className="card card-hero">
          <h2>Upcoming Appointment</h2>
          <p className="dashboard-value">14 June 2026 · 09:30</p>
        </article>

        <section className="card card-actions">
          <h2>Quick Actions</h2>
          <p>Select one of the common tasks below.</p>
          <div className="action-grid">
            <button type="button" onClick={() => navigate("/appointments")}>📅 Book Appointment</button>
            <button type="button" onClick={() => navigate("/queue")}>📋 Check Queue</button>
            <button type="button" onClick={() => navigate("/appointments")}>🗓 View Appointments</button>
          </div>
        </section>
      </div>
    </section>
  );
}
