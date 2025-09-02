# TaskTracker
********************************
Уровень: Beginner
Тип: CLI
Технологии: C#, .NET9
Источник задания: (TaskTracker)[https://roadmap.sh/projects/task-tracker]
Статус: в разработке

TaskTracker запускается из командной строки, принимает действия пользователя и вводимые данные в качестве аргументов
и сохраняет задачи в JSON-файле. 

Пользователь имеет возможность:
- Добавлять, обновлять и удалять задачи
- Помечать задачу как выполняемую или завершенную
- Перечислять все задачи
- Перечислите все задачи, которые были выполнены
- Перечислите все задачи, которые не были выполнены
- Перечислите все задачи, которые находятся в процессе выполнения

## Пример

### Добавить новую задачу
```shell
task-cli add "Buy groceries"
```
### Output: Task added successfully (ID: 1)

### Обновление м удаление задач
```shell
task-cli update 1 "Buy groceries and cook dinner"
task-cli delete 1
```

### Пометить задачу как in progress или done
```shell
task-cli mark-in-progress 1
task-cli mark-done 1
```

### Список всех задач
```shell
task-cli list
```

### Список задач по статусу
```shell
task-cli list done
task-cli list todo
task-cli list in-progress
```

********************************
Level: Beginner
Type: CLI
Technology: C#, .NET9
Source: (TaskTracker)[https://roadmap.sh/projects/task-tracker]
Stage: In process

TaskTracker run from the command line, accept user actions and inputs as arguments, 
and store the tasks in a JSON file. 

The user is able to:
- Add, Update, and Delete tasks
- Mark a task as in progress or done
- List all tasks
- List all tasks that are done
- List all tasks that are not done
- List all tasks that are in progress

## Example

### Adding a new task
```shell
task-cli add "Buy groceries"
```
### Output: Task added successfully (ID: 1)

### Updating and deleting tasks
```shell
task-cli update 1 "Buy groceries and cook dinner"
task-cli delete 1
```

### Marking a task as in progress or done
```shell
task-cli mark-in-progress 1
task-cli mark-done 1
```

### Listing all tasks
```shell
task-cli list
```

### Listing tasks by status
```shell
task-cli list done
task-cli list todo
task-cli list in-progress
```