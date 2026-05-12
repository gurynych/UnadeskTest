Тестовое задание: загрузка PDF и извлечение текста.

UnadeskTest.Api — REST API. Принимает PDF, копирует документ и outbox-запись в БД.
<br/>UnadeskTest.Worker — слушает очередь, извлекает текст, обновляет документ в БД.
<br/>UnadeskTest.Shared — общий код.

Для запуска в .env.example проставить пароль для postgres и переименовать в .env.
<br/>docker compose up --build