import { useEffect, useState } from "react";
import { API_BASE_URL } from "../services/api";

export type Appointment = {
  id: string;
  patientName: string;
  patientContact: string;
  scheduledAt: string;
  clinicId: string;
  checkedIn: boolean;
  status: string;
};

type PagedResult = {
  page: number;
  pageSize: number;
  totalRecords: number;
  data: Appointment[];
};

export function Appointments() {
  const [pagedResult, setPagedResult] = useState<PagedResult | null>(null);
  const appointments = pagedResult?.data || [];
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);
  const totalRecords = pagedResult?.totalRecords || 0;
  const totalPages = Math.max(1, Math.ceil(totalRecords / pageSize));
  const [statusFilter, setStatusFilter] = useState("All");
  const [editingId, setEditingId] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [form, setForm] = useState({
    patientName: "",
    patientContact: "",
    scheduledAt: "",
    clinicId: "",
    checkedIn: false,
    status: "Scheduled"
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value, type } = e.target;

    setForm(prev => ({
      ...prev,
      [name]: type === "checkbox"
        ? (e.target as HTMLInputElement).checked
        : value
    }));
  };

  const handleEdit = (appointment: Appointment) => {
    setForm({
      patientName: appointment.patientName,
      patientContact: appointment.patientContact,
      clinicId: appointment.clinicId,
      checkedIn: appointment.checkedIn,
      scheduledAt: appointment.scheduledAt.slice(0, 16),
      status: appointment.status
    });

    setEditingId(appointment.id);
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Delete this appointment?")) return;

    try {
      const response = await fetch(`${API_BASE_URL}/appointments/${id}`, {
        method: "DELETE"
      });

      if (!response.ok) {
        throw new Error("Failed to delete appointment");
      }

      setPagedResult(prev => prev ? {
        ...prev,
        data: prev.data.filter(a => a.id !== id),
        totalRecords: prev.totalRecords - 1
      } : prev);
      setMessage("Appointment deleted successfully");

    } catch (err: any) {
      setError(err.message || "Unable to delete appointment");
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setMessage("");

    try {
      const url = editingId
        ? `${API_BASE_URL}/appointments/${editingId}`
        : `${API_BASE_URL}/appointments`;

      const response = await fetch(url, {
        method: editingId ? "PUT" : "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          ...form,
          scheduledAt: new Date(form.scheduledAt).toISOString()
        })
      });

      if (!response.ok) {
        throw new Error("Failed to save appointment");
      }

      const savedAppointment: Appointment = await response.json();

      if (editingId) {
        setPagedResult(prev => prev ? {
          ...prev,
          data: prev.data.map(a => a.id === editingId ? savedAppointment : a)
        } : prev);
        setMessage("Appointment updated successfully");
      } else {
        setPagedResult(prev => prev ? {
          ...prev,
          data: [savedAppointment, ...prev.data],
          totalRecords: prev.totalRecords + 1
        } : {
          page,
          pageSize,
          totalRecords: 1,
          data: [savedAppointment]
        });
        setMessage("Appointment created successfully");
      }

      setEditingId(null);
      setPage(1);
      setShowForm(false);
      setForm({
        patientName: "",
        patientContact: "",
        scheduledAt: "",
        clinicId: "",
        status: "Scheduled",
        checkedIn: false
      });
    } catch (err: any) {
      setError(err.message || "Error saving appointment");
    }
  };

  useEffect(() => {
    const fetchAppointments = async () => {
      setLoading(true);
      try {
        let url = `${API_BASE_URL}/appointments/query?page=${page}&pageSize=${pageSize}`;
        if (statusFilter !== "All") {
          url += `&status=${encodeURIComponent(statusFilter)}`;
        }
        const res = await fetch(url);
        if (!res.ok) {
          const text = await res.text();
          throw new Error(text || `${res.status} ${res.statusText}`);
        }
        const data: PagedResult = await res.json();
        setPagedResult(data);
      } catch (err: any) {
        console.error("Error fetching appointments:", err);
        setError("Unable to load appointments");
      } finally {
        setLoading(false);
      }
    };

    fetchAppointments();
  }, [page, statusFilter]);

  useEffect(() => {
    if (!message) return;
    const timer = window.setTimeout(() => setMessage(""), 3000);
    return () => window.clearTimeout(timer);
  }, [message]);

  return (
    <section className="appointments-page">
      <div className="appointments-header">
        <div>
          <h1>Appointments</h1>
          <p>Manage appointments inside the clinic application.</p>
        </div>

        <button className="primary-button" onClick={() => {
          setEditingId(null);
          setShowForm(prev => !prev);
        }}>
          {showForm ? "Hide form" : "New appointment"}
        </button>
      </div>

      {message && <p style={{ color: "green" }}>{message}</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}

      {showForm && (
        <form onSubmit={handleSubmit} className="appointments-form-card">
          <div className="appointments-form-grid">
            <input name="patientName" placeholder="Name and surname" value={form.patientName} onChange={handleChange} required />
            <input name="patientContact" placeholder="Contact" value={form.patientContact} onChange={handleChange} required />
          </div>

          <div className="appointments-form-grid">
            <input name="clinicId" placeholder="Clinic ID" value={form.clinicId} onChange={handleChange} required />
            <input type="datetime-local" name="scheduledAt" value={form.scheduledAt} onChange={handleChange} required />
          </div>

          <div className="appointments-form-grid appointments-form-actions">
            <select name="status" value={form.status} onChange={handleChange}>
              <option value="Pending">Pending</option>
              <option value="Scheduled">Scheduled</option>
              <option value="CheckedIn">Checked In</option>
              <option value="Completed">Completed</option>
            </select>

            <label className="checkbox-label">
              <input type="checkbox" name="checkedIn" checked={form.checkedIn} onChange={handleChange} /> Checked in
            </label>
          </div>

          <button type="submit" className="secondary-button">
            {editingId ? "Update appointment" : "Create appointment"}
          </button>
        </form>
      )}

      <div className="appointments-toolbar">
        <div className="appointments-filter">
          <label>
            Filter status:
            <select value={statusFilter} onChange={e => { setPage(1); setStatusFilter(e.target.value); }}>
              <option value="All">All</option>
              <option value="Scheduled">Scheduled</option>
              <option value="Incomplete">Incomplete</option>
              <option value="Completed">Completed</option>
            </select>
          </label>
        </div>
        <div className="appointments-pagination">
          <button className="secondary-button" disabled={page === 1} onClick={() => setPage(prev => Math.max(1, prev - 1))}>Previous</button>
          <button className="secondary-button" disabled={page === totalPages} onClick={() => setPage(prev => Math.min(totalPages, prev + 1))}>Next</button>
        </div>
      </div>

      {loading ? (
        <p>Loading appointments...</p>
      ) : (
        <div style={{ overflowX: "auto", background: "#ffffff", border: "1px solid #e2e8f0", borderRadius: "14px", padding: "16px" }}>
          <table style={{ width: "100%", borderCollapse: "collapse" }}>
            <thead>
              <tr>
                <th>Patient</th>
                <th>Contact</th>
                <th>Clinic</th>
                <th>Checked In</th>
                <th>Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {appointments.map(a => (
                <tr key={a.id}>
                  <td><span className="truncate" title={a.patientName}>{a.patientName}</span></td>
                  <td><span className="truncate" title={a.patientContact}>{a.patientContact}</span></td>
                  <td><span className="truncate" title={a.clinicId}>{a.clinicId}</span></td>
                  <td>{a.checkedIn ? "Yes" : "No"}</td>
                  <td>{new Date(a.scheduledAt).toLocaleString()}</td>
                  <td>{a.status || "N/A"}</td>
                  <td className="actions-col">
                    <details className="action-menu">
                      <summary className="action-compact" aria-label="Actions">⋯</summary>
                      <div className="action-group">
                        <button className="edit-btn" onClick={() => handleEdit(a)}>Edit</button>
                        <button className="delete-btn" onClick={() => handleDelete(a.id)}>Delete</button>
                      </div>
                    </details>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      <p style={{ marginTop: "16px" }}>
        Page {page} of {totalPages} · {totalRecords} appointments total
      </p>
    </section>
  );
}
