# TaskManager – Aplicație de gestionare a sarcinilor (C# / .NET)

## Descriere generală
**TaskManager** este o aplicație dezvoltată în **C# (.NET 9)** care permite gestionarea sarcinilor (task-uri), folosind principii de **programare orientată pe obiect**, stocare în fișiere și o interfață **Console + WPF**.

Proiectul a fost realizat în scop educațional, pentru aprofundarea conceptelor OOP și a lucrului cu tehnologii .NET.

---

## Funcționalități principale
- Creare, actualizare și gestionare task-uri
- Prioritizarea sarcinilor
- Reminder-e și work items
- Planificare folosind matrice (planner)
- Salvare și încărcare date din fișiere:
  - fișiere text
  - fișiere binare
- Interfață:
  - aplicație Console
  - aplicație WPF (Windows Presentation Foundation)

---

## Structura proiectului

TaskManager/
│
├── TaskManager.Console/ # Aplicație de tip Console
│
├── TaskManagerApp/
│ ├── TaskManager.Core/ # Logica aplicației
│ │ ├── Models/ # Clasele de bază (TaskItem, WorkItem etc.)
│ │ ├── Interfaces/ # Interfețe
│ │ ├── Services/ # Servicii (TaskService, Repository)
│ │ ├── Storage/ # Stocare fișiere text/binare
│ │ ├── Exceptions/ # Excepții custom
│ │ └── Dtos/ # Data Transfer Objects
│ │
│ └── TaskManager.Wpf/ # Interfață grafică WPF (MVVM)
│
├── TaskManager.sln # Soluția Visual Studio
└── README.md

---

## Concepte de programare utilizate

### ✔ Programare Orientată pe Obiect (OOP)
- Clase și obiecte
- Încapsulare
- Constructori și proprietăți

### ✔ Moștenire
- Clase derivate (ex: `TaskItem`, `WorkItem`, `ReminderItem`)

### ✔ Interfețe
- `IStorable`
- `IValidatable`
- `IHasPriority`

### ✔ Polimorfism
- Tratarea unitară a obiectelor prin interfețe și clase de bază

### ✔ Clase abstracte
- Folosite pentru structurarea logicii comune

### ✔ Lucrul cu colecții
- `List<T>`
- `Dictionary<TKey, TValue>`

### ✔ Lucrul cu matrice
- Utilizate în `MatrixPlanner` pentru planificarea task-urilor

### ✔ Expresii Lambda
- Pentru filtrare, sortare și procesarea colecțiilor

### ✔ Tratarea excepțiilor
- Excepții standard
- Excepții definite de utilizator:
  - `TaskManagerException`
  - `ValidationException`
  - `StorageException`

### ✔ Lucrul cu fișiere
- Salvare / încărcare date din:
  - fișiere text
  - fișiere binare

---

## Tehnologii utilizate
- Limbaj: **C#**
- Framework: **.NET 9**
- UI: **Console + WPF**
- IDE: **Visual Studio / VS Code**
- Control versiuni: **Git & GitHub**

---

## Rulare aplicație

### Console
```bash
dotnet run --project TaskManager.Console
