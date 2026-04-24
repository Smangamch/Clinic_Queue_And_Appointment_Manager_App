import { useEffect, useState } from "react";

/**
 * Type definition for an Appointment entity.
 * Represents the structure of appointment data fetched from the backend API.
 */
type Appointment = {
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

function App() {
  // State variables for managing appointments, loading status, messages, errors, pagination, and form data
  const [pagedResult, setPagedResult] = useState<PagedResult | null>(null);
  const appointments = pagedResult?.data || [];
  const [loading, setLoading] = useState(true);
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [page, setPage] = useState(1);
  const [pageSize] = useState(5);
  const totalRecords = pagedResult?.totalRecords || 0;
  const totalPages = Math.ceil(totalRecords / pageSize);
  const [statusFilter, setStatusFilter] = useState("All");
  const [form, setForm] = useState({
    patientName: "",
    patientContact: "",
    scheduledAt: "",
    clinicId: "",
    checkedIn: false,
    status: "Scheduled"
  });

  // Generic change handler for form inputs
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
  const { name, value, type } = e.target;

  setForm(prev => ({
    ...prev,
    [name]: type === "checkbox" 
      ? (e.target as HTMLInputElement).checked 
      : value
  }));
};

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      // Reset previous messages
      setError("");
      setMessage("");

      const response = await fetch("http://localhost:5188/api/appointments", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          ...form,
          scheduledAt: new Date(form.scheduledAt).toISOString()
        })
      });

      if (!response.ok) {
        const errText = await response.text(); // 🔹 get backend error
        throw new Error(errText || "Failed to create appointment");
      }

      // Consume response but don't store since we'll refetch
      await response.json();
      // Reset to first page to show new appointment
      setMessage("Appointment created successfully");
      setPage(1);

      // Reset form
      setForm({
        patientName: "",
        patientContact: "",
        scheduledAt: "",
        clinicId: "",
        status: "Pending",
        checkedIn: false
      });

    } catch (err: any) {
      setError(err.message);
      console.error("Error:", err);
    }
  };  

  // Fetch appointments on component mount
  useEffect(() => {
  const fetchAppointments = async () => {
    setLoading(true);

    try {
      let url = `http://localhost:5188/api/appointments/query?page=${page}&pageSize=${pageSize}`;

      if (statusFilter !== "All") {
        url += `&status=${statusFilter}`;
      }

      const res = await fetch(url);
      const data: PagedResult = await res.json();

      setPagedResult(data);

    } catch (err) {
      console.error("Error fetching appointments:", err);
    } finally {
      setLoading(false);
    }
  };

  fetchAppointments();
}, [page, statusFilter]);

  useEffect(() => {
    if (!message) return;

    const timer = setTimeout(() => {
      setMessage("");
    }, 3000);

    return () => clearTimeout(timer);
  }, [message]);

  // Render the UI
  return (
    <div style={{ padding: "20px" }}>
      <h1>Clinic Appointments</h1>

      {message && <p style={{ color: "green" }}>{message}</p>}
      {error && <p style={{ color: "red" }}>{error}</p>}

      <div className="form-container">
      <form onSubmit={handleSubmit} style={{ marginBottom: "20px" }}>
        <input
          name="patientName"
          placeholder="Name and Surname"
          value={form.patientName}
          onChange={handleChange}
          required
        />

        <input
          name="patientContact"
          placeholder="Contact"
          value={form.patientContact}
          onChange={handleChange}
          required
        />

        <input
          name="clinicId"
          placeholder="Clinic ID"
          value={form.clinicId}
          onChange={handleChange}
          required
        />

        <label>
          CheckedIn:
          <input
            type="checkbox"
            name="checkedIn"
            checked={form.checkedIn}
            onChange={handleChange}
          />
        </label>

        <input
          type="datetime-local"
          name="scheduledAt"
          value={form.scheduledAt}
          onChange={handleChange}
          required
        />

        <select name="status" value={form.status} onChange={handleChange}>
          <option value="Pending">Pending</option>
          <option value="Scheduled">Scheduled</option>
          <option value="CheckedIn">Checked In</option>
          <option value="Completed">Completed</option>
        </select>

        <button type="submit">Create</button>
      </form>
      </div>

      <select
        value={statusFilter}
        onChange={(e) => {
          setPage(1);
          setStatusFilter(e.target.value);
        }}
      >
        <option value="All">All</option>
        <option value="Scheduled">Scheduled</option>
        <option value="Incomplete">Incomplete</option>
        <option value="Completed">Completed</option> 
      </select>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <table border={1} cellPadding={10}>
          <thead>
            <tr>
              <th>Patient</th>
              <th>Contact</th>
              <th>Clinic</th>
              <th>Checked In</th>
              <th>Date</th>
              <th>Status</th>
            </tr>
          </thead>

          <tbody>
            {appointments.map(a => (
              <tr key={a.id}>
                <td>{a.patientName}</td>
                <td>{a.patientContact}</td>
                <td>{a.clinicId}</td>
                <td>{a.checkedIn ? "Yes" : "No"}</td>
                <td>{new Date(a.scheduledAt).toLocaleString()}</td>
                <td>{a.status || "N/A"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

        <div style={{ marginTop: "20px" }}>
          <button 
            onClick={() => setPage(prev => prev - 1)} 
            disabled={page === 1}
          >
            Previous
          </button>

          <span style={{ margin: "0 10px" }}>
            Page {page} of {totalPages}
          </span>

          <button 
            onClick={() => setPage(prev => prev + 1)} 
            disabled={page === totalPages}
          >
            Next
          </button>
      </div>
    </div>
  );
}

export default App;