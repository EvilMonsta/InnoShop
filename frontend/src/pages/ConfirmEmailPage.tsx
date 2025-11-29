import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { confirmEmail } from '../shared/api/users';

type Status = 'idle' | 'loading' | 'success' | 'error';

export function ConfirmEmailPage() {
  const { token } = useParams<{ token: string }>();
  const [status, setStatus] = useState<Status>('idle');
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    if (!token) {
      setStatus('error');
      setErrorMessage('Некорректная ссылка для подтверждения почты.');
      return;
    }

    const run = async () => {
      setStatus('loading');
      setErrorMessage(null);
      try {
        await confirmEmail(token);
        setStatus('success');
      } catch (err) {
        setStatus('error');
        setErrorMessage(
          'Ошибка при подтверждении почты. Возможно, ссылка устарела или уже использована.',
        );
      }
    };

    void run();
  }, [token]);

  return (
    <div>
      <h1>Подтверждение e-mail</h1>

      {status === 'loading' && <p>Подтверждаем ваш e-mail...</p>}

      {status === 'success' && (
        <>
          <p>Ваш e-mail успешно подтверждён ✅</p>
          <p>
            Теперь вы можете использовать все функции аккаунта.{' '}
            <Link to="/login">Перейти к входу</Link>
          </p>
        </>
      )}

      {status === 'error' && (
        <>
          <p>{errorMessage ?? 'Ошибка при подтверждении e-mail.'}</p>
          <p>
            Попробуйте запросить новое письмо в{' '}
            <Link to="/profile">профиле</Link> или{' '}
            <Link to="/login">вернитесь на страницу входа</Link>.
          </p>
        </>
      )}
    </div>
  );
}
