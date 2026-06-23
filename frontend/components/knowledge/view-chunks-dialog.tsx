"use client"

import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Skeleton } from "@/components/ui/skeleton"
import { knowledgeApi } from "@/lib/api"
import { useQuery } from "@tanstack/react-query"
import { ListTree } from "lucide-react"
import { useState } from "react"

export function ViewChunksDialog({ articleId }: { articleId: string }) {
  const [open, setOpen] = useState(false)

  const {
    data: chunks = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["knowledge-article-chunks", articleId],
    queryFn: () => knowledgeApi.getChunks(articleId),
    enabled: open,
  })

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline" size="sm">
          <ListTree className="size-4" />
          View chunks
        </Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-3xl">
        <DialogHeader>
          <DialogTitle>Knowledge chunks</DialogTitle>
          <DialogDescription>
            These chunks are the smaller text blocks that future vector search
            will use for AI retrieval.
          </DialogDescription>
        </DialogHeader>

        <div className="max-h-[520px] space-y-3 overflow-y-auto pr-1">
          {isLoading ? (
            <>
              <Skeleton className="h-24 w-full" />
              <Skeleton className="h-24 w-full" />
              <Skeleton className="h-24 w-full" />
            </>
          ) : isError ? (
            <div className="rounded-lg border border-rose-200 bg-rose-50 p-4 text-sm text-rose-700">
              Could not load chunks.
            </div>
          ) : chunks.length === 0 ? (
            <div className="rounded-lg border border-slate-200 bg-slate-50 p-6 text-center">
              <p className="text-sm font-medium">No chunks generated yet</p>
              <p className="mt-1 text-sm text-slate-500">
                Prepare this article for AI first to generate chunks.
              </p>
            </div>
          ) : (
            chunks.map((chunk) =>
              chunks.map((chunk) => (
                <div
                  key={chunk.id}
                  className="rounded-lg border border-slate-200 bg-white p-4"
                >
                  <div className="mb-3 flex flex-wrap items-center justify-between gap-2">
                    <div className="text-xs font-medium uppercase tracking-wide text-slate-500">
                      Chunk {chunk.chunkIndex + 1}
                    </div>

                    {chunk.isEmbedded ? (
                      <span className="rounded-full border border-emerald-200 bg-emerald-50 px-2.5 py-1 text-xs font-medium text-emerald-700">
                        Embedded
                      </span>
                    ) : (
                      <span className="rounded-full border border-amber-200 bg-amber-50 px-2.5 py-1 text-xs font-medium text-amber-700">
                        Pending embedding
                      </span>
                    )}
                  </div>

                  {chunk.embeddedAt && (
                    <p className="mb-2 text-xs text-slate-500">
                      Embedded {new Date(chunk.embeddedAt).toLocaleString()}
                    </p>
                  )}

                  <p className="whitespace-pre-wrap text-sm leading-6 text-slate-700">
                    {chunk.content}
                  </p>
                </div>
              )),
            )
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
