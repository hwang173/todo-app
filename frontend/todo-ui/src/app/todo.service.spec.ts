import { describe, it, expect, beforeEach, afterEach } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TodoService } from './todo.service';
import { TodoItem } from './todo-item';

describe('TodoService', () => {
  let service: TodoService;
  let httpMock: HttpTestingController;
  const apiUrl = 'http://localhost:5209/api/todos';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TodoService]
    });
    service = TestBed.inject(TodoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  describe('getTodos', () => {
    it('should fetch todos from the API', () => {
      // Arrange
      const mockTodos: TodoItem[] = [
        { id: '1', title: 'Test 1' },
        { id: '2', title: 'Test 2' }
      ];

      // Act
      let result: TodoItem[] = [];
      service.getTodos().subscribe((todos: TodoItem[]) => {
        result = todos;
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockTodos);

      // Assert
      expect(result).toEqual(mockTodos);
      expect(result.length).toBe(2);
    });

    it('should return empty array when no todos exist', () => {
      // Arrange
      const mockTodos: TodoItem[] = [];

      // Act
      let result: TodoItem[] = [];
      service.getTodos().subscribe((todos: TodoItem[]) => {
        result = todos;
      });

      const req = httpMock.expectOne(apiUrl);
      req.flush(mockTodos);

      // Assert
      expect(result).toEqual([]);
      expect(result.length).toBe(0);
    });

    it('should handle API errors', () => {
      // Arrange
      let errorStatus = 0;

      // Act & Assert
      service.getTodos().subscribe({
        next: () => {
          throw new Error('Should have failed');
        },
        error: (error: any) => {
          errorStatus = error.status;
        }
      });

      const req = httpMock.expectOne(apiUrl);
      req.flush('Server error', { status: 500, statusText: 'Internal Server Error' });
      
      expect(errorStatus).toBe(500);
    });
  });

  describe('addTodo', () => {
    it('should add a new todo', () => {
      // Arrange
      const title = 'New Test Todo';
      const mockResponse: TodoItem = { id: '3', title };

      // Act
      let result: TodoItem | null = null;
      service.addTodo(title).subscribe((todo: TodoItem) => {
        result = todo;
      });

      const req = httpMock.expectOne(apiUrl);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ title });
      req.flush(mockResponse);

      // Assert
      expect(result !== null).toBe(true);
      expect((result as unknown as TodoItem).title).toBe(title);
    });

    it('should send correct request body', () => {
      // Arrange
      const title = 'Another Todo';
      const mockResponse: TodoItem = { id: '4', title };
      let completed = false;

      // Act
      service.addTodo(title).subscribe(() => {
        completed = true;
      });

      // Assert
      const req = httpMock.expectOne(apiUrl);
      expect(req.request.body).toEqual({ title });
      req.flush(mockResponse);
      expect(completed).toBe(true);
    });

    it('should handle add todo errors', () => {
      // Arrange
      let errorStatus = 0;

      // Act & Assert
      service.addTodo('Test').subscribe({
        next: () => {
          throw new Error('Should have failed');
        },
        error: (error: any) => {
          errorStatus = error.status;
        }
      });

      const req = httpMock.expectOne(apiUrl);
      req.flush('Bad request', { status: 400, statusText: 'Bad Request' });
      
      expect(errorStatus).toBe(400);
    });
  });

  describe('deleteTodo', () => {
    it('should delete a todo by id', () => {
      // Arrange
      const todoId = '1';
      let completed = false;

      // Act
      service.deleteTodo(todoId).subscribe(() => {
        completed = true;
      });

      // Assert
      const req = httpMock.expectOne(`${apiUrl}/${todoId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
      
      expect(completed).toBe(true);
    });

    it('should use correct delete endpoint', () => {
      // Arrange
      const todoId = 'abc-123';
      let completed = false;

      // Act
      service.deleteTodo(todoId).subscribe(() => {
        completed = true;
      });

      // Assert
      const req = httpMock.expectOne(`${apiUrl}/${todoId}`);
      expect(req.request.method).toBe('DELETE');
      req.flush(null);
      
      expect(completed).toBe(true);
    });

    it('should handle delete errors', () => {
      // Arrange
      const todoId = 'non-existent';
      let errorStatus = 0;

      // Act & Assert
      service.deleteTodo(todoId).subscribe({
        next: () => {
          throw new Error('Should have failed');
        },
        error: (error: any) => {
          errorStatus = error.status;
        }
      });

      const req = httpMock.expectOne(`${apiUrl}/${todoId}`);
      req.flush('Not found', { status: 404, statusText: 'Not Found' });
      
      expect(errorStatus).toBe(404);
    });
  });
});
