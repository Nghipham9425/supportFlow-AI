"use client"

import { Button } from "@/components/ui/button"
import { knowledgeApi } from "@/lib/api"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { Sparkles } from "lucide-react"
import { toast } from "sonner"

export function RegenerateChunksButton({ articleId }: { articleId: string }) {
  const queryClient = useQueryClient()

  const regenerateChunks = useMutation({
    mutationFn: () => knowledgeApi.regenerateChunks(articleId),
    onSuccess: (chunks) => {
      queryClient.invalidateQueries({
        queryKey: ["knowledge-article-chunks", articleId],
      })
      queryClient.invalidateQueries({ queryKey: ["knowledge-articles"] })

      toast.success(`Prepared ${chunks.length} chunk(s) for AI retrieval`)
    },
    onError: () => {
      toast.error("Could not prepare article for AI")
    },
  })

  return (
    <Button
      variant="outline"
      size="sm"
      onClick={() => regenerateChunks.mutate()}
      disabled={regenerateChunks.isPending}
    >
      <Sparkles className="size-4" />
      {regenerateChunks.isPending ? "Preparing..." : "Prepare for AI"}
    </Button>
  )
}
