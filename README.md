# QueryExpressionBuilder
Library for generating predicate functions for filtering database queries.

## Description
This library helps create a predicate function for filtering database queries based on a model.

## Instruction
To use the library, you need to create a model with properties for filtering. Properties in the model need to be marked with attributes.<br>

Currently, there are 3 attributes:<br>

Namespace QueryExpressionBuilder.Attributes.String - attributes for properties of type String<br>
StartWithAttribute - Indicates that a filter equivalent to the System.String.StartWith() function will be used for the property.<br>
ContainsAttribute - Indicates that a filter equivalent to the System.String.Contains() function will be used for the property.<br>

Namespace QueryExpressionBuilder.Attributes.Numbers - attribute for all properties that are numeric, including DateTime<br>
GreaterOrEqualAttribute - Indicates that a filter equivalent to the >= conditional expression will be used for the property.<br>
LessOrEqualAttribute - Indicates that a filter equivalent to the <= conditional expression will be used for the property.<br>
EqualsAttribute - Just compares values like in a function Equals.<br>

You need to pass the property name from the class representing the entity in the database to the attribute constructor.

## Examples
Suppose we have an entity User, which is an entity in the database.
```csharp
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string PasswodHash { get; set; }
        public float Amount { get; set; }
    }
```
To create a Query filter, we need to create a query model.
```csharp
    public class UserQuery
    {
        [QueryExpressionBuilder.Attributes.String.StartWith("Name")]
        public string? Name { get; set; }

        [QueryExpressionBuilder.Attributes.String.StartWith("Surname")]
        public string? Surname { get; set; }

        [QueryExpressionBuilder.Attributes.String.StartWith("Email")]
        public string? Email { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("BirthDate")]
        public DateTime? FromBirthDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("BirthDate")]
        public DateTime? ToBirthDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("RegistrationDate")]
        public DateTime? FromRegistrationDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("RegistrationDate")]
        public DateTime? ToRegistrationDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("Amount")]
        public float? FromAmount { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("Amount")]
        public float? ToAmount { get; set; }
    }
```
And now, in the controller method, which represents the endpoint, simply pass the UserQuery object to the ExpressionBuilder class, which will return the predicate function.
```csharp
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUserService userService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUserService _userService)
        {
            userService = _userService;
            _logger = logger;
        }

        /// <summary>
        /// Method for get users
        /// </summary>
        /// <param name="queryParams">Object containing filtering parameters</param>
        /// <returns>Returns a list of users</returns>
        [HttpGet("[controller]/GetUsers")]
        public async Task<IEnumerable<User>> GetUsers([FromQuery] UserQuery queryParams)
        {
            //Generating a predicate based on parameters
            var predicate = ExpressionBuilder.GetPredicate<User, UserQuery>(queryParams);
            //We pass the predicate to the service and return the result
            var result = await userService.GetUsers(predicate);
            return result;
        }
    }
```
Now, when sending a request to the database<br>
https://localhost:7001/GetUsers?Name=M&FromBirthDate=2000-01-01&ToBirthDate=2040-01-01&FromAmount=1000<br>
We get an SQL query.
```sql
SELECT "u"."Id", "u"."Amount", "u"."BirthDate", "u"."Email", "u"."Name", "u"."PasswodHash", "u"."RegistrationDate", "u"."Surname"
      FROM "Users" AS "u"
      WHERE instr("u"."Name", 'M') > 0 AND "u"."BirthDate" >= '2000-01-01 00:00:00' AND "u"."BirthDate" <= '2040-01-01 00:00:00' AND "u"."Amount" >= 1000
```
## License
This project is distributed under the [MIT](https://opensource.org/licenses/MIT) license, which allows free use, modification, and distribution of the code in accordance with the terms of the MIT license.
<br><br><br>

# QueryExpressionBuilder
Библиотека для генирации функций-предикатов для фильтрирования запросов в БД.

## Описание
Данная библиотека помогает создать функцию-предикат для фильтрирования запросов в БД на основе модели.

## Инструкция
Для работы библиотеки необходимо создать модель с свойствами для фильтрации.<br>
Свойства в модели необходимо пометить атрибутами.<br>

На данный момент существует 3 атрибута:<br>

Пространство имен QueryExpressionBuilder.Attributes.String - атрибуты для свойств типа String<br>
StartWithAttribute - Означает, что дял свойства будет использоватся фильтр с аналогом функции System.String.StartWith()<br>
ContainsAttribute - Означает, что дял свойства будет использоватся фильтр с аналогом функции System.String.Contains()<br>

Пространство имен QueryExpressionBuilder.Attributes.Numbers - атрибут для всех свойств которые ввляются числовыми, в том числе DateTime<br>
GreaterOrEqualAttribute - Означает, что для свойства будет использоваться фильтр с аналогом условного выражения >=<br>
LessOrEqualAttribute - Означает, что для свойства будет использоваться фильтр с аналогом условного выражения <=<br>
EqualsAttribute - Просто то же самое, что и Equals.<br>

В конструктор атрибута необходимо передать название свойства из класса представляющий сущьность в БД.<br>

## Примеры
Предположим у нас есть сущьность User, которая являеться сущьностью в БД.
```csharp
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string PasswodHash { get; set; }
        public float Amount { get; set; }
    }
```
Для создания Query фильтра нам необходимо создать query-модель
```csharp
    public class UserQuery
    {
        [QueryExpressionBuilder.Attributes.String.StartWith("Name")]
        public string? Name { get; set; }

        [QueryExpressionBuilder.Attributes.String.StartWith("Surname")]
        public string? Surname { get; set; }

        [QueryExpressionBuilder.Attributes.String.StartWith("Email")]
        public string? Email { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("BirthDate")]
        public DateTime? FromBirthDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("BirthDate")]
        public DateTime? ToBirthDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("RegistrationDate")]
        public DateTime? FromRegistrationDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("RegistrationDate")]
        public DateTime? ToRegistrationDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("Amount")]
        public float? FromAmount { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("Amount")]
        public float? ToAmount { get; set; }
    }
```
И теперь в методе контроллера, который представляет конечную точку, нужно просто передать обьект UserQuery в класс ExpressionBuilder, который вернет функцию-предикат.
```csharp
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IUserService userService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IUserService _userService)
        {
            userService = _userService;
            _logger = logger;
        }

        /// <summary>
        /// Метод получения юзеров
        /// </summary>
        /// <param name="queryParams">Объект содержащий параметры фильтрации</param>
        /// <returns>Возвращает список юзеров</returns>
        [HttpGet("[controller]/GetUsers")]
        public async Task<IEnumerable<User>> GetUsers([FromQuery] UserQuery queryParams)
        {
            //Генерируем предикат на основе параметров
            var predicate = ExpressionBuilder.GetPredicate<User, UserQuery>(queryParams);
            //Передаем предикат в сервис и возвращаем результат
            var result = await userService.GetUsers(predicate);
            return result;
        }
    }
```
Теперь отправляя запрос<br>
https://localhost:7001/GetUsers?Name=M&FromBirthDate=2000-01-01&ToBirthDate=2040-01-01&FromAmount=1000<br>
в БД мы получаем SQL запрос
```sql
SELECT "u"."Id", "u"."Amount", "u"."BirthDate", "u"."Email", "u"."Name", "u"."PasswodHash", "u"."RegistrationDate", "u"."Surname"
      FROM "Users" AS "u"
      WHERE instr("u"."Name", 'M') > 0 AND "u"."BirthDate" >= '2000-01-01 00:00:00' AND "u"."BirthDate" <= '2040-01-01 00:00:00' AND "u"."Amount" >= 1000
```
## Лицензия
Этот проект распространяется под лицензией [MIT](https://opensource.org/licenses/MIT), которая разрешает свободное использование, изменение и распространение кода в соответствии с условиями лицензии MIT.