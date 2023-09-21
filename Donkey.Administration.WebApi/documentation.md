# Header
`Content-Type: application/json`

# Методы окружения

### GET: {host}/api/environment/time
Получение текущего времени сервера 

### GET: {host}/api/environment/hash
Хеширование переданной строки

`Body:`
```"123"``` - строка, которую требуется захэшировать

### GET: {host}/api/environment/getKey

Получение ключа, необходимого для последующих методов

`Body:`
```json
{
	"Data":
	{
		"receiver": "{username}", // никнейм
		"firstKey": "{anscjk1l24pnsdknv124}" // захэшированный первичный ключ (его получение распшу отдельно)
	}
}
```

`positive Response:`
```json
{
  "success": true,
  "message": null,
  "data": {
    "key": "fbdcf094-620c-47a0-a26d-885e3cef1bf4"
  }
}
```

`negative Response:`
```json
{
  "success": true,
  "message": null,
  "data": {
    "key": "First key is invalid"
  }
}
```

### GET: {host}/api/environment/auth
Аутентификация администратора

`Body:`
```json
{
	"Data":
	{
		"username":"{username}",
		"passwordHash":"{password_hash}"
	}
}
```

`Positive Response:`
```json
{
  "success": true,
  "message": null,
  "data": {
    "key": "{valid_key}"
  }
}
```
`Negotive Response:`
```json
{
  "success": false,
  "message": "Внутренняя ошибка: Некорректные данные",
  "data": null
}
```

# Методы, предоставляемые администраторам
Ключ получается из метода аутентификации администратоа

### GET: {host}/api/admins/client
Получение информации обо всех клиентах

`Body:`
```json
{
	"Key":"asnckjn1234215" // ключ авторизации, получаемый выше
}
```

`positive Response:`
```json
{
  "success": true,
  "message": null,
  "data": {
    "data": [
      {
        "name": "api-added",
        "secondName": "dotnetov",
        "thirdName": ""dotnetovich,
        "emailAddress": "api@gmail.com",
        "phoneNumber": "123226"
      }
    ]
  }
}
```

### GET: {host}/api/admins/client?id={id}
Получение информации об одном конкретном клиенте

`Body:`
```json
{
	"Key":"6677758b-e4ec-48b4-a86b-6a5160a98905",
	"Data": 
	{
		"email": "api@gmail.com"
	}
}
```
`Positive response:`
```json
{
  "success": true,
  "message": null,
  "data": {
    "client": {
      "name": "api-added",
      "secondName": "dotnetov",
      "thirdName": null,
      "emailAddress": "api@gmail.com",
      "phoneNumber": "123226"
    },
    "orders": [
      {
        "id": null,
        "client": {
          "name": "api-added",
          "secondName": "dotnetov",
          "thirdName": "aksmcalsc",
          "emailAddress": "api@gmail.com",
          "phoneNumber": "123226"
        },
        "arrivalTime": "2021-07-11T13:31:00",
        "finishTime": "2021-06-12T00:00:00",
        "description": "[добавить в бд поле описания заказа]",
        "tags": [
          0,
          1,
          2
        ],
        "status": 0
      }
    ]
  }
}
```
### GET: {host}/api/admins/order
Получение всех заказов

`Body:`
```json
{
	"Key":"72db82b1-3845-4957-8ec2-8c71dc72db42"
}
```
`Positive response:`
```json
{
  "success": true,
  "message": null,
  "data": {
    "data": [
      {
        "id": "1",
        "client": {
          "name": "api-added",
          "secondName": "dotnetov",
          "thirdName": "aksmcalsc",
          "emailAddress": "api@gmail.com",
          "phoneNumber": "123226"
        },
        "arrivalTime": "2021-07-11T13:31:00",
        "finishTime": "2021-06-12T00:00:00",
        "description": "Добавить в бд описание заказа",
        "tags": [
          0,
          1,
          2
        ],
        "status": 0
      }
    ]
  }
}
```

# Методы, предоставляемые клиентскому интерфейсу
Ключ полуачется методом getKey

### POST: {host}/api/general/client
Добавление клиента

`Body:`
```json
{
	"Key": "6677758b-e4ec-48b4-a86b-6a5160a98905",
	"Data":
	{
		"client": 
		{
			"Name": "api-added",
			"SecondName": "dotnetov",
			"ThirdName": "aksmcalsc",
			"EmailAddress": "api@gmail.com",
			"Contacts": "Номер телефона: +79999995634, телеграмм: @afdf123"
		}
		
	}
}
```

`Positive response:`
```json
{ 
	"Success": true, 
	"Message": null, 
	"Data": null 
}
```

`Negative response:`
```json
{ 
	"Success": false, 
	"Message": "Некорректный токен", 
	"Data": null 
}
```

### POST: {host}/api/general/order
Добавление заказа. 

`Body:`
```json
{
	"Key": "72db82b1-3845-4957-8ec2-8c71dc72db42",
	"Data":
	{
		"order":
		{
			"FinishTime": "06.12.2021 00:00",
			"Description": "Описание",
			"Tags": [0, 1, 2], // именованные константы, позже согласуем
			"Email": "test@test.ru",
			"Title": "Название",
			"Topic": "Тема",
			"AdditionalInfo": "Ссылка на Figma: <ссылка>"
		}
	}
}
```

`Response:`
```json
{ 
	"Success": true, 
	"Message": null, 
	"Data": null 
}
```
```json
{ 
	"Success": false, 
	"Message": "Ошибка сервера", 
	"Data": null 
}
```

### PUT: {host}/api/admins/order?id={id}
Изменение информации о проекте. 
(пока не подключен)

`Body:`
```json
{
	"Key": "72db82b1-3845-4957-8ec2-8c71dc72db42",
	"Data":
	{
		"Order":
		{
			"Id": 1001,
			"FinishTime": "06.12.2021 00:00",
			"Description": "Описание",
			"Tags": ["Дизайн", "Бэк"],
			"Status": "В работе",
			"PercentageDone": 30,
			"Title": "Название",
			"Topic": "Тема",
			"AdditionalInfo": "Ссылка на Figma: <ссылка>",
			"LatestEditAuthorId": 134 
		}
	}
}
```

`Response:`
```json
{ 
	"Success": true, 
	"Message": null, 
	"Data": null 
}
```
```json
{ 
	"Success": false, 
	"Message": "Ошибка сервера", 
	"Data": null 
}
```
