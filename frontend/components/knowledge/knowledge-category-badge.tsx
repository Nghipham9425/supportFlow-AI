import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import {
  KnowledgeArticleCategory,
  knowledgeCategoryLabels,
} from "@/types/knowledge";

const categoryColors: Record<KnowledgeArticleCategory, string> = {
  1: "border-slate-200 bg-slate-100 text-slate-700",
  2: "border-violet-200 bg-violet-50 text-violet-700",
  3: "border-amber-200 bg-amber-50 text-amber-700",
  4: "border-sky-200 bg-sky-50 text-sky-700",
  5: "border-rose-200 bg-rose-50 text-rose-700",
  6: "border-emerald-200 bg-emerald-50 text-emerald-700",
};

export function KnowledgeCategoryBadge({
  category,
}: {
  category: KnowledgeArticleCategory;
}) {
  return (
    <Badge
      className={cn("rounded-full border px-2.5 py-1", categoryColors[category])}
    >
      {knowledgeCategoryLabels[category]}
    </Badge>
  );
}
