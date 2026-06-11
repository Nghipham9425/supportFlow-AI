export type KnowledgeArticleCategory = 1 | 2 | 3 | 4 | 5 | 6;

export type KnowledgeArticle = {
  id: string;
  title: string;
  content: string;
  category: KnowledgeArticleCategory;
  createdAt: string;
  updatedAt: string;
};

export type CreateKnowledgeArticleInput = {
  title: string;
  content: string;
  category: KnowledgeArticleCategory;
};

export const knowledgeCategoryLabels: Record<KnowledgeArticleCategory, string> = {
  1: "General",
  2: "Account",
  3: "Billing",
  4: "Technical",
  5: "Refund",
  6: "Product",
};
