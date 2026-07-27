import { useState } from "react";
import { getMe, login, register } from "../api/auth";

type LoginPageProps = {
  onLoginSuccess: () => void;
};

export default function LoginPage({ onLoginSuccess }: LoginPageProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [message, setMessage] = useState("");
  const [mode, setMode] = useState<"login" | "signup">("login");

  async function handleSubmit() {
    setMessage("");

    try {
      if (mode === "signup") {
        await register(email, password);
        setMessage("Account created. You can now log in.");
        setMode("login");
        return;
      }

      const loginResult = await login(email, password);
      localStorage.setItem("token", loginResult.token);

      const me = await getMe();
      setMessage(`Logged in as ${me.email} (UserId: ${me.userId})`);
      onLoginSuccess();
    } catch (error) {
      setMessage(error instanceof Error ? error.message : "Request failed");
    }
  }
  return (
    <div className="min-h-screen bg-slate-100 flex items-center justify-center px-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-sm border border-slate-200">
        <div className="mb-6">
          <p className="text-sm font-semibold text-slate-500">GrowthOps</p>

          <h1 className="mt-1 text-2xl font-bold text-slate-900">
            {mode === "login" ? "Welcome back" : "Create your account"}
          </h1>

          <p className="text-sm text-slate-500 mt-1">
            {mode === "login"
              ? "Sign in to track your goals and progress."
              : "Create an account to start tracking your goals."}
          </p>
        </div>

        <div className="space-y-4">
          <div>
            <input
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Email"
            />
          </div>
          <div>
            <input
              className="mt-1 w-full rounded-lg border border-slate-300 px-3 py-2 outline-none focus:ring-2 focus:ring-slate-400"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Password"
            />
          </div>
          <button
            onClick={handleSubmit}
            className="w-full rounded-lg bg-slate-900 px-4 py-2.5 text-white font-medium hover:bg-slate-800"
          >
            {mode === "login" ? "Login" : "Create account"}
          </button>
          <button
            type="button"
            onClick={() => {
              setMode(mode === "login" ? "signup" : "login");
              setMessage("");
            }}
            className="w-full text-sm text-slate-600 hover:text-slate-900"
          >
            {mode === "login"
              ? "Need an account? Sign up"
              : "Already have an account? Login"}
          </button>
          {message && (
            <p className="rounded-lg bg-slate-100 p-3 text-sm text-slate-700">
              {message}
            </p>
          )}
        </div>
      </div>
    </div>
  );
}
