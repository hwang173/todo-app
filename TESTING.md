# Testing Guide for Todo App

This document describes how to run tests for both the backend and frontend of the todo-app.

## Backend Tests (.NET)

### Prerequisites
- .NET 9 SDK installed
- Visual Studio or Visual Studio Code with C# extension

### Running Backend Tests

Navigate to the backend directory and run tests:

```bash
cd backend
dotnet test TodoApi.Tests/TodoApi.Tests.csproj
```

Or run tests from the root directory:

```bash
dotnet test backend/TodoApi.Tests/TodoApi.Tests.csproj
```

### Test Coverage

The backend test suite includes:

**TodoServiceTests** (10 tests)
- `GetAll_WhenNoTodosExist_ReturnsEmptyList` - Verifies empty list is returned when no todos exist
- `Add_WithValidTitle_ReturnsTodoItem` - Verifies a todo is created with the provided title
- `Add_WithValidTitle_AddsTodoToList` - Verifies the todo is added to the list
- `Add_WithMultipleTitles_AddsMultipleTodos` - Verifies multiple todos can be added
- `Delete_WithExistingId_RemovesTodo` - Verifies a todo is removed by ID
- `Delete_WithNonExistingId_ReturnsFalse` - Verifies delete returns false for non-existent IDs
- `Delete_WithNonExistingId_DoesNotAffectOtherTodos` - Verifies delete doesn't affect other todos
- `Delete_RemovesOnlyTargetedTodo` - Verifies only the targeted todo is removed

**TodosControllerTests** (9 tests)
- `GetAll_ReturnsOkWithTodos` - Verifies GET endpoint returns todos
- `GetAll_WhenNoTodosExist_ReturnsEmptyList` - Verifies empty list response
- `Add_WithValidRequest_ReturnsOkWithTodo` - Verifies POST endpoint creates todos
- `Add_CallsTodoServiceWithCorrectTitle` - Verifies correct data is passed to service
- `Delete_WithExistingId_ReturnsNoContent` - Verifies DELETE endpoint returns 204
- `Delete_WithNonExistingId_ReturnsNotFound` - Verifies DELETE returns 404 for non-existent IDs

### Viewing Test Results

After running `dotnet test`, you'll see a summary like:

```
Test Run Successful.
Total tests: 19
     Passed: 19
     Failed: 0
 Skipped: 0
```

For more detailed output:

```bash
dotnet test --verbosity:detailed
```

## Frontend Tests (Angular)

### Prerequisites
- Node.js 18+ installed
- npm or yarn package manager

### Running Frontend Tests

Navigate to the frontend directory:

```bash
cd frontend/todo-ui
```

Install dependencies if you haven't already:

```bash
npm install
```

Run the test suite:

```bash
npm test
```

Or run with coverage:

```bash
npm test -- --coverage
```

### Test Coverage

The frontend test suite includes:

**TodoServiceTests** (9 tests)
- `getTodos` - Fetches todos from the API
  - Should fetch todos from the API
  - Should return empty array when no todos exist
  - Should handle API errors
  
- `addTodo` - Adds a new todo
  - Should add a new todo
  - Should send correct request body
  - Should handle add todo errors
  
- `deleteTodo` - Deletes a todo
  - Should delete a todo by id
  - Should use correct delete endpoint
  - Should handle delete errors

**AppComponentTests** (17 tests)
- **Component Initialization** (3 tests)
  - Should create the app component
  - Should initialize with empty newTodoTitle
  - Should have todos$ observable after init

- **ngOnInit** (2 tests)
  - Should initialize todos$ observable
  - Should call getTodos on init

- **addTodo** (5 tests)
  - Should add a new todo with valid title
  - Should clear title after adding todo
  - Should not add todo with empty title
  - Should not add todo with only whitespace
  - Should refresh todos after adding

- **deleteTodo** (3 tests)
  - Should delete a todo by id
  - Should refresh todos after deletion
  - Should handle delete errors gracefully

- **Data Flow** (1 test)
  - Should display todos in todos$ observable

### Watch Mode

To run tests in watch mode (re-run on file changes):

```bash
npm test -- --watch
```

### Viewing Test Results

Tests results will be displayed in the terminal with:
- Pass/Fail status for each test
- Execution time
- Coverage information (if enabled)

Example output:

```
✓ src/app/todo.service.spec.ts (9)
✓ src/app/app.spec.ts (17)

Test Files  2 passed (2)
     Tests  26 passed (26)
```

## Continuous Integration

Both test suites are designed to be easily integrated into CI/CD pipelines:

### Backend CI Example

```bash
dotnet restore
dotnet build
dotnet test --no-build --verbosity:normal
```

### Frontend CI Example

```bash
npm ci
npm run build
npm test
```

## Test Debugging

### Backend
- Use Visual Studio debugger or VS Code debugger
- Add breakpoints in test files
- Run with `dotnet test` in debug mode

### Frontend
- Use Chrome DevTools for debugging
- Add `debugger;` statements in tests
- Use `--inspect-brk` flag with Node for breakpoint debugging

## Best Practices

1. **Isolation**: Each test should be independent and not depend on other tests
2. **Clear Names**: Test names clearly describe what is being tested
3. **Arrange-Act-Assert**: Follow the AAA pattern for organizing tests
4. **Mocking**: Mock external dependencies (HTTP calls, services)
5. **Coverage**: Aim for high coverage but focus on critical paths
6. **Readability**: Keep tests simple and easy to understand

