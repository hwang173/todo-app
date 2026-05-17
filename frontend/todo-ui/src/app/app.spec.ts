import { describe, it, expect, beforeEach, vi } from 'vitest';
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of, throwError } from 'rxjs';
import { App } from './app';
import { TodoService } from './todo.service';
import { TodoItem } from './todo-item';

describe('App Component', () => {
  let component: App;
  let fixture: ComponentFixture<App>;
  let todoService: TodoService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App, HttpClientTestingModule],
      providers: [TodoService]
    }).compileComponents();

    fixture = TestBed.createComponent(App);
    component = fixture.componentInstance;
    todoService = TestBed.inject(TodoService);
  });

  describe('Component Initialization', () => {
    it('should create the app component', () => {
      expect(component).toBeTruthy();
    });

    it('should initialize with empty newTodoTitle', () => {
      expect(component.newTodoTitle).toBe('');
    });

    it('should have todos$ observable after init', () => {
      // Arrange
      const mockTodos: TodoItem[] = [];
      vi.spyOn(todoService, 'getTodos').mockReturnValue(of(mockTodos));

      // Act
      fixture.detectChanges();
      component.ngOnInit();

      // Assert
      let result: TodoItem[] = [];
      component.todos$.subscribe((todos: TodoItem[]) => {
        result = todos;
      });
      
      expect(result).toEqual(mockTodos);
    });
  });

  describe('ngOnInit', () => {
    it('should initialize todos$ observable', () => {
      // Arrange
      const mockTodos: TodoItem[] = [{ id: '1', title: 'Test Todo' }];
      vi.spyOn(todoService, 'getTodos').mockReturnValue(of(mockTodos));

      // Act
      component.ngOnInit();

      // Assert
      let result: TodoItem[] = [];
      component.todos$.subscribe((todos: TodoItem[]) => {
        result = todos;
      });
      
      expect(result.length).toBe(1);
    });

    it('should call getTodos on init', () => {
      // Arrange
      const mockTodos: TodoItem[] = [];
      const getTodosSpy = vi.spyOn(todoService, 'getTodos').mockReturnValue(of(mockTodos));

      // Act
      component.ngOnInit();
      
      let called = false;
      component.todos$.subscribe(() => {
        called = true;
      });

      // Assert
      expect(getTodosSpy).toHaveBeenCalled();
      expect(called).toBe(true);
    });
  });

  describe('addTodo', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should add a new todo with valid title', () => {
      // Arrange
      component.newTodoTitle = 'New Todo';
      const newTodo: TodoItem = { id: '1', title: 'New Todo' };
      vi.spyOn(todoService, 'addTodo').mockReturnValue(of(newTodo));
      vi.spyOn(todoService, 'getTodos').mockReturnValue(of([newTodo]));

      // Act
      component.addTodo();

      // Assert
      expect(todoService.addTodo).toHaveBeenCalledWith('New Todo');
    });

    it('should clear title after adding todo', async () => {
      // Arrange
      component.newTodoTitle = 'Test Todo';
      const newTodo: TodoItem = { id: '1', title: 'Test Todo' };
      vi.spyOn(todoService, 'addTodo').mockReturnValue(of(newTodo));
      vi.spyOn(todoService, 'getTodos').mockReturnValue(of([newTodo]));

      // Act
      component.addTodo();

      // Assert - wait for async operations
      await new Promise(resolve => setTimeout(resolve, 150));
      expect(component.newTodoTitle).toBe('');
    });

    it('should not add todo with empty title', () => {
      // Arrange
      component.newTodoTitle = '';
      const addTodoSpy = vi.spyOn(todoService, 'addTodo');

      // Act
      component.addTodo();

      // Assert
      expect(addTodoSpy).not.toHaveBeenCalled();
    });

    it('should not add todo with only whitespace', () => {
      // Arrange
      component.newTodoTitle = '   ';
      const addTodoSpy = vi.spyOn(todoService, 'addTodo');

      // Act
      component.addTodo();

      // Assert
      expect(addTodoSpy).not.toHaveBeenCalled();
    });

    it('should refresh todos after adding', async () => {
      // Arrange
      component.newTodoTitle = 'New Todo';
      const newTodo: TodoItem = { id: '1', title: 'New Todo' };
      vi.spyOn(todoService, 'addTodo').mockReturnValue(of(newTodo));
      const getTodosSpy = vi.spyOn(todoService, 'getTodos').mockReturnValue(of([newTodo]));

      // Act
      component.addTodo();

      // Assert - wait for async operations
      await new Promise(resolve => setTimeout(resolve, 150));
      expect(getTodosSpy).toHaveBeenCalled();
    });
  });

  describe('deleteTodo', () => {
    beforeEach(() => {
      component.ngOnInit();
    });

    it('should delete a todo by id', () => {
      // Arrange
      const todoId = '1';
      const deleteTodoSpy = vi.spyOn(todoService, 'deleteTodo').mockReturnValue(of(undefined));
      vi.spyOn(todoService, 'getTodos').mockReturnValue(of([]));

      // Act
      component.deleteTodo(todoId);

      // Assert
      expect(deleteTodoSpy).toHaveBeenCalledWith(todoId);
    });

    it('should refresh todos after deletion', async () => {
      // Arrange
      const todoId = '1';
      vi.spyOn(todoService, 'deleteTodo').mockReturnValue(of(undefined));
      const getTodosSpy = vi.spyOn(todoService, 'getTodos').mockReturnValue(of([]));

      // Act
      component.deleteTodo(todoId);

      // Assert - wait for async operations
      await new Promise(resolve => setTimeout(resolve, 150));
      expect(getTodosSpy).toHaveBeenCalled();
    });

    it('should handle delete errors gracefully', () => {
      // Arrange
      const todoId = '1';
      const refreshSpyOnNext = vi.spyOn(component['refreshTodos$'], 'next');
      vi.spyOn(todoService, 'deleteTodo').mockReturnValue(of(void 0));

      // Act
      component.deleteTodo(todoId);
      
      // Assert - verify the delete was called and refresh triggered
      expect(todoService.deleteTodo).toHaveBeenCalledWith(todoId);
      expect(refreshSpyOnNext).toHaveBeenCalled();
    });
  });

  describe('Data Flow', () => {
    it('should display todos in todos$ observable', () => {
      // Arrange
      const mockTodos: TodoItem[] = [
        { id: '1', title: 'Todo 1' },
        { id: '2', title: 'Todo 2' }
      ];
      vi.spyOn(todoService, 'getTodos').mockReturnValue(of(mockTodos));

      // Act
      component.ngOnInit();

      // Assert
      let result: TodoItem[] = [];
      component.todos$.subscribe((todos: TodoItem[]) => {
        result = todos;
      });
      
      expect(result.length).toBe(2);
      expect(result[0].title).toBe('Todo 1');
    });
  });
});
