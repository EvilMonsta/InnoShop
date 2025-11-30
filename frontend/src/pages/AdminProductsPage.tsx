import { useState, type ChangeEvent } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getProductsAdmin, deleteProduct } from '../shared/api/products';
import type {
  ProductFilter,
  ProductResponse,
} from '../shared/api/products';
import { Link } from 'react-router-dom';

export function AdminProductsPage() {
  const [filter, setFilter] = useState<ProductFilter>({
    page: 1,
    pageSize: 20,
  });

  const queryClient = useQueryClient();

  const { data, isLoading, error } = useQuery({
    queryKey: ['admin-products', filter],
    queryFn: () => getProductsAdmin(filter),
  });

  const items: ProductResponse[] = data?.Items ?? [];
  const total = data?.Total ?? 0;
  const page = data?.Page ?? filter.page ?? 1;
  const pageSize = data?.PageSize ?? filter.pageSize ?? 20;

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteProduct(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-products'] });
      queryClient.invalidateQueries({ queryKey: ['products'] });
      queryClient.invalidateQueries({ queryKey: ['my-products'] });
    },
  });

  const handleSearchChange = (e: ChangeEvent<HTMLInputElement>) => {
    setFilter((prev) => ({
      ...prev,
      q: e.target.value || undefined,
      page: 1,
    }));
  };

  const handleOwnerChange = (e: ChangeEvent<HTMLInputElement>) => {
    setFilter((prev) => ({
      ...prev,
      ownerUserId: e.target.value || undefined,
      page: 1,
    }));
  };

  const handleOnlyAvailableChange = (e: ChangeEvent<HTMLInputElement>) => {
    setFilter((prev) => ({
      ...prev,
      onlyAvailable: e.target.checked || undefined,
      page: 1,
    }));
  };

  const handleResetFilters = () => {
    setFilter({
      page: 1,
      pageSize: 20,
      q: undefined,
      minPrice: undefined,
      maxPrice: undefined,
      onlyAvailable: undefined,
      ownerUserId: undefined,
    });
  };

  const canGoNext = pageSize > 0 && page * pageSize < total;

  return (
    <div>
      <h1>Админ — товары</h1>
      <p>Всего товаров: {total}</p>

      <div className="filters-row">
        <input
          type="search"
          placeholder="Поиск по названию / описанию"
          value={filter.q ?? ''}
          onChange={handleSearchChange}
        />

        <input
          type="text"
          placeholder="ID владельца (UserId)"
          value={filter.ownerUserId ?? ''}
          onChange={handleOwnerChange}
        />

        <label>
          <input
            type="checkbox"
            checked={!!filter.onlyAvailable}
            onChange={handleOnlyAvailableChange}
          />
          Только доступные
        </label>

        <button type="button" onClick={handleResetFilters}>
          Сбросить
        </button>
      </div>

      {isLoading && <p>Загрузка...</p>}
      {error && <p>Ошибка загрузки товаров</p>}

      <table>
        <thead>
          <tr>
            <th>Название</th>
            <th>Описание</th>
            <th>Цена</th>
            <th>Доступен</th>
            <th>Владелец (UserId)</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {items.map((p) => (
            <tr key={p.Id}>
              <td>{p.Name}</td>
              <td>{p.Description}</td>
              <td>{p.Price}</td>
              <td>{p.IsAvailable ? 'Да' : 'Нет'}</td>
              <td>{p.OwnerUserId}</td>
              <td>
                <Link to={`/products/${p.Id}/edit`}>Редактировать</Link>
                <button
                  type="button"
                  onClick={() => deleteMutation.mutate(p.Id)}
                >
                  Удалить
                </button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="pagination">
        <button
          type="button"
          disabled={page <= 1}
          onClick={() =>
            setFilter((prev) => ({
              ...prev,
              page: (prev.page ?? 1) - 1,
            }))
          }
        >
          Назад
        </button>
        <span>
          Страница {page} /{' '}
          {pageSize > 0 ? Math.max(1, Math.ceil(total / pageSize)) : 1}
        </span>
        <button
          type="button"
          disabled={!canGoNext}
          onClick={() =>
            setFilter((prev) => ({
              ...prev,
              page: (prev.page ?? 1) + 1,
            }))
          }
        >
          Вперёд
        </button>
      </div>
    </div>
  );
}
