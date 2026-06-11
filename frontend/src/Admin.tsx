import React, { useCallback, useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { FolderTree, MessageSquare, Plus, Shield, Trash2, Users } from 'lucide-react';
import api from './api';
import Layout from './components/Layout';
import { AdminUser, Category, PromptItem } from './types';

type Tab = 'users' | 'prompts' | 'categories';

const getApiError = (err: any, fallback: string) => {
  const data = err.response?.data;
  if (typeof data?.message === 'string') return data.message;
  if (data?.errors) {
    const messages = Object.values(data.errors).flat();
    if (messages.length) return messages.join(' ');
  }
  return err.message || fallback;
};

const Admin = () => {
  const [users, setUsers] = useState<AdminUser[]>([]);
  const [prompts, setPrompts] = useState<PromptItem[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [tab, setTab] = useState<Tab>('users');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [newCategory, setNewCategory] = useState('');
  const [newSubName, setNewSubName] = useState('');
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | ''>('');

  const loadCategories = useCallback(async () => {
    const res = await api.get<Category[]>('/Categories');
    setCategories(res.data);
  }, []);

  const loadData = useCallback(async () => {
    setLoading(true);
    setError('');
    try {
      const [usersRes, promptsRes] = await Promise.all([
        api.get<AdminUser[]>('/Admin/users'),
        api.get<PromptItem[]>('/Admin/prompts'),
      ]);
      setUsers(usersRes.data);
      setPrompts(promptsRes.data);
      await loadCategories();
    } catch (err: any) {
      setError(err.response?.data?.message || err.message || 'Failed to load admin data');
    } finally {
      setLoading(false);
    }
  }, [loadCategories]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleAddCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newCategory.trim()) return;
    setError('');
    setSuccess('');
    try {
      await api.post('/Admin/categories', { name: newCategory.trim() });
      setNewCategory('');
      setSuccess(`Category "${newCategory.trim()}" added successfully.`);
      await loadCategories();
    } catch (err: any) {
      const msg = getApiError(err, 'Failed to add category');
      setError(msg);
      if (msg.includes('already exists')) {
        await loadCategories();
        setSuccess('Category already exists — list refreshed below.');
      }
    }
  };

  const handleAddSubCategory = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedCategoryId || !newSubName.trim()) return;
    setError('');
    setSuccess('');
    try {
      await api.post(`/Admin/categories/${selectedCategoryId}/subcategories`, {
        name: newSubName.trim(),
      });
      setNewSubName('');
      setSuccess('Sub-category added successfully.');
      await loadCategories();
    } catch (err: any) {
      setError(getApiError(err, 'Failed to add sub-category'));
    }
  };

  const handleDeleteCategory = async (id: number) => {
    if (!window.confirm('Delete this category and all its sub-categories?')) return;
    try {
      await api.delete(`/Admin/categories/${id}`);
      await loadCategories();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete category');
    }
  };

  const handleDeleteSubCategory = async (id: number) => {
    if (!window.confirm('Delete this sub-category?')) return;
    try {
      await api.delete(`/Admin/subcategories/${id}`);
      await loadCategories();
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to delete sub-category');
    }
  };

  const tabClass = (active: boolean) =>
    `flex items-center gap-2 px-5 py-2.5 rounded-xl transition-all ${
      active ? 'bg-purple-500/20 text-purple-300 border border-purple-500/30' : 'text-white/50 hover:bg-white/5'
    }`;

  return (
    <Layout>
      <div className="p-6 md:p-12 max-w-6xl mx-auto">
        <div className="flex items-center gap-3 mb-8">
          <Shield className="text-purple-400" size={32} />
          <div>
            <h2 className="text-3xl font-bold">Admin Dashboard</h2>
            <p className="text-white/40 text-sm">Manage users, topics, and learning activity</p>
          </div>
        </div>

        <div className="flex flex-wrap gap-2 mb-8">
          <button onClick={() => setTab('users')} className={tabClass(tab === 'users')}>
            <Users size={18} /> Users ({users.length})
          </button>
          <button onClick={() => setTab('categories')} className={tabClass(tab === 'categories')}>
            <FolderTree size={18} /> Categories ({categories.length})
          </button>
          <button onClick={() => setTab('prompts')} className={tabClass(tab === 'prompts')}>
            <MessageSquare size={18} /> All Prompts ({prompts.length})
          </button>
        </div>

        {error && (
          <p className="mb-4 text-red-400 text-sm text-center bg-red-500/10 border border-red-500/20 rounded-xl p-3">
            {error}
          </p>
        )}
        {success && (
          <p className="mb-4 text-green-400 text-sm text-center bg-green-500/10 border border-green-500/20 rounded-xl p-3">
            {success}
          </p>
        )}

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

        {!loading && tab === 'categories' && (
          <div className="space-y-8">
            <div className="grid md:grid-cols-2 gap-6">
              <form onSubmit={handleAddCategory} className="p-6 rounded-2xl bg-white/5 border border-white/10 space-y-4">
                <h3 className="font-semibold text-indigo-300 flex items-center gap-2">
                  <Plus size={18} /> Add Category
                </h3>
                <input
                  value={newCategory}
                  onChange={(e) => setNewCategory(e.target.value)}
                  placeholder="e.g. Cyber Security"
                  className="w-full p-3 rounded-xl bg-[#151926] border border-white/10 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  required
                />
                <button type="submit" className="w-full py-3 rounded-xl bg-gradient-to-r from-purple-600 to-indigo-600 font-semibold">
                  Add Category
                </button>
              </form>

              <form onSubmit={handleAddSubCategory} className="p-6 rounded-2xl bg-white/5 border border-white/10 space-y-4">
                <h3 className="font-semibold text-indigo-300 flex items-center gap-2">
                  <Plus size={18} /> Add Sub-category
                </h3>
                <select
                  value={selectedCategoryId}
                  onChange={(e) => setSelectedCategoryId(Number(e.target.value) || '')}
                  className="w-full p-3 rounded-xl bg-[#151926] border border-white/10 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  required
                >
                  <option value="">Select category</option>
                  {categories.map((c) => (
                    <option key={c.id} value={c.id}>{c.name}</option>
                  ))}
                </select>
                <input
                  value={newSubName}
                  onChange={(e) => setNewSubName(e.target.value)}
                  placeholder="e.g. C, Python, Calculus"
                  className="w-full p-3 rounded-xl bg-[#151926] border border-white/10 focus:outline-none focus:ring-2 focus:ring-indigo-500"
                  required
                />
                <button type="submit" className="w-full py-3 rounded-xl bg-gradient-to-r from-purple-600 to-indigo-600 font-semibold">
                  Add Sub-category
                </button>
              </form>
            </div>

            <div className="space-y-4">
              {categories.map((cat) => (
                <motion.div
                  key={cat.id}
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  className="p-5 rounded-2xl bg-white/5 border border-white/10"
                >
                  <div className="flex items-center justify-between mb-3">
                    <h4 className="font-bold text-lg text-purple-300">{cat.name}</h4>
                    <button
                      onClick={() => handleDeleteCategory(cat.id)}
                      className="text-red-400/70 hover:text-red-400 flex items-center gap-1 text-sm"
                    >
                      <Trash2 size={14} /> Delete
                    </button>
                  </div>
                  <div className="flex flex-wrap gap-2">
                    {cat.subCategories.map((sub) => (
                      <span
                        key={sub.id}
                        className="inline-flex items-center gap-2 px-3 py-1.5 rounded-full bg-indigo-500/20 text-indigo-200 text-sm"
                      >
                        {sub.name}
                        <button
                          onClick={() => handleDeleteSubCategory(sub.id)}
                          className="text-red-300/70 hover:text-red-300"
                          aria-label={`Delete ${sub.name}`}
                        >
                          <Trash2 size={12} />
                        </button>
                      </span>
                    ))}
                    {cat.subCategories.length === 0 && (
                      <span className="text-white/30 text-sm">No sub-categories yet</span>
                    )}
                  </div>
                </motion.div>
              ))}
            </div>
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
