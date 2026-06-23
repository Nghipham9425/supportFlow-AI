"use client"

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogMedia,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { knowledgeApi } from "@/lib/api"
import { KnowledgeArticle } from "@/types/knowledge"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { AlertTriangle, BookOpen, Trash2 } from "lucide-react"
import { toast } from "sonner"
import { EditKnowledgeDialog } from "./edit-knowledge-dialog"
import { KnowledgeCategoryBadge } from "./knowledge-category-badge"
import { RegenerateChunksButton } from "./regenerate-chunks-button"
import { ViewChunksDialog } from "./view-chunks-dialog"
import { GenerateEmbeddingsButton } from "./generate-embeddings-button"

export function KnowledgeArticleCard({
  article,
}: {
  article: KnowledgeArticle
}) {
  const queryClient = useQueryClient()

  const deleteArticle = useMutation({
    mutationFn: knowledgeApi.remove,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["knowledge-articles"] })
      toast.success("Knowledge article deleted")
    },
    onError: () => toast.error("Could not delete article"),
  })

  return (
    <Card className="border border-slate-200 bg-white shadow-sm transition-shadow hover:shadow-md">
      <CardHeader>
        <div className="min-w-0 space-y-4">
          <div className="flex min-w-0 flex-wrap items-center gap-2">
            <span className="flex size-8 shrink-0 items-center justify-center rounded-md bg-emerald-50 text-emerald-700">
              <BookOpen className="size-4" />
            </span>
            <KnowledgeCategoryBadge category={article.category} />
            {article.isAiReady ? (
              <span className="w-fit rounded-full border border-emerald-200 bg-emerald-50 px-2.5 py-1 text-xs font-medium text-emerald-700">
                AI ready - {article.chunkCount} chunks
              </span>
            ) : (
              <span className="w-fit rounded-full border border-slate-200 bg-slate-50 px-2.5 py-1 text-xs font-medium text-slate-600">
                Not prepared
              </span>
            )}
          </div>
          <CardTitle className="line-clamp-2 text-base leading-6">
            {article.title}
          </CardTitle>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <p className="line-clamp-4 text-sm leading-6 text-slate-600">
          {article.content}
        </p>
        <div className="border-t border-slate-100 pt-3 text-xs text-slate-500">
          Updated{" "}
          {new Intl.DateTimeFormat("en", {
            month: "short",
            day: "numeric",
            hour: "2-digit",
            minute: "2-digit",
          }).format(new Date(article.updatedAt))}
        </div>
        <div className="flex flex-wrap items-center gap-2 border-t border-slate-100 pt-4">
          <RegenerateChunksButton articleId={article.id} />
          <GenerateEmbeddingsButton />
          <ViewChunksDialog articleId={article.id} />
          <EditKnowledgeDialog article={article} />
          <AlertDialog>
            <AlertDialogTrigger asChild>
              <Button
                variant="ghost"
                size="icon-sm"
                disabled={deleteArticle.isPending}
              >
                <Trash2 className="size-4" />
                <span className="sr-only">Delete article</span>
              </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogMedia className="bg-rose-50 text-rose-700">
                  <AlertTriangle className="size-5" />
                </AlertDialogMedia>
                <AlertDialogTitle>Delete article?</AlertDialogTitle>
                <AlertDialogDescription>
                  This will permanently delete{" "}
                  <span className="font-medium text-foreground">
                    {article.title}
                  </span>
                  . Future retrieval will not be able to use this content.
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel>Cancel</AlertDialogCancel>
                <AlertDialogAction
                  variant="destructive"
                  onClick={() => deleteArticle.mutate(article.id)}
                >
                  Delete article
                </AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        </div>
      </CardContent>
    </Card>
  )
}
