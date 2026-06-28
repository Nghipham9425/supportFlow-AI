"use client"

import { Button } from "@/components/ui/button"
import { DatabaseZap } from "lucide-react"
import { toast } from "sonner"
import { knowledgeApi } from "@/lib/api"
import { useMutation, useQueryClient } from "@tanstack/react-query"

export function GenerateEmbeddingsButton({ articleId }: { articleId: string }) {
  const queryClient = useQueryClient()
  const generateEmbeddings = useMutation({
    mutationFn: () => knowledgeApi.generateEmbeddings(articleId),
    onSuccess: (chunks) => {
      queryClient.invalidateQueries({
        queryKey: ["knowledge-article-chunks", articleId],
      })

      toast.success(`Generated embeddings for ${chunks.length} chunk(s)`)
    },
    onError: () => {
      toast.error("Could not generate embeddings")
    },
  })
  return (
    <Button
      variant="outline"
      size="sm"
      onClick={() => generateEmbeddings.mutate()}
      disabled={generateEmbeddings.isPending}
    >
      <DatabaseZap className="size-4" />
      {generateEmbeddings.isPending ? "Generating..." : "Generate embeddings"}
    </Button>
  )
}
