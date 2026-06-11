import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { Calendar, History as HistoryIcon } from 'lucide-react';
import api from './api';
import Layout from './components/Layout';
import { PromptItem } from './types';

const History = () => {
  const [history, setHistory] = useState<PromptItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    api.get<PromptItem[]>('/Prompts/history')
      .then((res) => setHistory(res.data))
      .finally(() => setLoading(false));
  }, []);

  return (
    <Layout>
      <div className="p-6 md:p-12 max-w-4xl mx-auto">
        <div className="flex items-center gap-3 mb-8">
          <HistoryIcon className="text-indigo-400" size={32} />
          <div>
            <h2 className="text-3xl font-bold">Your Learning History</h2>
            <p className="text-white/40 text-sm">All your AI-generated lessons</p>
          </div>
        </div>

        {loading && <p className="text-white/40 text-center py-12">Loading history...</p>}

        {!loading && history.length === 0 && (
          <div className="text-center py-16 rounded-2xl border border-white/10 bg-white/5">
            <p className="text-white/40">No lessons yet. Go to Dashboard and generate your first one!</p>
          </div>
        )}

        <div className="space-y-4">
          {history.map((item) => (
            <motion.div
              initial={{ opacity: 0, y: 8 }}
              animate={{ opacity: 1, y: 0 }}
              key={item.id}
              className="bg-white/5 p-6 rounded-2xl border border-white/10 hover:border-indigo-500/30 transition-all"
            >
              <div className="flex flex-wrap items-center gap-3 mb-3">
                <span className="px-3 py-1 rounded-full text-xs bg-indigo-500/20 text-indigo-300">
                  {item.categoryName} / {item.subCategoryName}
                </span>
                <span className="flex items-center gap-1 text-xs text-white/30">
                  <Calendar size={12} />
                  {new Date(item.createdAt).toLocaleString()}
                </span>
              </div>
              <p className="font-semibold text-indigo-300 mb-2">{item.promptText}</p>
              <p className="text-white/70 text-sm whitespace-pre-wrap leading-relaxed line-clamp-6">
                {item.response}
              </p>
            </motion.div>
          ))}
        </div>
      </div>
    </Layout>
  );
};

export default History;
