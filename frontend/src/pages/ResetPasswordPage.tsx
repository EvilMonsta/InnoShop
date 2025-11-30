import { useParams, Link, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { confirmPasswordReset } from '../shared/api/users';

const schema = z
  .object({
    password: z.string().min(6, 'Минимум 6 символов'),
    confirmPassword: z.string().min(6, 'Минимум 6 символов'),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: 'Пароли не совпадают',
    path: ['confirmPassword'],
  });

interface ResetPasswordFormValues {
  password: string;
  confirmPassword: string;
}

export function ResetPasswordPage() {
  const { token } = useParams<{ token: string }>();
  const navigate = useNavigate();
  const [statusMessage, setStatusMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<ResetPasswordFormValues>({
    resolver: zodResolver(schema),
  });

  if (!token) {
    return (
      <div>
        <h1>Сброс пароля</h1>
        <p>Некорректная ссылка для сброса пароля.</p>
        <Link to="/login">Перейти к входу</Link>
      </div>
    );
  }

  const onSubmit = async (data: ResetPasswordFormValues) => {
    setStatusMessage(null);
    setErrorMessage(null);
    try {
      await confirmPasswordReset(token, data.password);
      setStatusMessage(
        'Пароль успешно изменён. Теперь вы можете войти с новым паролем.',
      );
    } catch {
      setErrorMessage(
        'Ошибка при смене пароля. Возможно, ссылка устарела или неправильна.',
      );
    }
  };

  return (
    <div>
      <h1>Сброс пароля</h1>
      <p>Введите новый пароль для вашего аккаунта.</p>

      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label>Новый пароль</label>
          <input type="password" {...register('password')} />
          {errors.password && <p>{errors.password.message}</p>}
        </div>

        <div>
          <label>Повторите пароль</label>
          <input type="password" {...register('confirmPassword')} />
          {errors.confirmPassword && <p>{errors.confirmPassword.message}</p>}
        </div>

        <button type="submit" disabled={isSubmitting}>
          Сохранить новый пароль
        </button>
      </form>

      {statusMessage && <p>{statusMessage}</p>}
      {errorMessage && <p>{errorMessage}</p>}

      <p style={{ marginTop: '1rem' }}>
        <Link to="/login">Вернуться на страницу входа</Link>
      </p>
    </div>
  );
}
