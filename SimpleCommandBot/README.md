# Simple Command-line Bot

Это пример простейшего чат-бота, построенного на Microsoft Bot Framework 3.0. 

В рамках практической работы попробуйте создать такого бота самостоятельно, но в случае возникновения проблем -
смотрите на исходный код в этом примере.

## Предварительные шаги

Перед выполнением работы необходимо установить Bot Template, который будет использовать как отправная точка 
для создания проекта. 

1. Скачайте файл шаблона `http://aka.ms/bf-bc-vstemplate`
2. Поместите zip-файл *в исходном, не распакованном виде* в директорию `"%USERPROFILE%\Documents\Visual Studio 2015\Templates\ProjectTemplates\Visual C#\`
3. Скачайте и установите [Bot Framework Emulator](https://aka.ms/bf-bc-emulator)

## Создание проекта

1. Создайте новый проект типа *Bot Application*
2. Запустите проект и убедитесь, что бот работает через Bot Emulator 
   (возможно, вам придётся поместить правильный URL и порт в поле Emulator URL)
3. Добавьте в проект директорию [OpenWeatherMap](../ExtraFiles/OpenWeatherMap) со всеми файлами
4. Разработайте логику бота - ищите метод `Post` в файле [Controllers/MessagesController.cs](SimpleCommandBot/Controllers/MessagesController.cs)
5. Убедитесь, что бот работает в эмуляторе
6. Если вы берете исходный код из репозитория - добавьте ключ доступа к OpenWeatherMap API в файл `Config.cs`.
Ключ доступа можно получить [здесь](http://openweathermap.org/appid).
7. Опубликуйте бот в облаке
8. Зарегистрируйте бот на сайте [Bot Framework](http://dev.botframework.com) используя версию v3
9. Получите ключи *Microsoft App Id* и *Microsoft App Password* и добавьте их в файл 
[Web.Config](SimpleCommandBot/Web.Config) вашего проекта
10. Ещё раз опубликуйте код с добавленными ключами в облаке
11. Убедитесь, что бот в облаке работает в эмуляторе (для этого придётся добавить URL, App ID, App Password 
в эмуляторе)
12. Настройте реальные каналы общения (Skype, Slack, Telegram, etc.) через Bot Framework и убедитесь, что бот доступен 
через различные каналы общения.
13. Наслаждайтесь!

