import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { BookOpen, Loader2, Send, Sparkles } from 'lucide-react';
import api from './api';
import Layout from './components/Layout';
import { Category, PromptItem } from './types';

const Dashboard = () => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [categoryId, setCategoryId] = useState<number | ''>('');
  const [subCategoryId, setSubCategoryId] = useState<number | ''>('');
  const [prompt, setPrompt] = useState('');
  const [response, setResponse] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const subCategories =
    categories.find((c) => c.id === categoryId)?.subCategories ?? [];

  useEffect(() => {
    api.get<Category[]>('/Categories').then((res) => setCategories(res.data));
  }, []);

  useEffect(() => {
    setSubCategoryId('');
  }, [categoryId]);

  const handleGenerate = async () => {
    if (!categoryId || !subCategoryId || !prompt.trim()) {
      setError('Please select a topic and enter your learning request.');
      return;
    }
    setError('');
    setLoading(true);
    setResponse('');
    try {
      const res = await api.post<PromptItem>('/Prompts', {
        categoryId,
        subCategoryId,
        promptText: prompt,
      });
      setResponse(res.data.response);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to generate lesson');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Layout>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="p-6 md:p-12 max-w-3xl mx-auto"
      >
        <header className="mb-10 text-center">
          <h1 className="text-4xl md:text-5xl font-extrabold flex items-center justify-center gap-3">
            <Sparkles className="text-indigo-400" /> AI Master
          </h1>
          <p className="text-white/40 mt-3">Choose a topic and get a personalized AI lesson</p>
        </header>

        <div className="grid md:grid-cols-2 gap-4 mb-6">
          <div>
            <label className="text-sm text-white/50 mb-2 block">Category</label>
            <select
              value={categoryId}
              onChange={(e) => setCategoryId(Number(e.target.value) || '')}
              className="w-full p-4 rounded-2xl bg-[#151926] border border-white/10 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">Select category</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>{c.name}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="text-sm text-white/50 mb-2 block">Sub-topic</label>
            <select
              value={subCategoryId}
              onChange={(e) => setSubCategoryId(Number(e.target.value) || '')}
              disabled={!categoryId}
              className="w-full p-4 rounded-2xl bg-[#151926] border border-white/10 focus:outline-none focus:ring-2 focus:ring-indigo-500 disabled:opacity-40"
            >
              <option value="">Select sub-topic</option>
              {subCategories.map((s) => (
                <option key={s.id} value={s.id}>{s.name}</option>
              ))}
            </select>
          </div>
        </div>

        <motion.div whileFocus={{ scale: 1.01 }} className="relative group mb-6">
          <div className="absolute -inset-1 bg-gradient-to-r from-purple-600 to-indigo-600 rounded-2xl blur opacity-20 group-hover:opacity-40 transition duration-1000" />
          <textarea
            className="relative w-full h-36 bg-[#151926] border border-white/10 rounded-2xl p-6 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            placeholder="What do you want to learn today?"
            value={prompt}
            onChange={(e) => setPrompt(e.target.value)}
          />
        </motion.div>

        {error && <p className="text-red-400 text-sm mb-4 text-center">{error}</p>}

        <button
          onClick={handleGenerate}
          disabled={loading}
          className="w-full py-4 rounded-2xl bg-gradient-to-r from-purple-600 to-indigo-600 font-bold hover:shadow-[0_0_20px_rgba(79,70,229,0.5)] transition-all flex items-center justify-center gap-2 disabled:opacity-50"
        >
          {loading ? <Loader2 className="animate-spin" /> : <><Send size={20} /> Generate Lesson</>}
        </button>

        {response && (
          <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            className="mt-8 p-6 rounded-2xl bg-white/5 border border-indigo-500/20"
          >
            <div className="flex items-center gap-2 mb-4 text-indigo-300">
              <BookOpen size={20} />
              <span className="font-semibold">Your AI Lesson</span>
            </div>
            <div className="text-white/80 whitespace-pre-wrap leading-relaxed text-sm md:text-base">
              {response}
            </div>
          </motion.div>
        )}
      </motion.div>
    </Layout>
  );
};

export default Dashboard;
