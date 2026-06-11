import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { Shield, Users, MessageSquare } from 'lucide-react';
import api from './api';
import Layout from './components/Layout';
import { AdminUser, PromptItem } from './types';

const Admin = () => {
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [prompts, setPrompts] = useState<PromptItem[]>([]);
  const [tab, setTab] = useState<'users' | 'prompts'>('users');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      api.get<AdminUser[]>('/Admin/users'),
      api.get<PromptItem[]>('/Admin/prompts'),
    ]).then(([usersRes, promptsRes]) => {
      setUsers(usersRes.data);
      setPrompts(promptsRes.data);
    }).finally(() => setLoading(false));
  }, []);

  return (
    <Layout>
      <div className="p-6 md:p-12 max-w-6xl mx-auto">
        <div className="flex items-center gap-3 mb-8">
          <Shield className="text-purple-400" size={32} />
          <div>
            <h2 className="text-3xl font-bold">Admin Dashboard</h2>
            <p className="text-white/40 text-sm">Manage users and monitor all learning activity</p>
          </div>
        </div>

        <div className="flex gap-2 mb-8">
          <button
            onClick={() => setTab('users')}
            className={`flex items-center gap-2 px-5 py-2.5 rounded-xl transition-all ${
              tab === 'users' ? 'bg-purple-500/20 text-purple-300 border border-purple-500/30' : 'text-white/50 hover:bg-white/5'
            }`}
          >
            <Users size={18} /> Users ({users.length})
          </button>
          <button
            onClick={() => setTab('prompts')}
            className={`flex items-center gap-2 px-5 py-2.5 rounded-xl transition-all ${
              tab === 'prompts' ? 'bg-indigo-500/20 text-indigo-300 border border-indigo-500/30' : 'text-white/50 hover:bg-white/5'
            }`}
          >
            <MessageSquare size={18} /> All Prompts ({prompts.length})
          </button>
        </div>

        {loading && <p className="text-white/40 text-center py-12">Loading admin data...</p>}

        {!loading && tab === 'users' && (
          <div className="overflow-x-auto rounded-2xl border border-white/10">
            <table className="w-full text-sm">
              <thead className="bg-white/5 text-white/50">
                <tr>
                  <th className="text-left p-4">ID</th>
                  <th className="text-left p-4">Name</th>
                  <th className="text-left p-4">Email</th>
                  <th className="text-left p-4">Phone</th>
                  <th className="text-left p-4">Role</th>
                  <th className="text-left p-4">Prompts</th>
                </tr>
              </thead>
              <tbody>
                {users.map((u) => (
                  <motion.tr
                    key={u.id}
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    className="border-t border-white/5 hover:bg-white/5"
                  >
                    <td className="p-4">{u.id}</td>
                    <td className="p-4 font-medium">{u.name}</td>
                    <td className="p-4 text-white/70">{u.email}</td>
                    <td className="p-4 text-white/50">{u.phone || '—'}</td>
                    <td className="p-4">
                      <span className={`px-2 py-1 rounded-full text-xs ${u.isAdmin ? 'bg-purple-500/20 text-purple-300' : 'bg-white/10 text-white/50'}`}>
                        {u.isAdmin ? 'Admin' : 'User'}
                      </span>
                    </td>
                    <td className="p-4">{u.promptCount}</td>
                  </motion.tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {!loading && tab === 'prompts' && (
          <div className="space-y-4">
            {prompts.map((p) => (
              <motion.div
                key={p.id}
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                className="p-5 rounded-2xl bg-white/5 border border-white/10"
              >
                <div className="flex flex-wrap gap-2 mb-2 text-xs">
                  <span className="px-2 py-1 rounded-full bg-purple-500/20 text-purple-300">{p.userName}</span>
                  <span className="px-2 py-1 rounded-full bg-indigo-500/20 text-indigo-300">
                    {p.categoryName} / {p.subCategoryName}
                  </span>
                  <span className="text-white/30">{new Date(p.createdAt).toLocaleString()}</span>
                </div>
                <p className="font-medium text-indigo-300 mb-1">{p.promptText}</p>
                <p className="text-white/60 text-sm whitespace-pre-wrap line-clamp-4">{p.response}</p>
              </motion.div>
            ))}
          </div>
        )}
      </div>
    </Layout>
  );
};

export default Admin;
