#QueryExpressionBuilder
Библиотека для генирации функций-предикатов для фильтрирования запросов в БД.

##Описание
Данная библиотека помогает создать функцию-предикат для фильтрирования запросов в БД на основе модели.

##Инструкция
Для работы библиотеки необходимо создать модель с свойствами для фильтрации.<br>
Свойства в модели необходимо пометить атрибутами.<br><br>

На данный момент существует 3 атрибута:<br><br>

Пространство имен QueryExpressionBuilder.Attributes.String - атрибуты для свойств типа String<br>
StartWithAttribute - Означает, что дял свойства будет использоватся фильтр с аналогом функции System.String.StartWith()<br><br>

Пространство имен QueryExpressionBuilder.Attributes.Numbers - атрибут для всех свойств которые ввляются числовыми, в том числе DateTime<br>
GreaterOrEqualAttribute - Означает, что для свойства будет использоваться фильтр с аналогом условного выражения >=<br>
LessOrEqualAttribute - Означает, что для свойства будет использоваться фильтр с аналогом условного выражения <=<br>

В конструктор атрибута необходимо передать название свойства из класса представляющий сущьность в БД.<br><br>

##Примеры
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
        public int Age { get; set; }
        public float Amount { get; set; }
    }
```
Для создания Query фильтра нам необходимо создать query-модель
```csharp
    public class UserQuery
    {
        [QueryExpressionBuilder.Attributes.String.StartWith("Name")]
        public string Name { get; set; }

        [QueryExpressionBuilder.Attributes.String.StartWith("Surname")]
        public string Surname { get; set; }

        [QueryExpressionBuilder.Attributes.String.StartWith("Email")]
        public string Email { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("BirthDate")]
        public DateTime? FromBirthDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("BirthDate")]
        public DateTime? ToBirthDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("RegistrationDate")]
        public DateTime? FromRegistrationDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("RegistrationDate")]
        public DateTime? ToRegistrationDate { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.GreaterOrEqual("Age")]
        public int? FromAge { get; set; }

        [QueryExpressionBuilder.Attributes.Numbers.LessOrEqual("Age")]
        public int? ToAge { get; set; }

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
https://localhost:7001/GetUsers?Name=S&FromBirthDate=2010-01-01&ToBirthDate=2040-01-01&FromAge=20<br>
в БД мы получаес SQL запрос
```sql
SELECT "u"."Id", "u"."Age", "u"."Amount", "u"."BirthDate", "u"."Email", "u"."Name", "u"."PasswodHash", "u"."RegistrationDate", "u"."Surname"
      FROM "Users" AS "u"
      WHERE instr("u"."Name", 'S') > 0 AND "u"."BirthDate" >= '2010-01-01 00:00:00' AND "u"."BirthDate" <= '2040-01-01 00:00:00' AND "u"."Age" >= 20
```