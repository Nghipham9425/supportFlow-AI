"use client";

import { CreateKnowledgeDialog } from "@/components/knowledge/create-knowledge-dialog";
import { KnowledgeList } from "@/components/knowledge/knowledge-list";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Skeleton } from "@/components/ui/skeleton";
import { knowledgeApi } from "@/lib/api";
import {
  KnowledgeArticleCategory,
  knowledgeCategoryLabels,
} from "@/types/knowledge";
import { useQuery } from "@tanstack/react-query";
import { BookOpen, Layers3, Search, Sparkles } from "lucide-react";
import { useMemo, useState } from "react";

type CategoryFilter = "all" | `${KnowledgeArticleCategory}`;

export default function KnowledgePage() {
  const [search, setSearch] = useState("");
  const [category, setCategory] = useState<CategoryFilter>("all");

  const {
    data: articles = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["knowledge-articles"],
    queryFn: knowledgeApi.list,
  });

  const filteredArticles = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();

    return articles.filter((article) => {
      const matchesCategory =
        category === "all" || article.category === Number(category);

      const matchesSearch =
        normalizedSearch.length === 0 ||
        article.title.toLowerCase().includes(normalizedSearch) ||
        article.content.toLowerCase().includes(normalizedSearch);

      return matchesCategory && matchesSearch;
    });
  }, [articles, category, search]);

  return (
    <div className="space-y-7">
      <div className="flex flex-col gap-4 md:flex-row md:items-start md:justify-between">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">
            Knowledge Base
          </h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Manage internal support articles that will power future RAG
            retrieval.
          </p>
        </div>
        <CreateKnowledgeDialog />
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <MetricCard
          icon={BookOpen}
          label="Articles"
          value={articles.length}
          accent="text-emerald-700 bg-emerald-50"
        />
        <MetricCard
          icon={Layers3}
          label="Categories used"
          value={new Set(articles.map((article) => article.category)).size}
          accent="text-sky-700 bg-sky-50"
        />
        <MetricCard
          icon={Sparkles}
          label="Ready for RAG"
          value={articles.length}
          accent="text-violet-700 bg-violet-50"
        />
      </div>

      <div className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-center lg:justify-between">
          <div className="relative w-full lg:w-96">
            <Search className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-slate-400" />
            <Input
              className="h-10 rounded-md border border-slate-200 bg-white pl-10 pr-3 shadow-sm focus-visible:border-emerald-500"
              placeholder="Search articles..."
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />
          </div>
          <Select
            value={category}
            onValueChange={(value) => setCategory(value as CategoryFilter)}
          >
            <SelectTrigger className="h-10 w-full border-slate-200 bg-white shadow-sm lg:w-56">
              <SelectValue placeholder="Category" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All categories</SelectItem>
              {Object.entries(knowledgeCategoryLabels).map(([value, label]) => (
                <SelectItem key={value} value={value}>
                  {label}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      {isLoading ? (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          <Skeleton className="h-64 w-full" />
          <Skeleton className="h-64 w-full" />
          <Skeleton className="h-64 w-full" />
        </div>
      ) : isError ? (
        <div className="rounded-xl border border-rose-200 bg-rose-50 p-8 text-sm text-rose-700">
          Could not load knowledge articles. Check that the ASP.NET API is
          running on port 5059.
        </div>
      ) : (
        <KnowledgeList articles={filteredArticles} />
      )}
    </div>
  );
}

function MetricCard({
  icon: Icon,
  label,
  value,
  accent,
}: {
  icon: React.ComponentType<{ className?: string }>;
  label: string;
  value: number;
  accent: string;
}) {
  return (
    <Card className="border border-slate-200 bg-white shadow-sm">
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">
          {label}
        </CardTitle>
        <span className={`flex size-8 items-center justify-center rounded-md ${accent}`}>
          <Icon className="size-4" />
        </span>
      </CardHeader>
      <CardContent>
        <p className="text-2xl font-semibold">{value}</p>
      </CardContent>
    </Card>
  );
}
