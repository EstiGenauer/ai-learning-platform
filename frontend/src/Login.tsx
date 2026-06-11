import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Sparkles } from 'lucide-react';
import api from './api';
import { useAuth } from './context/AuthContext';

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleSignIn = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await api.post('/Auth/login', { email, password });
      login(res.data.token, res.data.user);
      navigate(res.data.user.isAdmin ? '/admin' : '/dashboard');
    } catch (err: any) {
      const msg = err.message || err.response?.data?.message || 'Login failed';
      setError(msg);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen flex items-center justify-center bg-[#0B0F1A] text-white p-6">
      <div className="absolute inset-0 pointer-events-none">
        <div className="absolute top-1/3 left-1/2 -translate-x-1/2 w-[500px] h-[500px] bg-indigo-600/15 rounded-full blur-3xl" />
      </div>
      <div className="relative z-10 w-full max-w-md p-10 rounded-3xl border border-white/10 bg-white/5 backdrop-blur-2xl shadow-2xl">
        <div className="flex items-center justify-center gap-2 mb-2">
          <Sparkles className="text-indigo-400" />
          <span className="text-sm text-indigo-300 font-medium">AI Learning Platform</span>
        </div>
        <h1 className="text-4xl font-bold mb-2 text-center">Welcome back</h1>
        <p className="text-center text-white/40 text-sm mb-8">Sign in to continue learning</p>

        <form onSubmit={handleSignIn} className="space-y-5">
          <input
            type="email"
            required
            className="w-full p-4 rounded-2xl bg-white/5 border border-white/10 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <input
            type="password"
            required
            className="w-full p-4 rounded-2xl bg-white/5 border border-white/10 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            placeholder="Password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {error && <p className="text-red-400 text-sm text-center">{error}</p>}
          <button
            type="submit"
            disabled={loading}
            className="w-full p-4 rounded-2xl bg-gradient-to-r from-purple-500 to-indigo-500 font-semibold hover:shadow-[0_0_20px_rgba(99,102,241,0.4)] transition-all disabled:opacity-50"
          >
            {loading ? 'Signing in...' : 'Sign in'}
          </button>
        </form>

        <p className="text-center text-xs text-white/40 mt-6">
          No account?{' '}
          <button onClick={() => navigate('/register')} className="text-indigo-400 hover:underline">
            Create one
          </button>
        </p>
        <p className="text-center text-xs text-white/25 mt-3">
          Admin demo: admin@admin.com / Admin123!
        </p>
      </div>
    </div>
  );
};

export default Login;
