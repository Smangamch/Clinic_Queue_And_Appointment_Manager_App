const BASE_URL = "http://localhost:5188";

export const getAppointments = async () => {
    const response = await fetch(`${BASE_URL}/appointments`);
    if (!response.ok)
        throw new Error("Failed to fetch appointments");
    return response.json();
};

export const createAppointment = async (data: any) => {
    const response = await fetch(`${BASE_URL}/appointments`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(data),
    });

    if (!response.ok)
        throw new Error("Failed to create appointment");
};

