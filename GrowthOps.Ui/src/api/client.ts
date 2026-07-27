const API_BASE_URL = "http://localhost:5131"; // change if your API runs on different port

export async function apiFetch<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const token = localStorage.getItem("token");

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers ?? {}),
    },
  });

  if (!response.ok) {
    let message = "Request failed";

    try {
      const error = await response.json();
      message = error.error ?? message;
    } catch {
      // ignore non-JSON errors
    }

    throw new Error(message);
  }

  if (response.status === 204) {
    return undefined as T;
  }
  return response.json();
}
