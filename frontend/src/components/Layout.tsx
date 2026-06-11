import { NavLink, useNavigate } from 'react-router-dom';
import { BookOpen, History, LayoutDashboard, LogOut, Shield, Sparkles } from 'lucide-react';
import { useAuth } from '../context/AuthContext';

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `flex items-center gap-2 px-4 py-2 rounded-xl transition-all ${
    isActive
      ? 'bg-indigo-500/20 text-indigo-300 border border-indigo-500/30'
      : 'text-white/60 hover:text-white hover:bg-white/5'
  }`;

const Layout: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-[#0B0F1A] text-white">
      <div className="fixed inset-0 pointer-events-none">
        <div className="absolute top-0 left-1/4 w-96 h-96 bg-purple-600/10 rounded-full blur-3xl" />
        <div className="absolute bottom-0 right-1/4 w-96 h-96 bg-indigo-600/10 rounded-full blur-3xl" />
      </div>

      <nav className="relative z-10 border-b border-white/10 bg-[#0B0F1A]/80 backdrop-blur-xl">
        <div className="max-w-6xl mx-auto px-6 py-4 flex flex-wrap items-center justify-between gap-4">
          <div className="flex items-center gap-3">
            <Sparkles className="text-indigo-400" size={28} />
            <div>
              <p className="font-bold text-lg leading-tight">AI Learning Platform</p>
              <p className="text-xs text-white/40">Hello, {user?.name}</p>
            </div>
          </div>

          <div className="flex flex-wrap items-center gap-2">
            <NavLink to="/dashboard" className={linkClass}>
              <LayoutDashboard size={18} /> Dashboard
            </NavLink>
            <NavLink to="/history" className={linkClass}>
              <History size={18} /> History
            </NavLink>
            {isAdmin && (
              <NavLink to="/admin" className={linkClass}>
                <Shield size={18} /> Admin
              </NavLink>
            )}
            <button
              onClick={handleLogout}
              className="flex items-center gap-2 px-4 py-2 rounded-xl text-white/60 hover:text-red-300 hover:bg-red-500/10 transition-all"
            >
              <LogOut size={18} /> Logout
            </button>
          </div>
        </div>
      </nav>

      <main className="relative z-10">
        {children}
      </main>

      <footer className="relative z-10 text-center py-6 text-white/30 text-sm flex items-center justify-center gap-2">
        <BookOpen size={14} /> Learn smarter with AI
      </footer>
    </div>
  );
};

export default Layout;
