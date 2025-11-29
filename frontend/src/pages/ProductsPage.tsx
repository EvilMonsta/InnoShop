import { useState } from 'react';
import type { ChangeEvent } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getProducts } from '../shared/api/products';
import type {
  ProductFilter,
  ProductResponse,
} from '../shared/api/products';

export function ProductsPage() {
  const [filter, setFilter] = useState<ProductFilter>({
    page: 1,
    pageSize: 20,
    onlyAvailable: true,
  });

  const { data, isLoading, error } = useQuery({
    queryKey: ['products', filter],
    queryFn: () => getProducts(filter),
  });

  const items: ProductResponse[] = data?.Items ?? [];

  const handleSearchChange = (e: ChangeEvent<HTMLInputElement>) => {
    setFilter((prev) => ({
      ...prev,
      q: e.target.value || undefined,
      page: 1,
    }));
  };

  const handleMinPriceChange = (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setFilter((prev) => ({
      ...prev,
      minPrice: value === '' ? undefined : Number(value),
      page: 1,
    }));
  };

  const handleMaxPriceChange = (e: ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    setFilter((prev) => ({
      ...prev,
      maxPrice: value === '' ? undefined : Number(value),
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
      onlyAvailable: true,
    });
  };

  return (
    <div>
      <h1>Каталог товаров</h1>

      <div className="filters-row">
        <input
          type="search"
          placeholder="Поиск по названию / описанию"
          value={filter.q ?? ''}
          onChange={handleSearchChange}
        />

        <label>
          Мин. цена
          <input
            type="number"
            step="0.01"
            value={filter.minPrice ?? ''}
            onChange={handleMinPriceChange}
          />
        </label>

        <label>
          Макс. цена
          <input
            type="number"
            step="0.01"
            value={filter.maxPrice ?? ''}
            onChange={handleMaxPriceChange}
          />
        </label>

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
      {error && <p>Ошибка загрузки</p>}

      <table>
        <thead>
          <tr>
            <th>Название</th>
            <th>Описание</th>
            <th>Цена</th>
            <th>Доступен</th>
          </tr>
        </thead>
        <tbody>
          {items.map((p) => (
            <tr key={p.Id}>
              <td>{p.Name}</td>
              <td>{p.Description}</td>
              <td>{p.Price}</td>
              <td>{p.IsAvailable ? 'Да' : 'Нет'}</td>
            </tr>
          ))}
        </tbody>
      </table>

      <div className="pagination">
        <button
          type="button"
          disabled={(filter.page ?? 1) <= 1}
          onClick={() =>
            setFilter((prev) => ({
              ...prev,
              page: (prev.page ?? 1) - 1,
            }))
          }
        >
          Назад
        </button>

        <span>Страница {filter.page}</span>

        <button
          type="button"
          disabled={items.length < (filter.pageSize ?? 20)}
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
