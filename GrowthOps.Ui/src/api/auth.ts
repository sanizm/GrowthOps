import { apiFetch } from "./client";

export type LoginResponse = {
  token: string;
  id: number;
  email: string;
};

export type MeResponse = {
  userId: string;
  email: string;
};

export type RegisterResponse = {
  id: number;
  email: string;
};

export function login(email: string, password: string) {
  return apiFetch<LoginResponse>("/auth/login", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}

export function getMe() {
  return apiFetch<MeResponse>("/me");
}

export function register(email: string, password: string) {
  return apiFetch<RegisterResponse>("/auth/register", {
    method: "POST",
    body: JSON.stringify({ email, password }),
  });
}
