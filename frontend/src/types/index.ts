export interface User {
  id: number;
  name: string;
  email: string;
  isAdmin: boolean;
}

export interface Category {
  id: number;
  name: string;
  subCategories: SubCategory[];
}

export interface SubCategory {
  id: number;
  name: string;
  categoryId: number;
}

export interface PromptItem {
  id: number;
  userId: number;
  userName: string;
  categoryName: string;
  subCategoryName: string;
  promptText: string;
  response: string;
  createdAt: string;
}

export interface AdminUser {
  id: number;
  name: string;
  email: string;
  phone: string;
  isAdmin: boolean;
  promptCount: number;
}
