# Шаблон Telegram-бота для развертывания в виде Yandex Function

Чтобы развернуть бота в YC Function для начала нужно создать аккаунт в Yandex Cloud и поставить себе YC CLI ([лучше оригинального гайда я не объясню](https://cloud.yandex.ru/docs/cli/quickstart#install))

Также для начала создайте себе сервисный аккаунт (ищите вкладочку в Yandex Cloud Console) 

Внутри Yandex Cloud нам нужно создать три сущности (все три можно найти во вкладке Console):
 - **Yandex Cloud Function** (там просто выберите имя, дальше пока ничего не нужно)
 - Бакет в **ObjectStorage** (тут тоже можно не запариваться над настройками, просто задайте имя). Тут можно сразу получить ключи доступа, которые нам понадабятся дальше. [Тык (см. CLI)](https://cloud.yandex.ru/docs/iam/operations/sa/create-access-key) (желательно сразу записать куда-нибудь последние две строчки ответа, а именно `key_id` и `secret`)
 - Базу данных **Managed Service for YDB** (точно так же выбрать имя, в качестве типа я выбирал Serverless)

## Настройки среды

Теперь, когда у нас есть все нужные части, то можно заполнить все настройки окружения. Сейчас пройдусь по необходимым переменным.
Внутри проектов бота и тестов лежат файлы `settings.example.json` и `testsettings.example.json` соответственно. Вместо них надо создать `settings.json` и `testsettings.json` и заменить значения на нужные.

Что там лежит:
 - `AppSettings` -- тут лежат всякие рандомные настройки
   -  `TelegramToken` -- тут думаю все очевидно, токен телеграм-бота
   -  `YdbEndpoint` -- эндпоинт API YDB, искать во вкладке "Обзор" и разделе "Соединение" для вашей YDB-базы в Облаке. Выглядит как `grpc://домен:порт`
   -  `YdbPath` -- расположение собственно вашей базы, искать прямо под эндпоинтом под названием "Размещение базы данных"
   -  [для тестов] `IamTokenPath` -- абсолютный путь к файлу, в который вы запихнете IAM-токен (сгенерить его можно поменяв нужные параметры в `Tests/getIamFile.cmd` и запустив этот скрипт в терминале)
 -  `CloudStorageOptions` -- это все настройки вашего объектного хранилища
    -  `Bucket` -- имя бакета, которое вы указывали при создании Object Storage
    -  `AccessKey` -- тот самый `key_id`, который вы получили несколькими шагами ранее
    -  `SecretKey` -- тот самый `secret`, который вы получили несколькими шагами ранее
    -  `Endpoint` -- эндпоинт для запросов, по умолчанию `storage.yandexcloud.net`
    -  `Location` -- зона доступности, можете выбрать, например, из этих: `ru-central1-a`, `ru-central1-b`, `ru-central1-c`
    -  `Protocol` -- протокол, не вижу смысла менять его на что-то кроме `https`


## Деплой

Все переменные выставлены, можно переходить к деплою, он состоит из двух шагов:
 - Посмотрите в файл `deploy.cmd`. В нем находится шаблон для деплоя вашей функции в Облако. Вставьте туда имя своей функции и ID сервисного аккаунта, после чего можно запускать скрипт. Если захотите залить более свежую версию бота в облако, просто запустите скрипт еще раз. Теперь у вас есть облачная функция!
 - Однако бот до сих пор не работает. Для того чтобы это исправить нужно настроить для бота хуки через API телеграма. Для этого в корне лежит папка `hooks`. Там лежит два абсолютно идентичных скрипта: для `cmd` и `PowerShell`. Используйте что ближе к вам. Просто вставьте в скрипт токен вашего бота и ID вашей функции, после чего, если все будет ок, то у вас появится развернутый в Yandex Function бот.
