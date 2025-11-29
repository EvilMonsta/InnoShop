import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { useAuth } from '../shared/auth/useAuth';
import { deleteProduct, getProducts } from '../shared/api/products';
import type { ProductResponse } from '../shared/api/products';
import { Link } from 'react-router-dom';

export function MyProductsPage() {
  const { user } = useAuth();
  const queryClient = useQueryClient();

  const { data, isLoading, error } = useQuery({
    queryKey: ['my-products', user?.id],
    queryFn: () =>
      getProducts({
        ownerUserId: user?.id,
        page: 1,
        pageSize: 100,
      }),
    enabled: !!user?.id,
  });

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteProduct(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['my-products', user?.id] });
    },
  });

  const items: ProductResponse[] = data?.Items ?? [];

  return (
    <div>
      <h1>Мои товары</h1>

      {isLoading && <p>Загрузка...</p>}
      {error && <p>Ошибка загрузки</p>}

      <table>
        <thead>
          <tr>
            <th>Название</th>
            <th>Цена</th>
            <th>Доступен</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {items.map((p) => (
            <tr key={p.Id}>
              <td>{p.Name}</td>
              <td>{p.Price}</td>
              <td>{p.IsAvailable ? 'Да' : 'Нет'}</td>
              <td>
                <Link to={`/products/${p.Id}/edit`}>Редактировать</Link>
                <button
                  onClick={() => deleteMutation.mutate(p.Id)}
                  style={{ marginLeft: '0.5rem' }}
                >
                  Удалить
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}
