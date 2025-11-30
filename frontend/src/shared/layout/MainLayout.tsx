import type { ReactNode } from 'react';
import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../auth/useAuth';

export function MainLayout({ children }: { children: ReactNode }) {
  const { user, isAuthenticated, logout } = useAuth();

  return (
    <div className="app">
      <header className="app-header">
        <Link to="/" className="logo">
          InnoShop
        </Link>

        <nav className="nav">
        <NavLink to="/products">Каталог</NavLink>
        {isAuthenticated && <NavLink to="/my-products">Мои товары</NavLink>}
        {isAuthenticated && <NavLink to="/products/new">Создать товар</NavLink>}
        {isAuthenticated && user?.role === 'Admin' && (
            <>
            <NavLink to="/admin/users">Админ: пользователи</NavLink>
            <NavLink to="/admin/products">Админ: товары</NavLink>
            </>
        )}
        </nav>

        <div className="auth-block">
          {isAuthenticated && user ? (
            <>
              <NavLink to="/profile">
                {user.name || user.email || 'Профиль'}
              </NavLink>
              <button onClick={logout}>Выйти</button>
            </>
          ) : (
            <>
              <NavLink to="/login">Войти</NavLink>
              <NavLink to="/register">Регистрация</NavLink>
            </>
          )}
        </div>
      </header>

      <main className="app-content">{children}</main>
    </div>
  );
}
