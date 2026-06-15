import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Sparkles } from 'lucide-react';
import api, { getApiErrorMessage } from './api';

const Register = () => {
  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [phone, setPhone] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleRegister = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await api.post('/Auth/register', { name, email, phone, password });
      navigate('/login');
    } catch (err: any) {
      setError(getApiErrorMessage(err, 'Registration failed'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="relative min-h-screen flex items-center justify-center bg-[#0B0F1A] text-white p-6">
      <div className="absolute inset-0 pointer-events-none">
        <div className="absolute top-1/3 left-1/2 -translate-x-1/2 w-[500px] h-[500px] bg-purple-600/15 rounded-full blur-3xl" />
      </div>
      <div className="relative z-10 w-full max-w-md p-10 rounded-3xl border border-white/10 bg-white/5 backdrop-blur-2xl shadow-2xl">
        <div className="flex items-center justify-center gap-2 mb-2">
          <Sparkles className="text-purple-400" />
          <span className="text-sm text-purple-300 font-medium">Join the platform</span>
        </div>
        <h1 className="text-4xl font-bold mb-8 text-center">Create Account</h1>

        <form onSubmit={handleRegister} className="space-y-4">
          <input
            required
            className="w-full p-4 rounded-2xl bg-white/5 border border-white/10 focus:outline-none focus:ring-2 focus:ring-purple-500"
            placeholder="Full name"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
          <input
            type="email"
            required
            className="w-full p-4 rounded-2xl bg-white/5 border border-white/10 focus:outline-none focus:ring-2 focus:ring-purple-500"
            placeholder="Email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
          <input
            className="w-full p-4 rounded-2xl bg-white/5 border border-white/10 focus:outline-none focus:ring-2 focus:ring-purple-500"
            placeholder="Phone (optional)"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />
          <input
            type="password"
            required
            minLength={6}
            className="w-full p-4 rounded-2xl bg-white/5 border border-white/10 focus:outline-none focus:ring-2 focus:ring-purple-500"
            placeholder="Password (min 6 chars)"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          {error && <p className="text-red-400 text-sm text-center">{error}</p>}
          <button
            type="submit"
            disabled={loading}
            className="w-full p-4 rounded-2xl bg-gradient-to-r from-purple-500 to-indigo-500 font-semibold hover:shadow-[0_0_20px_rgba(168,85,247,0.4)] transition-all disabled:opacity-50"
          >
            {loading ? 'Creating...' : 'Sign up'}
          </button>
        </form>

        <p className="text-center text-xs text-white/40 mt-6">
          Already registered?{' '}
          <button onClick={() => navigate('/login')} className="text-indigo-400 hover:underline">
            Sign in
          </button>
        </p>
      </div>
    </div>
  );
};

export default Register;
