"use client";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { knowledgeApi } from "@/lib/api";
import {
  CreateKnowledgeArticleInput,
  KnowledgeArticleCategory,
} from "@/types/knowledge";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Plus } from "lucide-react";
import { FormEvent, useState } from "react";
import { toast } from "sonner";

const initialForm: CreateKnowledgeArticleInput = {
  title: "",
  content: "",
  category: 1,
};

export function CreateKnowledgeDialog() {
  const [open, setOpen] = useState(false);
  const [form, setForm] = useState<CreateKnowledgeArticleInput>(initialForm);
  const queryClient = useQueryClient();

  const createArticle = useMutation({
    mutationFn: knowledgeApi.create,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["knowledge-articles"] });
      setForm(initialForm);
      setOpen(false);
      toast.success("Knowledge article created");
    },
    onError: () => toast.error("Could not create article"),
  });

  function updateField<K extends keyof CreateKnowledgeArticleInput>(
    key: K,
    value: CreateKnowledgeArticleInput[K],
  ) {
    setForm((current) => ({ ...current, [key]: value }));
  }

  function onSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    createArticle.mutate(form);
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="bg-emerald-600 text-white hover:bg-emerald-700">
          <Plus className="size-4" />
          New article
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-2xl">
        <DialogHeader>
          <DialogTitle>Create knowledge article</DialogTitle>
          <DialogDescription>
            Add internal support guidance that future AI retrieval can use when
            drafting ticket responses.
          </DialogDescription>
        </DialogHeader>
        <form className="space-y-5" onSubmit={onSubmit}>
          <Input
            placeholder="Article title"
            value={form.title}
            onChange={(event) => updateField("title", event.target.value)}
            required
          />
          <Select
            value={String(form.category)}
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
            value={form.content}
            onChange={(event) => updateField("content", event.target.value)}
            required
          />
          <DialogFooter>
            <Button
              type="submit"
              className="bg-emerald-600 text-white hover:bg-emerald-700"
              disabled={createArticle.isPending}
            >
              {createArticle.isPending ? "Creating..." : "Create article"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
