import { useForm } from 'react-hook-form';
import { useState } from 'react';
import { useAuth } from '../shared/auth/useAuth';
import {
  updateUserName,
  deactivateUser,
  reactivateUser,
  requestEmailConfirmation,
  requestPasswordReset,
} from '../shared/api/users';

interface ProfileFormValues {
  name: string;
}

export function ProfilePage() {
  const { user, reloadUser } = useAuth();
  const [isSaving, setIsSaving] = useState(false);
  const [isToggling, setIsToggling] = useState(false);
  const [isEmailSending, setIsEmailSending] = useState(false);
  const [isResetSending, setIsResetSending] = useState(false);
  const [emailMessage, setEmailMessage] = useState<string | null>(null);
  const [resetMessage, setResetMessage] = useState<string | null>(null);

  const { register, handleSubmit } = useForm<ProfileFormValues>({
    defaultValues: {
      name: user?.name ?? '',
    },
  });

  if (!user) {
    return <p>Нет данных пользователя.</p>;
  }

  const onSubmit = async (data: ProfileFormValues) => {
    setIsSaving(true);
    try {
      await updateUserName(user.id, data.name);
      await reloadUser(); 
    } finally {
      setIsSaving(false);
    }
  };

  const handleDeactivate = async () => {
    setIsToggling(true);
    try {
      await deactivateUser(user.id);
      await reloadUser();
    } finally {
      setIsToggling(false);
    }
  };

  const handleActivate = async () => {
    setIsToggling(true);
    try {
      await reactivateUser(user.id);
      await reloadUser();
    } finally {
      setIsToggling(false);
    }
  };

  const handleRequestEmailConfirmation = async () => {
    setIsEmailSending(true);
    setEmailMessage(null);
    try {
      await requestEmailConfirmation(user.id);
      setEmailMessage('Письмо для подтверждения отправлено на почту.');
    } catch {
      setEmailMessage('Ошибка при отправке письма для подтверждения.');
    } finally {
      setIsEmailSending(false);
    }
  };

  const handleRequestPasswordReset = async () => {
    if (!user.email) {
      setResetMessage('У пользователя не указан email.');
      return;
    }

    setIsResetSending(true);
    setResetMessage(null);
    try {
      await requestPasswordReset(user.email);
      setResetMessage('Письмо для сброса пароля отправлено на почту.');
    } catch {
      setResetMessage('Ошибка при отправке письма для сброса пароля.');
    } finally {
      setIsResetSending(false);
    }
  };

  return (
    <div>
      <h1>Профиль</h1>
      <p>Email: {user.email}</p>
      <p>Роль: {user.role ?? 'user'}</p>
      <p>
        Статус аккаунта:{' '}
        <strong>{user.isActive ? 'Активен' : 'Деактивирован'}</strong>
      </p>
      <p>
        Почта подтверждена:{' '}
        <strong style={{ color: user.emailConfirmed ? 'green' : 'red' }}>
          {user.emailConfirmed ? 'Да' : 'Нет'}
        </strong>
      </p>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div>
          <label>Имя</label>
          <input {...register('name')} />
        </div>

        <button type="submit" disabled={isSaving}>
          Сохранить
        </button>
      </form>

      <div style={{ marginTop: '1.5rem', display: 'flex', gap: '0.75rem', flexWrap: 'wrap' }}>
        {user.isActive ? (
          <button
            type="button"
            className="danger"
            onClick={handleDeactivate}
            disabled={isToggling}
          >
            Деактивировать аккаунт
          </button>
        ) : (
          <button
            type="button"
            className="primary"
            onClick={handleActivate}
            disabled={isToggling}
          >
            Активировать аккаунт
          </button>
        )}
        
        <button
          type="button"
          className="primary"
          onClick={handleRequestEmailConfirmation}
          disabled={isEmailSending || !!user.emailConfirmed}
        >
          Отправить письмо для подтверждения
        </button>

        <button
          type="button"
          onClick={handleRequestPasswordReset}
          disabled={isResetSending}
        >
          Отправить письмо для сброса пароля
        </button>
      </div>

      {emailMessage && <p>{emailMessage}</p>}
      {resetMessage && <p>{resetMessage}</p>}
    </div>
  );
}
