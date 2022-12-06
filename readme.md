## Úkoly

1. Použijte v projektu IoC container.

# Popis projektu

V projektu používáme Entity Framework, abychom ho mohli používat tak musíme buď vytvořit databází příkazem aplikaci nebo pomocí příkazové řádky. V následujícím textu si ukážeme obě možnosti.

Projekt používá klíčové slovo ```required``` z C# 11, potřebujeme mít tedy nainstalovaný minimálně .NET 7.

Návod pro Entity Framework: [Getting Started with EF Core](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)

Budeme používat následující příkazy pro příkazovou řádku:
- Příkaz ```dotnet add packgage``` stáhne nuget balíček z repozitáře nuget.org a přidá ho do projektu.
- Příkaz ```dotnet tool install```, který instaluje nové příkazy pro příkazovou řádku.
- Příkaz ```dotnet ef``` pomocí kterého vytváříme například nové migrace nebo aktualizujeme databázi.

## Entity Framework Provider

Pokud chceme používat konkrétní databázi s Entity Frameworkem, tak musím do projektu přidat providera pro tuto databázi. Provider je většinou knihovna distriovaná jako nuget balíček. Následující příkaz nainstaluje nuget balíček, konrétně EF database provider pro databázi Sqlite. 

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Bez použití migrací můžeme vytvořit databází pomocí metody *EnsureCreated() nebo EnsureCreatedAsync()*.

```csharp
await using SkolaContext db = new SkolaContext();
bool created = await db.Database.EnsureCreatedAsync();
```
Dokumentace: [Dependency inversion]([https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli](https://learn.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/architectural-principles#dependency-inversion))

## IoC Container

IoC Container je třída pomocí které vytváříme instance jiných tříd a zároveň při vytváření těchto instancí zjistí jaké jsou parametry konstruktoru a umí automaticky vkládat (inject) další instace jako argumenty konstruktoru.

Dotnet obsahuje zabudovaný IoC kontejner, který přidáme do projektu pomocí následujího nuget balíčku:

```powershell
dotnet add package Microsoft.Extensions.DependencyInjection
```

Nejprve si zaregistruje třídy, jejichž instance, chceme vytvářet. Zároveň zvolím jaký bude životnost vytvořených objektů. 

- Singleton znamená, že bude existovat jen jedna instance dané třídy. Kontejner tedy vždy vrátí referenci na stejný objekt.
- Transient znamé, že se vytvoří vždy nová instance třídy. Kontejner tedy vrátí vždy referenci na nový objekt.
- Scooped znamená, že se vytvoří vždy nová instance třídy pro nový request, ale v rámci jednoho requestu pak už vrací referenci na stejný objekt.


V následujícím příkladu registrujme DatabaseService jako Singleton:

```csharp
IServiceCollection serviceCollection = new ServiceCollection().AddSingleton<DatabaseService>();
```

A instance třídy DatabaseService vytváříme následujícím způsobem pomocí třídy ```ServiceProvider```. Kdy ```service1``` i ```service2``` představují referenci na stejný objekt.

```csharp
ServiceProvider provider = serviceCollection.BuildServiceProvider();

DatabaseService service1 = provider.GetRequiredService<DatabaseService>();
DatabaseService service2 = provider.GetRequiredService<DatabaseService>();
```

Příklad rozšíříme a zaregistrujeme ViewModel jako transient. 

```csharp
IServiceCollection serviceCollection = new ServiceCollection()
                .AddSingleton<DatabaseService>()
                .AddTransient<StudentListViewModel>();
```

Nyní mně kontejner vytvoří dvě instance třídy ```StudentViewModel```, ale každé instanci ```StudentViewModelu``` předá jako argument konstruktoru refenci na stejnou instanci třídy ```DatabaseService```.

```csharp
StudentListViewModel viewModel1 = provider.GetRequiredService<StudentListViewModel>();
StudentListViewModel viewModel2 = provider.GetRequiredService<StudentListViewModel>();
```

### IoC a Dependency Injection

IoC kontejner se používa společně s Dependency Injection. V následujícím příkladu máme rozhraní IDatabaseService a dvě implementace DatabaseService, která pracuje s databází a FakeDatabaseService, která slouží jen pro testování a vrací jen objekty v paměti.

V kontejneru si potom můžeme jednoduše volit konkrétní implementaci.

```csharp
IServiceCollection serviceCollection = new ServiceCollection()
  .AddSingleton<IDatabaseService, FakeDatabaseService>()
  .AddTransient<StudentListViewModel>();
```

---
Použité rozhraní a třídy.

```csharp
public interface IDatabaseService
{
    Task<List<Student>> GetAllStudents();
}
```

```csharp
public class DatabaseService : IDatabaseService
{
    public async Task<List<Student>> GetAllStudents()
    {
        await using SchoolContext schoolContext = new SchoolContext();

        List<Student> students = await schoolContext.Students.ToListAsync();

        return students;
    }
}
```

```csharp
public class FakeDatabaseService : IDatabaseService
{
    public Task<List<Student>> GetAllStudents()
    {
        List<Student> studenti = new List<Student>()
        {
            new Student() { Id = 1, Name = "Jitka"},
            new Student() { Id = 2, Name = "Oto"},
            new Student() { Id = 3, Name = "Jiri"}
        };

        return Task.FromResult(studenti);
    }
}
```
  
```csharp
public class StudentListViewModel
{
  private readonly DatabaseService databaseService;

  public StudentListViewModel(DatabaseService databaseService)
  {
      this.databaseService = databaseService;
  }
}
```

## Migrace

Pomocí migrací můžeme vytvářet a aktualizovat databázi pomocí příkazů pro příkazovou řádku.

### Příprava potřebných závislostí 

Proto, abychom mohli vytvářet nové migrace a aktualizovat databázi, tak musíme nainstalovat:
- nástroj **dotnet ef**. 
- Nuget balíček podporující vytváření migrací **Microsoft.EntityFrameworkCore.Design**

Následující příkaz nainstaluje příkaz **dotnet ef** globálně pro všechny projekty.

```powershell
dotnet tool install --global dotnet-ef
```

A následující příkaz přída do projektu nuget balíček **Microsoft.EntityFrameworkCore.Design**.

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Vytváření a spuštění migrací

Migrace představuje kód v jazyce C# který umí například vytvářet nebo aktualizovat tabulky v databázi a případně i vložit výchozí data pro model. 

Následující příkaz **dotnet ef** vytvoří novou migraci s názvem *VychoziMigrace*. 

```powershell
dotnet ef migrations add VychoziMigrace
```

A následující příkaz migraci aplikuje a vytvoří novou databází, nebo zaktualizuje stávající.

```powershell
dotnet ef database update
```

---
Poznámka

Na školních počítačích použijte v terminálu příkaz ```$env:Path = "D:\dotnet;C:\Users\ekral\.dotnet\tools"``` kde změňte uživatelské jméno a případně cestu k souboru dotnet.exe.
