"use client"

import { Button } from "@/components/ui/button"
import { DatabaseZap } from "lucide-react"
import { toast } from "sonner"

export function GenerateEmbeddingsButton() {
  return (
    <Button
      variant="outline"
      size="sm"
      onClick={() => {
        toast.info("Embedding generation will be added next.")
      }}
    >
      <DatabaseZap className="size-4" />
      Generate embeddings
    </Button>
  )
}
