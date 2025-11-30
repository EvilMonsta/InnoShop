import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { useNavigate, Link } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../shared/auth/useAuth';
import type { RegisterFormValues } from '../shared/api/users';

const schema = z.object({
  name: z.string().min(2, 'Минимум 2 символа'),
  email: z.string().email('Введите корректный email'),
  password: z.string().min(6, 'Минимум 6 символов'),
});

export function RegisterPage() {
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();
  const [submitError, setSubmitError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<RegisterFormValues>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: RegisterFormValues) => {
    setSubmitError(null);

    try {
      await registerUser(data);
      navigate('/login', { replace: true });
    } catch (err) {
      if (axios.isAxiosError(err) && err.response) {
        if (err.response.status === 409) {
          setError('email', {
            type: 'manual',
            message: 'Пользователь с таким email уже существует.',
          });
          return;
        }
      }

      setSubmitError('Ошибка регистрации. Попробуйте ещё раз.');
    }
  };

  return (
    <div>
      <h1>Регистрация</h1>

      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label>Имя</label>
          <input {...register('name')} />
          {errors.name && <p>{errors.name.message}</p>}
        </div>

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

        {submitError && <p>{submitError}</p>}

        <button type="submit" disabled={isSubmitting}>
          Зарегистрироваться
        </button>
      </form>

      <p style={{ marginTop: '1rem' }}>
        Уже есть аккаунт? <Link to="/login">Войти</Link>
      </p>
    </div>
  );
}
