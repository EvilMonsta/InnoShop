import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useAuth } from '../shared/auth/useAuth';
import { useLocation, useNavigate, Link } from 'react-router-dom';
import type { LoginFormValues } from '../shared/api/users';
import { requestPasswordReset } from '../shared/api/users';
import axios from 'axios';

const schema = z.object({
  email: z.string().email('Введите корректный email'),
  password: z.string().min(6, 'Минимум 6 символов'),
});

export function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation() as any;

  const [loginError, setLoginError] = useState<string | null>(null);
  const [resetMessage, setResetMessage] = useState<string | null>(null);
  const [isResetSending, setIsResetSending] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    getValues,
    trigger,
  } = useForm<LoginFormValues>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: LoginFormValues) => {
    setLoginError(null);
    setResetMessage(null);

    try {
      await login(data);
      const redirectTo = location.state?.from?.pathname || '/products';
      navigate(redirectTo, { replace: true });
    } catch (err) {
      if (axios.isAxiosError(err) && err.response) {
        if (err.response.status === 401) {
          setLoginError('Неверный email или пароль.');
        } else if (err.response.status === 403) {
          setLoginError('Аккаунт деактивирован. Обратитесь к администратору.');
        } else {
          setLoginError('Ошибка входа. Попробуйте ещё раз.');
        }
      } else {
        setLoginError('Ошибка входа. Попробуйте ещё раз.');
      }
    }
  };

  const handleForgotPassword = async () => {
    setResetMessage(null);
    setLoginError(null);

    const email = getValues('email');
    if (!email) {
      setResetMessage('Введите email, чтобы отправить письмо для сброса пароля.');
      return;
    }

    const valid = await trigger('email');
    if (!valid) return;

    setIsResetSending(true);
    try {
      await requestPasswordReset(email);
      setResetMessage('Письмо для сброса пароля отправлено. Проверьте почту.');
    } catch {
      setResetMessage('Не удалось отправить письмо для сброса пароля.');
    } finally {
      setIsResetSending(false);
    }
  };

  return (
    <div>
      <h1>Вход</h1>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label>Email</label>
          <input type="email" {...register('email')} />
          {errors.email && <p>{errors.email.message}</p>}
        </div>

        <div>
          <label>Пароль</label>
          <input type="password" {...register('password')} />
          {errors.password && <p>{errors.password.message}</p>}
        </div>

        {loginError && <p>{loginError}</p>}

        <button type="submit" disabled={isSubmitting}>
          Войти
        </button>

      <div style={{ marginTop: '0.75rem' }}>
        <p style={{ margin: 0, fontSize: '1rem' }}>
          Забыли пароль?{' '}
          <button
            type="button"
            className="link-button"
            onClick={handleForgotPassword}
            disabled={isResetSending}
          >
            Сбросить пароль
          </button>
        </p>

        {resetMessage && (
          <p
            style={{
              marginTop: '0.35rem',
              color: resetMessage.toLowerCase().includes('не удалось')
                ? 'var(--danger)'
                : 'green',
            }}
          >
            {resetMessage}
          </p>
        )}
      </div>

      </form>

      <p style={{ marginTop: '1rem' }}>
        Нет аккаунта? <Link to="/register">Зарегистрироваться</Link>
      </p>
    </div>
  );
}
