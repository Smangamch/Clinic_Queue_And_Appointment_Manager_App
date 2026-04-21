import { useEffect, useState } from "react";

type Appointment = {
  id: string;
  patientName: string;
  patientContact: string;
  scheduledAt: string;
  clinicId: string;
  checkedIn: boolean;
  status: string;
};

function App() {
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetch("http://localhost:5188/api/appointments/query?Page=1&PageSize=5")
      .then((res) => res.json())
      .then((data) => {
        console.log("API RESPONSE:", data); // For debugging - check the structure of the response
        setAppointments(data); // Assuming the API returns { data: Appointment[], total: number }
        setLoading(false);
      })
      .catch((err) => {
        console.error(err);
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <h2>Loading appointments...</h2>;
  if (error) return <h2>Error: {error}</h2>;

  return (
    <div style={{ padding: "20px" }}>
      <h1>Clinic Appointments</h1>

      {appointments.length === 0 ? (
        <p>No appointments found.</p>
      ) : (
        <table border={1} cellPadding={10}>
          <thead>
            <tr>
              <th>Patient</th>
              <th>Contact</th>
              <th>Scheduled</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {appointments.map((a) => (
              <tr key={a.id}>
                <td>{a.patientName}</td>
                <td>{a.patientContact}</td>
                <td>{new Date(a.scheduledAt).toLocaleString()}</td>
                <td>{a.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
}

export default App;