"use client"

import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { knowledgeApi } from "@/lib/api"
import {
  KnowledgeArticle,
  KnowledgeArticleCategory,
  UpdateKnowledgeArticleInput,
} from "@/types/knowledge"
import { useMutation, useQueryClient } from "@tanstack/react-query"
import { Pencil } from "lucide-react"
import { FormEvent, useState } from "react"
import { toast } from "sonner"

export function EditKnowledgeDialog({
  article,
}: {
  article: KnowledgeArticle
}) {
  const [open, setOpen] = useState(false)
  const [form, setForm] = useState<UpdateKnowledgeArticleInput>({
    title: article.title,
    content: article.content,
    category: article.category,
  })

  const queryClient = useQueryClient()

  const updateArticle = useMutation({
    mutationFn: () => knowledgeApi.update(article.id, form),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["knowledge-articles"] })
      setOpen(false)
      toast.success("Knowledge article updated")
    },
    onError: () => toast.error("Could not update article"),
  })

  function updateField<K extends keyof UpdateKnowledgeArticleInput>(
    key: K,
    value: UpdateKnowledgeArticleInput[K],
  ) {
    setForm((current) => ({ ...current, [key]: value }))
  }

  function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault()
    updateArticle.mutate()
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="ghost" size="icon-sm">
          <Pencil className="size-4" />
          <span className="sr-only">Edit article</span>
        </Button>
      </DialogTrigger>

      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle>Edit knowledge article</DialogTitle>
          <DialogDescription>
            Update internal support guidance used by the knowledge base.
          </DialogDescription>
        </DialogHeader>

        <form className="space-y-5" onSubmit={onSubmit}>
          <Input
            placeholder="Article title"
            value={form.title ?? ""}
            onChange={(event) => updateField("title", event.target.value)}
            required
          />

          <Select
            value={String(form.category ?? article.category)}
            onValueChange={(value) =>
              updateField("category", Number(value) as KnowledgeArticleCategory)
            }
          >
            <SelectTrigger className="w-full">
              <SelectValue placeholder="Category" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="1">General</SelectItem>
              <SelectItem value="2">Account</SelectItem>
              <SelectItem value="3">Billing</SelectItem>
              <SelectItem value="4">Technical</SelectItem>
              <SelectItem value="5">Refund</SelectItem>
              <SelectItem value="6">Product</SelectItem>
            </SelectContent>
          </Select>

          <Textarea
            className="min-h-56"
            placeholder="Write the support article content..."
            value={form.content ?? ""}
            onChange={(event) => updateField("content", event.target.value)}
            required
          />

          <DialogFooter>
            <Button
              type="submit"
              className="bg-emerald-600 text-white hover:bg-emerald-700"
              disabled={updateArticle.isPending}
            >
              {updateArticle.isPending ? "Saving..." : "Save changes"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  )
}
