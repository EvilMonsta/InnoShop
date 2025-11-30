import { useState, type ChangeEvent } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  listUsers,
  deactivateUser,
  reactivateUser,
  deleteUserAccount,
} from '../shared/api/users';
import type {
  UserFilter,
  UserResponse,
} from '../shared/api/users';

const roleOptions = [
  { value: '', label: 'Все роли' },
  { value: 'User', label: 'User' },
  { value: 'Admin', label: 'Admin' },
];

const boolOptions = [
  { value: '', label: 'Все' },
  { value: 'true', label: 'Да' },
  { value: 'false', label: 'Нет' },
];

export function AdminUsersPage() {
  const [filter, setFilter] = useState<UserFilter>({
    page: 1,
    pageSize: 20,
  });

  const queryClient = useQueryClient();

  const { data, isLoading, error } = useQuery({
    queryKey: ['admin-users', filter],
    queryFn: () => listUsers(filter),
  });

  const users: UserResponse[] = data?.Items ?? [];
  const total = data?.Total ?? 0;
  const page = data?.Page ?? filter.page ?? 1;
  const pageSize = data?.PageSize ?? filter.pageSize ?? 20;

  const toggleActiveMutation = useMutation({
    mutationFn: (user: UserResponse) =>
      user.IsActive ? deactivateUser(user.Id) : reactivateUser(user.Id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-users'] });
    },
  });

  const deleteUserMutation = useMutation({
    mutationFn: (id: string) => deleteUserAccount(id),
    onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ['admin-users'] });
    },
  });

  const handleSearchChange = (e: ChangeEvent<HTMLInputElement>) => {
    setFilter((prev) => ({
      ...prev,
      q: e.target.value || undefined,
      page: 1,
    }));
  };

  const handleRoleChange = (e: ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    setFilter((prev) => ({
      ...prev,
      role: value || undefined,
      page: 1,
    }));
  };

  const handleIsActiveChange = (e: ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    setFilter((prev) => ({
      ...prev,
      isActive:
        value === ''
          ? undefined
          : value === 'true'
          ? true
          : false,
      page: 1,
    }));
  };

  const handleEmailConfirmedChange = (e: ChangeEvent<HTMLSelectElement>) => {
    const value = e.target.value;
    setFilter((prev) => ({
      ...prev,
      emailConfirmed:
        value === ''
          ? undefined
          : value === 'true'
          ? true
          : false,
      page: 1,
    }));
  };

  const handleResetFilters = () => {
    setFilter({
      page: 1,
      pageSize: 20,
      q: undefined,
      role: undefined,
      isActive: undefined,
      emailConfirmed: undefined,
    });
  };

  const canGoNext = pageSize > 0 && page * pageSize < total;

  return (
    <div>
      <h1>Админ — пользователи</h1>
      <p>Всего пользователей: {total}</p>

      <div className="filters-row">
        <input
          type="search"
          placeholder="Поиск по имени или email"
          value={filter.q ?? ''}
          onChange={handleSearchChange}
        />

        <label>
          Роль
          <select
            value={filter.role ?? ''}
            onChange={handleRoleChange}
          >
            {roleOptions.map((opt) => (
              <option key={opt.value || 'all'} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
        </label>

        <label>
          Активен
          <select
            value={
              filter.isActive === undefined
                ? ''
                : filter.isActive
                ? 'true'
                : 'false'
            }
            onChange={handleIsActiveChange}
          >
            {boolOptions.map((opt) => (
              <option key={opt.value || 'all'} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
        </label>

        <label>
          Почта подтверждена
          <select
            value={
              filter.emailConfirmed === undefined
                ? ''
                : filter.emailConfirmed
                ? 'true'
                : 'false'
            }
            onChange={handleEmailConfirmedChange}
          >
            {boolOptions.map((opt) => (
              <option key={opt.value || 'all'} value={opt.value}>
                {opt.label}
              </option>
            ))}
          </select>
        </label>

        <button type="button" onClick={handleResetFilters}>
          Сбросить
        </button>
      </div>

      {isLoading && <p>Загрузка...</p>}
      {error && <p>Ошибка загрузки списка пользователей</p>}

      <table>
        <thead>
          <tr>
            <th>Имя</th>
            <th>Email</th>
            <th>Роль</th>
            <th>Активен</th>
            <th>Email подтверждён</th>
            <th>Создан</th>
            <th></th>
          </tr>
        </thead>
        <tbody>
          {users.map((u) => (
            <tr key={u.Id}>
              <td>{u.Name}</td>
              <td>{u.Email}</td>
              <td>{u.Role}</td>
              <td>{u.IsActive ? 'Да' : 'Нет'}</td>
              <td>{u.EmailConfirmed ? 'Да' : 'Нет'}</td>
              <td>
                {u.CreatedAt
                  ? new Date(u.CreatedAt).toLocaleString()
                  : ''}
              </td>
                <td>
                <button
                    type="button"
                    onClick={() => toggleActiveMutation.mutate(u)}
                    className={u.IsActive ? 'danger' : 'primary'}
                >
                    {u.IsActive ? 'Деактивировать' : 'Активировать'}
                </button>
                <button
                    type="button"
                    onClick={() => {
                    if (
                        window.confirm(
                        `Удалить пользователя ${u.Email ?? u.Name ?? u.Id}?`,
                        )
                    ) {
                        deleteUserMutation.mutate(u.Id);
                    }
                    }}
                    style={{ marginLeft: '0.4rem' }}
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
