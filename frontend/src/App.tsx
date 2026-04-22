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
  // State for appointments, loading status, and form data
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
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
        throw new Error("Failed to create appointment");
      }

      const newAppointment = await response.json();

      // Update UI immediately
      setAppointments(prev => [newAppointment, ...prev]);

      // Reset form
      setForm({
        patientName: "",
        patientContact: "",
        scheduledAt: "",
        clinicId: "",
        status: "Pending",
        checkedIn: false
      });

    } catch (error) {
      console.error("Error:", error);
    }
};

  useEffect(() => {
    fetch("http://localhost:5188/api/appointments/query?Page=1&PageSize=5")
      .then((res) => res.json())
      .then((data: PagedResult) => {
        console.log("API RESPONSE:", data); // For debugging - check the structure of the response
        setAppointments(data.data); // Assuming the API returns { data: Appointment[], total: number }
        setLoading(false);
      })
      .catch((err) => {
        console.error("Error fetching appointments:", err);
        setLoading(false);
      });
  }, []);

  return (
    <div style={{ padding: "20px" }}>
      <h1>Clinic Appointments</h1>

      <form onSubmit={handleSubmit} style={{ marginBottom: "20px" }}>
        <input
          name="patientName"
          placeholder="Patient's Name"
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
    </div>
  );
}

export default App;