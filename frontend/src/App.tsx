import { useEffect, useState } from "react";
import { getAppointments } from "./services/api";

function App() {
  const [appointments, setAppointments] = useState<any[]>([]);

  useEffect(() => {
    getAppointments()
      .then(setAppointments)
      .catch(console.error);
  }, []);

  return (
    <div>
      <h1>Appointments</h1>
      {appointments.map((a) => (
        <div key={a.id}>
          <p>{a.patientName}</p>
          <p>{a.scheduledAt}</p>
        </div>
      ))}
    </div>
  );
}

export default App;