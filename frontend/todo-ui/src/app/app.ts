import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BehaviorSubject, Observable } from 'rxjs';
import { tap, switchMap } from 'rxjs/operators';

import { TodoItem } from './todo-item';
import { TodoService } from './todo.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {

  todos$!: Observable<TodoItem[]>;
  private refreshTodos$ = new BehaviorSubject<void>(undefined);

  newTodoTitle = '';

  constructor(private todoService: TodoService) {
  }

  ngOnInit(): void {
    this.todos$ = this.refreshTodos$.pipe(
      switchMap(() => this.todoService.getTodos())
    );
  }

  addTodo(): void {

    if (!this.newTodoTitle.trim()) {
      return;
    }

    this.todoService.addTodo(this.newTodoTitle)
      .pipe(
        tap(() => {
          this.newTodoTitle = '';
          this.refreshTodos$.next();
        })
      )
      .subscribe();
  }

  deleteTodo(id: string): void {

    this.todoService.deleteTodo(id)
      .pipe(
        tap(() => {
          this.refreshTodos$.next();
        })
      )
      .subscribe();
  }
}