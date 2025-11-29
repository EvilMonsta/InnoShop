import { Routes, Route } from 'react-router-dom';
import { MainLayout } from './shared/layout/MainLayout';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { ProductsPage } from './pages/ProductsPage';
import { MyProductsPage } from './pages/MyProductsPage';
import { ProductEditPage } from './pages/ProductEditPage';
import { ProfilePage } from './pages/ProfilePage';
import { NotFoundPage } from './pages/NotFoundPage';
import { ProtectedRoute } from './shared/auth/ProtectedRoute';
import { ResetPasswordPage } from './pages/ResetPasswordPage';
import { ConfirmEmailPage } from './pages/ConfirmEmailPage';

export function AppRouter() {
  return (
    <MainLayout>
      <Routes>
        <Route path="/" element={<HomePage />} />

        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/reset-password/:token" element={<ResetPasswordPage />} />
        <Route path="/products" element={<ProductsPage />} />
        <Route path="/reset/:token" element={<ResetPasswordPage />} />
        <Route path="/confirm/:token" element={<ConfirmEmailPage />} />

        <Route
          path="/my-products"
          element={
            <ProtectedRoute>
              <MyProductsPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/products/new"
          element={
            <ProtectedRoute>
              <ProductEditPage mode="create" />
            </ProtectedRoute>
          }
        />

        <Route
          path="/products/:id/edit"
          element={
            <ProtectedRoute>
              <ProductEditPage mode="edit" />
            </ProtectedRoute>
          }
        />

        <Route
          path="/profile"
          element={
            <ProtectedRoute>
              <ProfilePage />
            </ProtectedRoute>
          }
        />

        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </MainLayout>
  );
}
