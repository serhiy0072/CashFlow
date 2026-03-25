# CashFlow — Personal Finance Tracker API

REST API для обліку особистих фінансів. Дозволяє відстежувати доходи та витрати, категоризувати транзакції, встановлювати бюджети та переглядати статистику.

## Технології

- **Backend:** ASP.NET Core Web API (.NET 10)
- **ORM:** Entity Framework Core
- **База даних:** PostgreSQL 18
- **Аутентифікація:** ASP.NET Identity + JWT Bearer
- **Архітектура:** Clean Architecture + Repository Pattern
- **Валідація:** DataAnnotations

## Структура проєкту

```
CashFlow/
├── Api/                — Контролери, Middleware, Program.cs
├── Application/        — DTO, інтерфейси репозиторіїв
├── Domain/             — Entities, Enums
├── Infrastructure/     — EF Core DbContext, міграції, репозиторії
└── Tests/              — Unit-тести (xUnit)
```

**Clean Architecture** — залежності йдуть від зовнішніх шарів до внутрішніх:
- `Domain` — не залежить ні від кого
- `Application` → Domain
- `Infrastructure` → Application
- `Api` → Application + Infrastructure

## 🗄 Сутності

| Сутність | Опис |
|----------|------|
| **AppUser** | Користувач (ASP.NET Identity) з додатковими полями FirstName, LastName |
| **Category** | Категорія доходу/витрати (Їжа, Зарплата, Транспорт тощо) |
| **Transaction** | Фінансова транзакція з сумою, типом, датою, описом |
| **Budget** | Місячний ліміт витрат на категорію з відстеженням перевищення |

## Аутентифікація

API використовує JWT Bearer токени.

**Реєстрація:**
```
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "SecurePass1!",
  "firstName": "Ім'я"
}
```

**Логін:**
```
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "SecurePass1!"
}
```

Відповідь містить JWT токен, який передається в заголовку:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIs...
```

## API Ендпоінти

### Категорії
| Метод | URL | Опис |
|-------|-----|------|
| GET | `/api/categories` | Отримати всі категорії |
| GET | `/api/categories/{id}` | Отримати категорію за Id |
| POST | `/api/categories` | Створити категорію |
| PUT | `/api/categories/{id}` | Оновити категорію |
| DELETE | `/api/categories/{id}` | Видалити категорію |

### Транзакції
| Метод | URL | Опис |
|-------|-----|------|
| GET | `/api/transactions` | Отримати транзакції (з фільтрацією) |
| GET | `/api/transactions/{id}` | Отримати транзакцію за Id |
| POST | `/api/transactions` | Створити транзакцію |
| PUT | `/api/transactions/{id}` | Оновити транзакцію |
| DELETE | `/api/transactions/{id}` | Видалити транзакцію |

**Параметри фільтрації транзакцій:**
```
GET /api/transactions?type=Expense&minAmount=100&page=1&pageSize=10&sortBy=date&sortDirection=desc
```

| Параметр | Тип | Опис |
|----------|-----|------|
| type | string | "Income" або "Expense" |
| categoryId | Guid | Фільтр по категорії |
| dateFrom | DateTime | Дата від |
| minAmount | decimal | Мінімальна сума |
| maxAmount | decimal | Максимальна сума |
| page | int | Номер сторінки (за замовчуванням 1) |
| pageSize | int | Записів на сторінку (за замовчуванням 10) |
| sortBy | string | "date" або "amount" |
| sortDirection | string | "asc" або "desc" |

### Бюджети
| Метод | URL | Опис |
|-------|-----|------|
| GET | `/api/budgets` | Отримати всі бюджети |
| GET | `/api/budgets/{id}` | Отримати бюджет за Id |
| POST | `/api/budgets` | Створити бюджет |
| PUT | `/api/budgets/{id}` | Оновити бюджет (суму) |
| DELETE | `/api/budgets/{id}` | Видалити бюджет |

Відповідь бюджету включає реальні витрати:
```json
{
  "id": "...",
  "amount": 3000,
  "categoryName": "Їжа",
  "spent": 530,
  "remaining": 2470,
  "isExceeded": false
}
```

### Статистика
| Метод | URL | Опис |
|-------|-----|------|
| GET | `/api/statistics/dashboard` | Дашборд за поточний місяць |

**Параметри:**
```
GET /api/statistics/dashboard?dateFrom=2026-01-01&dateTo=2026-03-31
```

Відповідь:
```json
{
  "totalIncome": 55000,
  "totalExpense": 530,
  "balance": 54470,
  "expensesByCategory": [
    { "categoryName": "Їжа", "total": 530, "count": 3 }
  ],
  "incomesByCategory": [
    { "categoryName": "Зарплата", "total": 55000, "count": 2 }
  ]
}
```

## Як запустити

### Передумови
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 18](https://www.postgresql.org/download/)

### Кроки

1. **Клонуй репозиторій:**
```bash
git clone https://github.com/serhiy0072/CashFlow.git
cd CashFlow
```

2. **Налаштуй connection string** в `Api/appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=CashFlowDb;Username=postgres;Password=your_password"
}
```

3. **Налаштуй JWT ключ** (User Secrets):
```bash
cd Api
dotnet user-secrets set "Jwt:Key" "Your_Super_Secret_Key_At_Least_32_Characters!"
```

4. **Застосуй міграції:**
```bash
dotnet ef database update --project Infrastructure --startup-project Api
```

5. **Запусти API:**
```bash
dotnet run --project Api
```

API буде доступний за адресою `https://localhost:7130`

## 🏗 Архітектурні рішення

- **Repository Pattern** — контролери працюють через інтерфейси, не напряму з DbContext
- **JWT аутентифікація** — кожен юзер бачить тільки свої дані
- **Guid для Id** — безпечніше для API ніж числові Id
- **Enum як string** — TransactionType зберігається як текст в базі
- **DataAnnotations** — валідація на рівні DTO
- **Global Exception Middleware** — єдина точка обробки помилок

## Автор

**Serhii** — [GitHub](https://github.com/serhiy0072)
