# Плагины сервера

13 сторонних плагинов с uMod и один собственный. Все версии зафиксированы на момент
настройки сервера: если после обновления что-то поведёт себя иначе, откат версии — самый
быстрый способ разобраться.

## Установка

Скачать `.cs` с uMod и положить в `server\carbon\plugins\`. Carbon сам пересоберёт
плагин, перезапуск сервера не нужен. Свой плагин `RLXMenu.cs` берётся из папки
`plugins-source` этого репозитория.

## Список

| Плагин | Версия | Автор | Зачем |
|---|---|---|---|
| [Kits](https://umod.org/plugins/kits) | 4.4.10 | k1lly0u | Наборы, в том числе стартовый |
| [NTeleportation](https://umod.org/plugins/nteleportation) | 1.9.6 | nivex | Дома и телепорты `/home`, `/tpr` |
| [Backpacks](https://umod.org/plugins/backpacks) | 3.17.8 | WhiteThunder | Рюкзаки, сумеречная зона |
| [FancyDrop](https://umod.org/plugins/fancydrop) | 3.2.10 | FastBurst | Ежедневные дропы по расписанию |
| [FurnaceSplitter](https://umod.org/plugins/furnacesplitter) | 2.5.4 | FastBurst | Разделитель печей |
| [ChestStackMultiplier](https://umod.org/plugins/cheststackmultiplier) | 1.6.3 | MON@H | Сложение стаков в сундуках |
| [ChatClear](https://umod.org/plugins/chatclear) | 2.0.0 | Wulf | Очистка чата |
| [AdminHammer](https://umod.org/plugins/adminhammer) | 1.13.1 | mvrb | Молоток для админа |
| [AdminPanel](https://umod.org/plugins/adminpanel) | 1.4.8 | nivex | Панель админа |
| [AdminRadar](https://umod.org/plugins/adminradar) | 5.4.4 | nivex | Радар с уведомлениями в Discord |
| [AdminLogger](https://umod.org/plugins/adminlogger) | 2.4.2 | AK | Лог действий и уведомления в Discord |
| [RemoverTool](https://umod.org/plugins/removertool) | 4.3.48 | Reneb и другие | Удаление построек |
| [Vanish](https://umod.org/plugins/vanish) | 2.1.5 | Whispers88 | Невидимость |
| `RLXMenu` | 1.3.0 | этот репозиторий | Меню при входе и правила |

## Собственный плагин

`plugins-source/RLXMenu.cs` показывает меню через 12 секунд после входа, только когда игрок
жив: если при входе игрок мёртв, плагин ждёт возрождения и открывает меню после него.
Кнопки отправляют клиентские команды `kit`, `home`, `who`, `rules`, `menu`.

Подробности, включая почему плагин собирает CUI JSON вручную, в
`plugins-source/README.md`.

## Права

Разбивка прав по группам в `configs/permissions/groups.json`. Обычные игроки получают
рюкзаки, телепорт домой, удаление своих построек и разделитель печей. Группа `admin`
получает всё остальное, включая `/tpl`, `/tpsave` и `/tpremove`.

## Обновление

Обновлять плагины лучше по одному и с проверкой в игре. Carbon обновляет их сам, если
включено автообновление, но после крупных обновлений FancyDrop и NTeleportation
конфиги обычно требуют ручной правки: в них меняются названия секций.