import { useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { createProduct, getProductById, updateProduct } from '../shared/api/products';
import type { ProductFormValues } from '../shared/api/products';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';

const schema = z.object({
  name: z.string().min(2, 'Минимум 2 символа'),
  description: z.string().max(1000, 'Слишком длинное описание'),
  price: z
    .number()
    .positive('Цена должна быть положительной'),
  isAvailable: z.boolean(),
});

interface ProductEditPageProps {
  mode: 'create' | 'edit';
}

export function ProductEditPage({ mode }: ProductEditPageProps) {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const { data: product } = useQuery({
    queryKey: ['product', id],
    queryFn: () => getProductById(id!),
    enabled: mode === 'edit' && !!id,
  });

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<ProductFormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: '',
      description: '',
      price: 0,
      isAvailable: true,
    },
  });

  useEffect(() => {
    if (product) {
      setValue('name', product.Name ?? '');
      setValue('description', product.Description ?? '');
      setValue('price', product.Price);
      setValue('isAvailable', product.IsAvailable);
    }
  }, [product, setValue]);

  const createMutation = useMutation({
    mutationFn: (data: ProductFormValues) => createProduct(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      queryClient.invalidateQueries({ queryKey: ['my-products'] });
      navigate('/my-products');
    },
  });

  const updateMutation = useMutation({
    mutationFn: (data: ProductFormValues) => updateProduct(id!, data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['products'] });
      queryClient.invalidateQueries({ queryKey: ['my-products'] });
      navigate('/my-products');
    },
  });

  const onSubmit = async (data: ProductFormValues) => {
    if (mode === 'create') {
      await createMutation.mutateAsync(data);
    } else {
      await updateMutation.mutateAsync(data);
    }
  };

  return (
    <div>
      <h1>{mode === 'create' ? 'Создать товар' : 'Редактировать товар'}</h1>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label>Название</label>
          <input {...register('name')} />
          {errors.name && <p>{errors.name.message}</p>}
        </div>

        <div>
          <label>Описание</label>
          <textarea rows={3} {...register('description')} />
          {errors.description && <p>{errors.description.message as string}</p>}
        </div>

        <div>
          <label>Цена</label>
          <input
            type="number"
            step="0.01"
            {...register('price', { valueAsNumber: true })}
          />
          {errors.price && <p>{errors.price.message}</p>}
        </div>

        <div>
          <label>
            <input type="checkbox" {...register('isAvailable')} />
            Доступен для покупки
          </label>
        </div>

        <button type="submit" disabled={isSubmitting}>
          {mode === 'create' ? 'Создать' : 'Сохранить'}
        </button>
      </form>
    </div>
  );
}
