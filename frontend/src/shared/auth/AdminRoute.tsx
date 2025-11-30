import type { ReactNode } from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './useAuth';

export function AdminRoute({ children }: { children: ReactNode }) {
  const { user, isAuthenticated } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (user?.role !== 'Admin') {
    return <Navigate to="/" replace />;
  }

  return <>{children}</>;
}
