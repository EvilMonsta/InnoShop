import { productsApi } from './http';

export interface ProductResponse {
  Id: string;
  OwnerUserId: string;
  Name: string | null;
  Description: string | null;
  Price: number;
  IsAvailable: boolean;
  CreatedAt: string | null;
}

export interface ProductPagedResult {
  Items: ProductResponse[] | null;
  Total: number;
  Page: number;
  PageSize: number;
}

export interface ProductFilter {
  q?: string;
  minPrice?: number;
  maxPrice?: number;
  onlyAvailable?: boolean;
  ownerUserId?: string;
  page?: number;
  pageSize?: number;
}

export interface ProductFormValues {
  name: string;
  description: string;
  price: number;
  isAvailable: boolean;
}

export async function getProducts(
  filter: ProductFilter,
): Promise<ProductPagedResult> {
  const res = await productsApi.get<ProductPagedResult>('/products', {
    params: {
      q: filter.q,
      minPrice: filter.minPrice,
      maxPrice: filter.maxPrice,
      onlyAvailable: filter.onlyAvailable,
      ownerUserId: filter.ownerUserId,
      page: filter.page ?? 1,
      pageSize: filter.pageSize ?? 50,
    },
  });
  return res.data;
}

export async function getProductById(id: string): Promise<ProductResponse> {
  const res = await productsApi.get<ProductResponse>(`/products/${id}`);
  return res.data;
}

export async function createProduct(data: ProductFormValues): Promise<ProductResponse> {
  const res = await productsApi.post<ProductResponse>('/products', {
    Name: data.name,
    Description: data.description,
    Price: data.price,
    IsAvailable: data.isAvailable,
  });
  return res.data;
}

export async function updateProduct(
  id: string,
  data: ProductFormValues,
): Promise<void> {
  await productsApi.put(`/products/${id}`, {
    Name: data.name,
    Description: data.description,
    Price: data.price,
    IsAvailable: data.isAvailable,
  });
}

export async function deleteProduct(id: string): Promise<void> {
  await productsApi.delete(`/products/${id}`);
}

export async function getProductsAdmin(
  filter: ProductFilter,
): Promise<ProductPagedResult> {
  const params: Record<string, unknown> = {
    q: filter.q,
    minPrice: filter.minPrice,
    maxPrice: filter.maxPrice,
    ownerUserId: filter.ownerUserId,
    page: filter.page ?? 1,
    pageSize: filter.pageSize ?? 20,
  };

  if (filter.onlyAvailable !== undefined) {
    params.onlyAvailable = filter.onlyAvailable;
  }

  const res = await productsApi.get<ProductPagedResult>('/products', {
    params,
  });
  return res.data;
}
