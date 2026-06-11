"use client";

import { KnowledgeArticle } from "@/types/knowledge";
import { BookOpen } from "lucide-react";
import { KnowledgeArticleCard } from "./knowledge-article-card";

export function KnowledgeList({ articles }: { articles: KnowledgeArticle[] }) {
  if (articles.length === 0) {
    return (
      <div className="flex min-h-72 items-center justify-center rounded-xl border border-slate-200 bg-white">
        <div className="text-center">
          <div className="mx-auto mb-3 flex size-11 items-center justify-center rounded-full bg-emerald-50 text-emerald-700">
            <BookOpen className="size-5" />
          </div>
          <p className="text-sm font-medium">No knowledge articles found</p>
          <p className="mt-1 text-sm text-slate-500">
            Create articles that future AI retrieval can cite in support replies.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
      {articles.map((article) => (
        <KnowledgeArticleCard key={article.id} article={article} />
      ))}
    </div>
  );
}
